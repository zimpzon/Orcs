TODO
  1) why would we ever level up? -> add level requirements on upgrades.
  1.1) 5K damage in 20 seconds! Bonus gold: 12345
  2) enemies that provide other bonuses like throwing gold
  3) much better bonus for completion


When adding a new upgrade:

Add prefab to scroll area
Create file Upgrades/[new name]Manager.cs, fill it in like the other managers
Update UpgradeManager with this new Upgrades/[new name]Manager.cs
Create UpgradeManager UpdateUpgradeUi, OnBuy, etc.
Drag the prefab (script) to UpdateManagers new 
Add button click event on added prefab, call OnBuy[new name]

flow:

buy button is enabled if enough money
    UpgradeManager.Update
    UpgradeManager.UpdateUpgradeUiButtons
    UpdateUI for each [name]Manager