using System;
using UnityEngine;
using System.Linq;

namespace Assets.Script
{
    public enum Enemy { small_skeleton, large_skeleton, caster_skeleton, charger_skeleton };

    [Serializable]
    public class Wave
    {
        public float start_time = 0.0f;
        public Enemy enemy_type = Enemy.small_skeleton;
        public int count = 10;
        public PositionUtility.SpawnDirection where = PositionUtility.SpawnDirection.Left;
        public float interval = 0.1f;
    }

    [Serializable]
    public class SandboxData
    {
        public string challenge_name = "<whatever name you like>";
        public string creator_name = "<your name>";
        public Color background_tint = Color.white;
        public int weapon_left_click = 1;
        public int weapon_right_click = 0;
        public float enemy_hitpoint_scale = 1.0f;
        public float enemy_move_speed_scale = 1.0f;
        public float enemy_fire_interval_scale = 1.0f;
        public float enemy_fire_count_scale = 1.0f;
        public float enemy_charge_cooldown_scale = 1.0f;
        public float player_move_speed_scale = 1.0f;
        public Wave[] Waves = new Wave[0];

        public static SandboxData TryParse(string json, out string error)
        {
            error = "";
            try
            {
                return FromJson(json);
            }
            catch(Exception e)
            {
                error = e.Message;
            }
            return null;
        }

        public string ToJson()
        {
            var result = JsonUtility.ToJson(this, true);
            result = result.Replace("\n", Environment.NewLine);
            return result;
        }

        public static SandboxData FromJson(string json)
        {
            // Remove lines containing #
            var lines = json.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            lines = lines.Where(l => !l.Contains("#")).ToArray();
            var cleanedJson = string.Concat(lines).Trim();

            return JsonUtility.FromJson<SandboxData>(cleanedJson);
        }
    }

    public class Sandbox
    {
        public string GetExample1() { return Example1; }
        public string GetExample2() { return Example2; }

        public static string Example1 = @"{
    ""challenge_name"": ""a simple example"",
    ""creator_name"": ""<your name>"",
    ""weapon_left_click"": 8,
    ""Waves"": [
        {
            ""start_time"": 0.0,
            ""enemy_type"": 2,
            ""count"": 10,
            ""where"": 6,
            ""interval"": 1.0
        },
        {
            ""start_time"": 3.0,
            ""enemy_type"": 5,
            ""count"": 4,
            ""where"": 0,
            ""interval"": 2.0
        }
    ]
}
​";
        public static string Example2 = @"{
    ""challenge_name"": ""a larger example"",
    ""creator_name"": ""<your name>"",
    ""background_tint"": {
        ""r"": 0.6,
        ""g"": 1.0,
        ""b"": 0.4
    },
    ""weapon_left_click"": 4,
    ""weapon_right_click"": 3,
    ""enemy_hitpoint_scale"": 1.0,
    ""enemy_move_speed_scale"": 1.0,
    ""enemy_fire_interval_scale"": 1.0,
    ""enemy_fire_count_scale"": 1.0,
    ""enemy_charge_cooldown_scale"": 1.0,
    ""player_move_speed_scale"": 1.0,
    ""Waves"": [
        {
            ""start_time"": 0.0,
            ""enemy_type"": 2,
            ""count"": 100,
            ""where"": 6,
            ""interval"": 0.25
        },
        {
            ""start_time"": 3.0,
            ""enemy_type"": 3,
            ""count"": 20,
            ""where"": 6,
            ""interval"": 1.0
        },
        {
            ""start_time"": 10.0,
            ""enemy_type"": 5,
            ""count"": 10,
            ""where"": 4,
            ""interval"": 2.0
        }
    ]
}
​";
    }
}
