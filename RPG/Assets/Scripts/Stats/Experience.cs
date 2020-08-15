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

        // experience point counter
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            currentLevel = baseStats.GetLevel();
            experienceDisplay.UpdateUIText(currentLevel);
        }

        // getter for experience display
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
