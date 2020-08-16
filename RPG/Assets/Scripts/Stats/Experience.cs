using RPG.Saving;
using System;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        int currentLevel = 1;
        ExperienceDisplay experienceDisplay;
        BaseStats baseStats;

        public event Action onExperienceGained;
        
        private void Awake()
        {
            experienceDisplay = FindObjectOfType<ExperienceDisplay>();
            baseStats = GetComponent<BaseStats>();
        }

        private void Start()
        {
            UpdateDisplay();
        }

        // Add Experience points
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
            UpdateDisplay();
        }

        // updates experience and lvl on XP gain - and at start of scene
        private void UpdateDisplay()
        {
            currentLevel = baseStats.GetLevel();
            experienceDisplay.UpdateUIText(currentLevel, experiencePoints);
        }

        // getter for experience
        public float GetExperiencePoints()
        {
            return experiencePoints;
        }    

        #region Save ExperiencePoints
        public object CaptureState()
        {
            return experiencePoints;
        }

        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
        #endregion
    }
}
