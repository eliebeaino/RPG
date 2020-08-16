using UnityEngine;
using UnityEngine.UI;
namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter playerFighter;
        Text text;

        void Awake()
        {
            playerFighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        // TODO change the health display to only change upon a health change
        private void Update()
        {
            if (playerFighter.GetTarget() == null)
            {
                text.text = "N/A";
            }
            else
            {
                text.text = playerFighter.GetTarget().GetHP() + "/" + playerFighter.GetTarget().GetMaxHP().ToString("F0");
            }
        }
    }
}