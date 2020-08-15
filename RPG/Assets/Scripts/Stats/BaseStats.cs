using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 100)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        Experience experience;
        int currentLevel = 0;

        // cache componenets
        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        // adds action for exp gain - intialize start level
        private void Start()
        {
            currentLevel = CalculateLevel();
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        // called from Action for recalculation of level when we have experience gain
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                print("Levelled Up!");
            }
        }

        // getter for any stat
        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        // getter for level - used for display
        public int GetLevel()
        {
            // avoid bug from excecution order of scripts
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();  
            }
            return currentLevel;
        }

        public int CalculateLevel()
        {

            if (experience == null) return startingLevel;
            float currentXP = experience.GetExperiencePoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentXP)
                {
                    return level;
                }
            }
            return penultimateLevel + 1;
        }
    }
}


// calculate the level - explained inside 
//public int CalculateLevel()
//{
//    // exit loop for characters without experience - mostly if not all enemies -- also exit for player when reached max level
//    if (experience == null || level == maxLevel) return level;

//    float currentXP = experience.GetExperiencePoints();
//    float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
//    Debug.Log("exp needed to level up: " + XPToLevelUp);
//    if (currentXP >= XPToLevelUp && level <= maxLevel)
//    {
//        Debug.Log("level up " + gameObject.name + " - " + level++) ;
//        return level++;
//        // TODO add experience resetter for next level
//    }
//    Debug.Log("same level " + gameObject.name + " - " + level);
//    return level;
//}