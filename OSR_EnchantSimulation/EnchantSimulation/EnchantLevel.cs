using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnchantSimulation
{
    public class EnchantLevel
    {
        private static readonly Dictionary<int, double> LegendArmorProbabilities = new Dictionary<int, double>
        {
            {0, 1 }, // placeholder; actual percentage should be user parameter
            {1, 0.6 },
            {2, 0.5 },
            {3, 0.4 },
            {4, 0.3 },
            {5, 0.2 },
            {6, 0.1 }
        };

        private static readonly Dictionary<int, double> LegendWeaponProbabilities = new Dictionary<int, double>
        {
            {0, 1 }, // placeholder; actual percentage should be user parameter
            {1, 0.9 },
            {2, 0.8 },
            {3, 0.7 },
            {4, 0.6 },
            {5, 0.5 },
            {6, 0.4 },
            {7, 0.3 },
            {8, 0.2 },
            {9, 0.1 },
            {10, 0.1 },
            {11, 0.05 },
            {12, 0.05 },
            {13, 0.05 },
            {14, 0.01 }
            
            /*,
            {14, 0.15 },
            {15, 0.05 },
            {16, 0.05 },
            {17, 0.02 },
            {18, 0.02 },
            {19, 0.01 }*/
        };
        private static readonly Dictionary<int, double> WeaponArmorProbabilities = new Dictionary<int, double>
        {
            {0, 1 },
            {1, 1 },
            {2, 1 },
            {3, 1 },
            {4, 1 },
            {5, 0.9 },
            {6, 0.8 },
            {7, 0.6 },
            {8, 0.4 },
            {9, 0.2 },
            {10, 0.1 }, 
            {11, 0.05 }, 
            {12, 0.025 },
            {13, 0.0125 },
            {14, 0.005 }
            
            /*,
            {14, 0.15 },
            {15, 0.05 },
            {16, 0.05 },
            {17, 0.02 },
            {18, 0.02 },
            {19, 0.01 }*/
        };

        public int Level { get; set; }
        public bool UseSpCard { get; set;  }
        public bool Use3Booster { get; set; }
        public bool Use5Booster { get; set; }
        public bool Use8Booster { get; set; }
        public bool UseBEProt { get; set; }
        public bool UseEProt { get; set; }
        public bool UseHProt { get; set; }

        public enum ItemType { WeaponArmor, LegendArmor, LegendWeapon};

        public double GetDerivedProbability(int level, ItemType itemType)
        {
            if(itemType == ItemType.WeaponArmor) // weapon and armor
            {
                return WeaponArmorProbabilities[level] * (Use3Booster ? 1.03 : 1) * (Use5Booster ? 1.05 : 1) * (Use8Booster ? 1.08 : 1);
            }
            else if (itemType == ItemType.LegendArmor) // legend armor; enchant probabilities cannot be modified
            {
                return LegendArmorProbabilities[level];
            }
            
            else // legend weapon
            {
                return LegendWeaponProbabilities[level];
            }
        }        
    }    
}
