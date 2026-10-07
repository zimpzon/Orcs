"""
IdleKnight balance simulator: whole playthroughs of runs + rebirths, to compare credit curves, rebirth rewards and
tier prices before changing the game. Plain Python 3, no dependencies.

    python Tools/balance_sim.py                 # compare the built-in experiments (see EXPERIMENTS at the bottom)
    python Tools/balance_sim.py --runs current  # per-run table for one experiment

Model (deliberately simple - trust the direction and relative differences more than absolute numbers):
- 24 tiers with the real prices/incomes from UpgradeProgression.cs (keep TIER_PRICES / TIER_INCOMES in sync).
  Level price x1.15 per level, X2 price = 10 x tier price x 3^n with the Cookie Clicker level requirements,
  each X2 doubles that tier. 1% bonuses (price curve copied), X2 ranks (+10% per 5 X2s, +10% per X2 Mastery).
- Money per second = passive income x MONEY_FACTOR (arena rounds 3 s/round, chests, mysteries).
- The player buys the MOST EXPENSIVE affordable thing (level, X2 or 1%) - players don't buy greedily from the bottom.
- Credits: credit XP = passive income (with the credit speed floor and the optional decaying credit bonus).
- Rebirth when the current credit rate drops below the run's average credit rate (the optimal rule), min 30 min.
  On rebirth: credits -> diamonds, rebirth bonus (+1..10%), then cards are bought when they pay off.
- Missing / approximate: skins (Skin Collector), completion (Completionist), arena combat and arena level, bestiary
  (approximated from the best tier reached), offline time, mystery buffs (folded into MONEY_FACTOR).
"""
import argparse
import math

# ---------------------------------------------------------------- game data (from UpgradeProgression.cs)
_P = [50, 300, 3_100, 42_000, 750_000, 10.1e6, 150e6, 2.1e9, 20.5e9, 210e9, 2.3e12, 21.1e12, 330.1e12, 6.5001e15,
      250e15, 15.25e18]
# Tiers 17..24 are defined relative to the previous one (FastFeet = SmartDaggers*100, ...).
CURRENT_LATE_STEPS = [100, 100, 80, 60, 10, 10, 10, 10]
TIER_INCOMES = [0.2, 2, 10, 50, 275, 1_400, 1e4, 5e4, 2.75e5, 1.75e6, 1e7, 6.2e7, 3e8, 1.4e9, 5.5e9, 2.52e10, 1.101e11,
                5.0025e11, 2.50025e12, 1.0625625e13, 4.25025e13, 1.7001e14, 6.8004e14, 2.72016e15]
TIER_NAMES = ["Chain Zapping", "Dagger Damage", "Richer Chests", "Dagger Cooldown", "Witch Doctor", "Gold Per Dagger",
              "Wizard", "Master Wizard", "Frenzy", "Necromancer", "Dagger Master", "Necro Ninja", "Skull Crusher",
              "Bountiful", "Voidgazer", "Smart Daggers", "Windwalker", "Crypt Master", "Angry Fireballs", "Beefy Earl",
              "Critical Strike", "Power Zap", "Skull Slicer", "Storm Lord"]
N = 24


def tier_prices(steps_from_15=None):
    """steps_from_15: price step for tiers 15..24 (10 values) relative to the previous tier, or None for current."""
    if steps_from_15 is None:
        p = list(_P)
        for s in CURRENT_LATE_STEPS:
            p.append(p[-1] * s)
        return p
    p = list(_P[:14])
    for s in steps_from_15:
        p.append(p[-1] * s)
    return p


def x2_level_requirement(n):
    return [0, 1, 5, 25][n] if n <= 3 else (n - 3) * 25


def pct_price(n):
    return [500, 1e4, 1e5, 2.5e5][n] if n < 4 else 5e5 * 1.2378 ** (n - 3)


