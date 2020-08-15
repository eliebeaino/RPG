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

        private void Update()
        {
            text.text = health.GetPercentageHealth().ToString("F2") + "%";
        }
    }
}