using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text text;

        public void DestroyText()
        {
            Destroy(gameObject);
        }

        public void DisplayText(float damageAmount)
        {
            text.text = damageAmount.ToString();
        }
    }
}