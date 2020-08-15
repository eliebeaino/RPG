using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        Text text;
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            text = GetComponent<Text>();
        }

        private void Update()
        {
            text.text = baseStats.GetLevel().ToString("F0");
        }
    }
}