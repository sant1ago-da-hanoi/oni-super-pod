using System;
using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

namespace SuperPOD
{
    internal class ConfigData
    {
        private List<string> stressList = new List<string>
        {
            "Aggressive", "StressVomiter", "UglyCrier", "BingeEater"
        };

        private List<string> overjoyedList = new List<string>
        {
            "BalloonArtist", "SparkleStreaker", "StickerBomber", "SuperProductive"
        };

        public float TimeBeforeSpawn { get; set; } = 1800f;
        public int DuplicantNumber { get; set; } = 3;
        public int CarePackageNumber { get; set; } = 1;
        public int InterestNumber { get; set; } = 3;
        public int InterestValue { get; set; } = 7;
        public int PositiveTraitsNumber { get; set; } = 3;
        public int NegativeTraitsNumber { get; set; } = 1;
        public string Stress { get; set; } = "Aggressive,StressVomiter,UglyCrier,BingeEater";
        public string Overjoyed { get; set; } = "BalloonArtist,SparkleStreaker,StickerBomber,SuperProductive";

        public static string GetStress()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            var list = config.Stress
                .Split(new[] { ',', '\uFF0C' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => config.stressList.Contains(s))
                .ToList();

            if (list.Count == 0)
                list = config.stressList;

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static string GetOverjoyed()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            var list = config.Overjoyed
                .Split(new[] { ',', '\uFF0C' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => config.overjoyedList.Contains(s))
                .ToList();

            if (list.Count == 0)
                list = config.overjoyedList;

            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static int GetDuplicantNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            CheckNumber();
            return config.DuplicantNumber;
        }

        public static int GetCarePackageNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            CheckNumber();
            return config.CarePackageNumber;
        }

        private static void CheckNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            config.DuplicantNumber = Mathf.Clamp(config.DuplicantNumber, 0, 10);
            config.CarePackageNumber = Mathf.Clamp(config.CarePackageNumber, 0, 10);

            if (config.DuplicantNumber + config.CarePackageNumber > 10)
            {
                config.DuplicantNumber = 3;
                config.CarePackageNumber = 1;
            }
        }

        public static int GetInterestNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            return Mathf.Clamp(config.InterestNumber, 0, 13);
        }

        public static int GetMaxTraits()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            CheckTraitsNumber(config);
            return config.PositiveTraitsNumber + config.NegativeTraitsNumber + 4;
        }

        public static int GetPositiveTraitsNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            CheckTraitsNumber(config);
            return config.PositiveTraitsNumber;
        }

        public static int GetNegativeTraitsNumber()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            CheckTraitsNumber(config);
            return config.NegativeTraitsNumber;
        }

        private static void CheckTraitsNumber(ConfigData cfg)
        {
            cfg.PositiveTraitsNumber = Mathf.Clamp(cfg.PositiveTraitsNumber, 0, DUPLICANTSTATS.GOODTRAITS.Count);
            cfg.NegativeTraitsNumber = Mathf.Clamp(cfg.NegativeTraitsNumber, 0, DUPLICANTSTATS.BADTRAITS.Count);
        }

        public static int GetInterestValue()
        {
            var config = ConfigHelper<ConfigData>.GetConfig("config.ini");
            int value = Mathf.Clamp(config.InterestValue, 0, 1000);
            int multiplier = UnityEngine.Random.Range(0, 101) <= 90 ? 1 : 2;
            return value * multiplier;
        }

        public static float GetInterestValueAsFloat()
        {
            return GetInterestValue();
        }
    }
}
