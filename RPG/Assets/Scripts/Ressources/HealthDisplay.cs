using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        Text text;
        Health health;

         void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            text = GetComponent<Text>();
        }

        // TODO change the health display to only change upon a health change
        private void Update()
        {
            text.text = health.GetHP().ToString("F1") + "/" + health.GetMaxHP().ToString("F0");
        }
    }
}