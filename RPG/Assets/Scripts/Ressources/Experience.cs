using RPG.Saving;
using UnityEngine;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        // experience point counter
        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        // getter for experience display
        public float GetExperience()
        {
            return experiencePoints;
        }    

        // saving system
        #region>>>>
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
