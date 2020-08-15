using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 100)] int currentLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        Experience experience;
        int maxLevel;


        // cache componenets
        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        // stores max level achievable by the player
        private void Start()
        {
            if (experience != null)
            {
                maxLevel = progression.GetMaxlevels(Stat.ExperienceToLevelUp, characterClass);
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }
        

        public int GetLevel()
        {
            // exit loop for characters without experience - mostly if not all enemies -- also exit for player when reached max level
            if (experience == null && currentLevel != maxLevel) return currentLevel;

            float currentXP = experience.GetExperience();
            float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, currentLevel);
            if (currentXP >= XPToLevelUp && currentLevel <= maxLevel)
            {
                return currentLevel++;
                // TODO add experience resetter for next level
            }
            return currentLevel;
        }
    }
}