using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        #region Inspector Serialize Arrays & Classes
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;

        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
        #endregion

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        // build a dictionary out of all the progression file - once - when first called
        private void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progressionCharacterClass.characterClass] = statLookupTable;
            }
        }

        // gets called everytime we need to check a stat from our progression asset file
        public float GetStat(Stat stat,CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level)
            {
                Debug.Log("Progression field empty for: " + characterClass + " - " + stat + " - level: " + level);
                return 0;
            }
            return levels[level -  1];
        }

        // getter for the level lenght
        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            return lookupTable[characterClass][stat].Length;
        }
    }
}

// old code
//// gets called to check the max level achievable for the player
//public int GetMaxlevels(Stat stat, CharacterClass characterClass)
//{
//    return lookupTable[characterClass][stat].Length + 1;
//}



//// search the listings for character class that is passed in (enum)
//foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
//{
//    if (progressionCharacterClass.characterClass != characterClass) continue;

//    // search the listings for the stat that is passed in (enum)
//    foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
//    { 
//        if (progressionStat.stat != stat) continue;

//        // guard in case the array is too small
//        if (progressionStat.levels.Length < level) continue;

//        // return appropreiate stat according to level passed in (float)
//        return progressionStat.levels[level - 1];
//    }
//}
//Debug.Log("Progression field empty for: " + characterClass + " - " + stat + " - level: " + level);
//return 0; // in case nothing is found