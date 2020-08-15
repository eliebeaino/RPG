using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        public float GetStat(Stat stat,CharacterClass characterClass, int level)
        {
            // search the listings for character class that is passed in (enum)
            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                if (progressionCharacterClass.characterClass != characterClass) continue;

                // search the listings for the stat that is passed in (enum)
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                { 
                    if (progressionStat.stat != stat) continue;

                    // guard in case the array is too small
                    if (progressionStat.levels.Length < level) continue;

                    // return appropreiate stat according to level passed in (float)
                    return progressionStat.levels[level - 1];
                }
            }
            Debug.Log("Progression field empty for: " + characterClass + " - " + stat + " - level: " + level);
            return 0; // in case nothing is found
        }

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

    }
}