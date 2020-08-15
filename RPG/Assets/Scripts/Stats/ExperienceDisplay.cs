using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Text text;
        Experience experience;

        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            text = GetComponent<Text>();
        }

        private void Update()
        {
            text.text = experience.GetExperience().ToString("F0");
        }
    }
}