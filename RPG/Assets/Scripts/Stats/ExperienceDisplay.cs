using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] Text levelText;
        [SerializeField] Text xpText;

        public void UpdateUIText(int currentLevel, float experience)
        {
            xpText.text = experience.ToString("F0");
            levelText.text = currentLevel.ToString();
        }
    }
}