def current_credit_cost(level):
    """MonsterCreditXpForNextLevel: quadratic to 8, cubic after, x CreditCostMul 0.5 (gentle lifetime curve)."""
    if level <= 8:
        c = 5e9 * (1 + 2 * (level - 1) ** 2)
    else:
        c = 495e9 + 58e9 * (level - 8) ** 3
    return c * 0.5


MONEY_FACTOR = 1.4
EXTRA_MULT = 1.0  # see simulate(): unsimulated bonuses at the last tier; ~10-20 is realistic with skins+completion
DT = 20.0  # seconds per simulation step


# ---------------------------------------------------------------- simulation
def simulate(cfg, days=12.0, show_runs=False):
    prices = tier_prices(cfg.get("late_steps"))
    cost = cfg.get("credit_cost", current_credit_cost)
    credit_scale = cfg.get("credit_scale", 1.0)
    credit_bonus = cfg.get("credit_bonus", lambda t: 0.0)  # extra credit XP fraction, t = seconds into the run
    floor = cfg.get("credit_floor", 0.02)
    x2_cards = cfg.get("x2_cards", (2, 2, 2, 2))
    x2_card_cost = cfg.get("x2_card_cost", (1, 2, 6, 12, 25, 50, 100, 200))
    diamond_pct = cfg.get("diamond_pct", 0.08)  # per diamond (game: linear 8% + Shiny cards)
    # Bonuses the model doesn't simulate (Skin Collector, Completionist, mystery buffs): a flat factor that grows
    # with the best tier reached, up to extra_mult at the last tier.
    extra_mult = cfg.get("extra_mult", EXTRA_MULT)
    shiny_cost, shiny_add = (50, 250, 500, 1000, 2000), (0.08, 0.12, 0.16, 0.2, 0.24)

    # Per-run penalty (UpgradeProgression.XpForNextCredit): price doubles per run_penalty_fraction of the lifetime
    # credits before this run (min 5) earned in this run; 0 disables it.
    run_penalty_fraction = cfg.get("run_penalty_fraction", 0.5)
    run_credits = [0]

    def credit_price(level):
        c = cost(level) * credit_scale
        if run_penalty_fraction > 0:
            size = max(5, (level - 1 - run_credits[0]) * run_penalty_fraction)
            c *= 2 ** (run_credits[0] / size)
        return c

    perm = dict(diamonds=0, x2c=0, shiny=0, haggler=0, pct10=False, mastery=0, discount=False,
                lifetime=0, rebirth_pct=0, max_income=0.0, best_tier=0)
    total = 0.0
    first_credit = None
    tier_reached = {}
    runs = []

    # Diamond bonus. Game: linear, d x (8% + Shiny 8..24%). diamond_power 0.5 = sqrt experiment (bonus x pct x shiny).
    diamond_power = cfg.get("diamond_power", 1.0)

    def diamond_mult(d, shiny=None):
        s = perm["shiny"] if shiny is None else shiny
        if diamond_power == 1.0:
            return 1 + max(0, d) * (diamond_pct + sum(shiny_add[:s]))
        return 1 + max(0, d) ** diamond_power * diamond_pct * (1 + sum(shiny_add[:s]))

    while total < days * 86400:
        lv, x2 = [0] * N, [0] * N
        pct, money, t, xp, credits = 0, 100.0, 0.0, 0.0, 0
        run_credits[0] = 0
        shop = 0.5 ** perm["haggler"]
        bestiary = 1 + 7.8 * (perm["best_tier"] / 24) ** 2
        bestiary *= extra_mult ** (perm["best_tier"] / 24)
        while True:
            m = 1.0
            for k in range(perm["x2c"]):
                m *= x2_cards[k]
            m *= diamond_mult(perm["diamonds"]) * (1 + perm["rebirth_pct"] / 100) * bestiary
            m *= 1 + (0.1 + 0.1 * perm["mastery"]) * (sum(x2) // 5)
            m *= 1 + 0.01 * pct * (10 if perm["pct10"] else 1)
            income = sum(TIER_INCOMES[i] * lv[i] * 2 ** x2[i] for i in range(N)) * m
            perm["max_income"] = max(perm["max_income"], income)
            money += (income * MONEY_FACTOR + (5 if t < 600 else 0)) * DT  # small early-arena floor
            rate = max(income, perm["max_income"] * floor) * (1 + credit_bonus(t))
            xp += rate * DT
            while xp >= credit_price(perm["lifetime"] + 1):
                xp -= credit_price(perm["lifetime"] + 1)
                perm["lifetime"] += 1
                credits += 1
                run_credits[0] = credits
                if first_credit is None:
                    first_credit = total + t

            while True:  # buy the most expensive affordable thing, repeatedly
                best = None
                for i in range(N):
                    p = prices[i] * 1.15 ** lv[i] * shop
                    if p <= money and (best is None or p > best[0]):
                        best = (p, 0, i)
                    if lv[i] > 0 and lv[i] >= x2_level_requirement(x2[i] + 1):
                        p = prices[i] * 10 * 3 ** (x2[i] + 1) * shop
                        if p <= money and (best is None or p > best[0]):
                            best = (p, 1, i)
                p = pct_price(pct)
                if p <= money and (best is None or p > best[0]):
                    best = (p, 2, 0)
                if best is None:
                    break
                money -= best[0]
                if best[1] == 0:
                    lv[best[2]] += 1
                elif best[1] == 1:
                    x2[best[2]] += 1
                else:
                    pct += 1

            t += DT
            for i in range(N):
                if lv[i] > 0 and i not in tier_reached:
                    tier_reached[i] = total + t
            if credits >= 1 and t > 1800:
                now = rate / credit_price(perm["lifetime"] + 1)
                avg = (credits + xp / credit_price(perm["lifetime"] + 1)) / t
                if now < avg or t > 4 * 86400:
                    break
            if total + t > days * 86400:
                break

        total += t
        top = max([i for i in range(N) if lv[i] > 0], default=-1) + 1
        perm["best_tier"] = max(perm["best_tier"], top)
        runs.append(dict(day=total / 86400, hours=t / 3600, credits=credits, lifetime=perm["lifetime"],
                         income=income, top=top))

        # Rebirth: bonus (1..10% by credits vs target), credits -> diamonds, then cards that pay off.
        target = max(5, math.ceil((perm["lifetime"] - credits) * 0.10))
        perm["rebirth_pct"] += min(10, 1 + 9 * credits // target) if credits > 0 else 0
        perm["diamonds"] += credits
        bought = True
        while bought:
            bought = False
            disc = 0.8 if perm["discount"] else 1.0
            d = perm["diamonds"]
            options = []
            if perm["x2c"] < len(x2_cards):
                options.append(("x2", x2_card_cost[perm["x2c"]] * disc))
            if perm["shiny"] < 5:
                options.append(("shiny", shiny_cost[perm["shiny"]] * disc))
            if perm["haggler"] < 3:
                options.append(("haggler", (2500, 5000, 10000)[perm["haggler"]] * disc))
            if not perm["pct10"]:
                options.append(("pct10", 3500 * disc))
            if perm["mastery"] < 3:
                options.append(("mastery", (1500, 3000, 6000)[perm["mastery"]] * disc))
            if not perm["discount"]:
                options.append(("discount", 1000))
            for name, c in sorted(options, key=lambda o: o[1]):
                c = max(1, round(c))
                if c > d:
                    continue
                loss = diamond_mult(d) / diamond_mult(d - c)  # held diamonds give income
                if name == "x2":
                    if x2_cards[perm["x2c"]] < loss:
                        continue
                    perm["x2c"] += 1
                elif name == "shiny":
                    if diamond_mult(d - c, perm["shiny"] + 1) < diamond_mult(d):
                        continue
                    perm["shiny"] += 1
                else:
                    if loss > 1.5:  # utility cards: only when it doesn't hurt income much
                        continue
                    if name == "haggler":
                        perm["haggler"] += 1
                    elif name == "pct10":
                        perm["pct10"] = True
                    elif name == "mastery":
                        perm["mastery"] += 1
                    else:
                        perm["discount"] = True
                perm["diamonds"] -= c
                bought = True
                break

    if show_runs:
        for i, r in enumerate(runs, 1):
            print("  run %3d  day %5.1f  %6.1f h  credits %4d  lifetime %5d  income %.1e  top tier %2d (%s)" % (
                i, r["day"], r["hours"], r["credits"], r["lifetime"], r["income"], r["top"],
                TIER_NAMES[r["top"] - 1] if r["top"] else "-"))
    hours = sorted(r["hours"] for r in runs)
    q = lambda f: hours[min(len(hours) - 1, int(len(hours) * f))]
    return dict(first_credit_h=first_credit / 3600 if first_credit else None,
                storm_day=tier_reached[N - 1] / 86400 if N - 1 in tier_reached else None,
                tier18_day=tier_reached[17] / 86400 if 17 in tier_reached else None,
                runs=len(runs), run_p25=q(0.25), run_med=q(0.5), run_p90=q(0.9), run_max=hours[-1],
                lifetime=perm["lifetime"], best_tier=perm["best_tier"])


def summary(name, res):
    f = lambda v, fmt: "never" if v is None else fmt % v
    print("%-34s first credit %-6s | tier 18 %-6s | Storm Lord %-6s | runs %3d | run h p25/med/p90/max "
          "%4.1f/%4.1f/%4.1f/%4.1f | lifetime %5d | best tier %2d" % (
              name, f(res["first_credit_h"], "%.1fh"), f(res["tier18_day"], "%.1fd"), f(res["storm_day"], "%.1fd"),
              res["runs"], res["run_p25"], res["run_med"], res["run_p90"], res["run_max"], res["lifetime"],
              res["best_tier"]))


# ---------------------------------------------------------------- experiments
def decaying_bonus(start=2.0, end=0.1, tau_hours=2.0):
    """Credit earning bonus that starts at `start` (2.0 = +200%) and decays toward `end` over the run."""
    return lambda t: end + (start - end) * math.exp(-t / (tau_hours * 3600))


SMOOTH = lambda s: [s] * 6 + [10, 10, 10, 10]  # tiers 15..20 at x s, 21..24 x10 as now (15/16 are fixed today: x38/x61)
FIRST_5H = 1.0  # the game already uses CreditCostMul 0.065 (first credit ~5 h)

EXPERIMENTS = {
    "current": {},
    "no per-run penalty": dict(run_penalty_fraction=0),
    "per-run penalty 0.25": dict(run_penalty_fraction=0.25),
    "per-run penalty 1.0": dict(run_penalty_fraction=1.0),
    "+ bonus 200%->10%": dict(credit_scale=FIRST_5H, credit_bonus=decaying_bonus()),
    "tiers 15-20 x30": dict(late_steps=SMOOTH(30)),
    "tiers 15-20 x25": dict(late_steps=SMOOTH(25)),
    "tiers 15-20 x20": dict(late_steps=SMOOTH(20)),
    "x25 + 5h + bonus": dict(late_steps=SMOOTH(25), credit_scale=FIRST_5H, credit_bonus=decaying_bonus()),
    "x20 + 5h + bonus": dict(late_steps=SMOOTH(20), credit_scale=FIRST_5H, credit_bonus=decaying_bonus()),
}

if __name__ == "__main__":
    ap = argparse.ArgumentParser()
    ap.add_argument("--runs", metavar="EXPERIMENT", help="print the per-run table for one experiment")
    ap.add_argument("--days", type=float, default=12.0)
    ap.add_argument("--extra", type=float, default=1.0, help="EXTRA_MULT: unsimulated bonuses (skins, completion)")
    args = ap.parse_args()
    EXTRA_MULT = args.extra
    if args.runs:
        summary(args.runs, simulate(EXPERIMENTS[args.runs], args.days, show_runs=True))
    else:
        for name, cfg in EXPERIMENTS.items():
            summary(name, simulate(cfg, args.days))
