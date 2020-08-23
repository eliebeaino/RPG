﻿using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Image image = null;
        [SerializeField] Canvas canvas = null;
        Health health = null;

        private void Awake()
        {
            health = GetComponentInParent<Health>();   
        }

        public void UpdateHealthBarDisplay()
        {
            // if full hp don't display hp bar
            if (Mathf.Approximately (health.GetHP(), health.GetMaxHP()))
            {
                canvas.enabled = false;
                return;
            }
            // if dead don't display hp bar
            if (health.IsDead())
            {
                canvas.enabled = false;
                return;
            }

            // updates hp bar levels
            canvas.enabled = true;
            image.fillAmount = health.GetHP() / health.GetMaxHP();
        }
    }
}