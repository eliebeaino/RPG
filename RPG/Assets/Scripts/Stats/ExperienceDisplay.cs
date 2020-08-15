using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] Text levelText;
        [SerializeField] Text xpText;
        Experience experience;

        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        public void UpdateUIText(int currentLevel)
        {
            xpText.text = experience.GetExperiencePoints().ToString("F0");
            levelText.text = currentLevel.ToString();
        }
    }
}