using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using UnityEngine;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float CorpseTimer = 30f;
        [SerializeField] float healthPercentOnLevelUP = 50f;

        float healthPoints = -1f;

        private bool isDead = false;
        Animator animator;
        ActionScheduler actionScheduler;
        BaseStats baseStats;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
        }

        // Load base health from progression depending on level
        // if the character is dead, turns this off(for reloading scenes with dead enemies) -- TODO change this later on for respawns
        private void Start()
        {
            if (isDead) this.gameObject.SetActive(false);
            // check if health is no longer -1 meaning it was already restored from save file and no longer need to get the base stat
            if (healthPoints < 0)
            {
                healthPoints = baseStats.GetStat(Stat.Health);
            }
            baseStats.onLevelUp += IncreaseHPOnLevelUP;
        }

        // HEALTH CHANGE
        public void TakeDamage(GameObject instigator, float damage)
        {
            print(gameObject.name + " took " + damage + " damage from " + instigator);
            healthPoints = Mathf.Max(healthPoints - damage, 0);  // limit minimum health to 0
            if (healthPoints == 0 && !isDead)
            {
                AwardExperience(instigator);
                StartCoroutine(Die());
            }
        }

        IEnumerator Die()
        {
            isDead = true;
            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();

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
            float newHP = healthPoints + (newMaxHp * healthPercentOnLevelUP / 100);
            healthPoints = Mathf.Clamp(newHP,0, newMaxHp);
        }

        public float GetMaxHP()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHP()
        {
            return healthPoints;
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
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if (healthPoints == 0 && !isDead) this.gameObject.SetActive(false);
        }
        #endregion
    }
}
