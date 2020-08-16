using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] [Range(1, 100)] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUPVFX = null;
        [SerializeField] bool useModifiers = false;

        public event Action onLevelUp; 
        Experience experience;
        int currentLevel = 0;

        private void Awake()
        {
            experience = GetComponent<Experience>();
        }

        // adds action for exp gain
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
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUPVFX, transform);
        }

        #region Get Stat
        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        // TODO - dont understand
        private float GetAdditiveModifier(Stat stat)
        {
            if (!useModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentModifier(Stat stat)
        {
            if (!useModifiers) return 0;

            float total = 0;
            foreach (IModifierProvider provide in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provide.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }
        #endregion

        public int GetLevel()
        {
            // avoid bug from excecution order of scripts
            if (currentLevel < 1)
            {
                currentLevel = CalculateLevel();  
            }
            return currentLevel;
        }

        private int CalculateLevel()
        {
            #region TODELETE
            // remove this later when we setup XP reset to 0 between levels
            // placeholder to allow changing levels in inspector for player
            if (startingLevel != 1)
            {  
                int tempLevel = startingLevel;
                if (experience)
                {
                    experience.SetExperience(progression.GetStat(Stat.ExperienceToLevelUp, characterClass, startingLevel - 1));
                    startingLevel = 1; //This makes the startingLevel work only at the beginning of the scene - so when we gain experience it never checks this statment again
                }
                return tempLevel;
            }
            #endregion  

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