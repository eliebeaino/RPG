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

        // TODO change the health display to only change upon a health change
        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                text.text = "N/A";
            }
            else
            {
                text.text = fighter.GetTarget().GetHP() + "/" + fighter.GetTarget().GetMaxHP().ToString("F0");
            }
        }
    }
}