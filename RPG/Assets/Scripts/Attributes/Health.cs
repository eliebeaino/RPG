﻿using GameDevTV.Utils;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float CorpseTimer = 30f;
        [SerializeField] float healthPercentOnLevelUP = 50f;

        // this is done to show within the inspector the event system and allow a float to be passed
        [SerializeField] OnDie onDie = null;
        [System.Serializable]
        public class OnDie : UnityEvent<float> { }

        // this is done to show within the inspector the event system and allow a float to be passed
        [SerializeField] TakeDamageEvent takeDamage = null;
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float> { }

        LazyValue<float> healthPoints;

        private bool isDead = false;
        Animator animator;
        ActionScheduler actionScheduler;
        BaseStats baseStats;
        Collider colliderr;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
            colliderr = GetComponent<Collider>();

            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void OnEnable()
        {
            baseStats.onLevelUp += IncreaseHPOnLevelUP;
        }

        private void OnDisable()
        {
            baseStats.onLevelUp -= IncreaseHPOnLevelUP;
        }

        private void Start()
        {
            // if the character is dead, turns this off(for reloading scenes with dead enemies) -- TODO change this later on for respawns
            if (isDead) this.gameObject.SetActive(false);
            healthPoints.ForceInit();
        }

        // HEALTH CHANGE
        public void TakeDamage(GameObject instigator, float damage)
        {
            // calculate new health value
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);  // limit minimum health to 0

            // Trigger Death Events on 0 hp - awards XP
            if (healthPoints.value == 0 && !isDead)
            {
                AwardExperience(instigator);
                onDie.Invoke(damage); // TODO calculate proper final damage to display
                StartCoroutine(Die());
            }
            else
            {
                // Trigger Take Damage Event
                takeDamage.Invoke(damage);
            }
        }

        IEnumerator Die()
        {
            isDead = true;
            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();
            colliderr.enabled = false;

            yield return new WaitForSeconds(CorpseTimer);
            this.gameObject.SetActive(false); ;
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        // HEALTH CHANGE - on event
        private void IncreaseHPOnLevelUP()
        {
            float newMaxHp = baseStats.GetStat(Stat.Health);
            float newHP = healthPoints.value + (newMaxHp * healthPercentOnLevelUP / 100);
            healthPoints.value = Mathf.Clamp(newHP,0, newMaxHp);
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min((healthPoints.value + healthToRestore), GetMaxHP());
        }

        public float GetMaxHP()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHP()
        {
            return healthPoints.value;
        }

        public bool IsDead()
        {
            return isDead;
        }

        void Resurrect()
        {
            isDead = false;
            animator.enabled = false;
            animator.enabled = true;
        }

        #region Save HP
        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            if (healthPoints.value == 0 && !isDead) this.gameObject.SetActive(false);
        }
        #endregion
    }
}
