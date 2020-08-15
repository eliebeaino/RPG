using UnityEngine;
using UnityEngine.UI;
namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        Text text;

        void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                text.text = "N/A";
            }
            else
            {
                text.text = fighter.GetTarget().GetHealth() + " // " + fighter.GetTarget().GetPercentageHealth().ToString("F2") + "%";
            }
        }
    }
}