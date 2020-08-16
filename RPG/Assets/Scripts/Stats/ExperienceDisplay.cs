using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] Text levelText;
        [SerializeField] Text xpText;
        Experience experience;
        BaseStats baseStats;

        private void Awake()
        {
            GameObject player = GameObject.FindWithTag("Player");
            experience = player.GetComponent<Experience>();
            baseStats = player.GetComponent<BaseStats>();
        }

        private void OnEnable()
        {
            experience.onExperienceGained += UpdateUIText;
            baseStats.onLevelUp += UpdateUIText;
        }

        private void OnDisable()
        {
            experience.onExperienceGained -= UpdateUIText;
            baseStats.onLevelUp -= UpdateUIText;
        }

        private void Start()
        {
            UpdateUIText();
        }

        public void UpdateUIText()
        {
            xpText.text = experience.GetExperiencePoints().ToString("F0");
            levelText.text = baseStats.GetLevel().ToString();
        }
    }
}