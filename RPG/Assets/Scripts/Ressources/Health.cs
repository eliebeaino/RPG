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

        float healthPoints = -1f;

        private bool isDead = false;
        Animator animator;
        ActionScheduler actionScheduler;
        BaseStats baseStats;

        //cache componenets
        private void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
            baseStats = GetComponent<BaseStats>();
        }

        // Load base health from progression depending on level
        // if the character is dead, turns this off(for reloading scenes with dead enemies) -- TODO change this slightly later on for respawns
        private void Start()
        {
            if (isDead) this.gameObject.SetActive(false);
            // check if health is no longer -1 meaning it was already restored from save file and no longer need to get the base stat
            if (healthPoints < 0)
            {
                healthPoints = baseStats.GetStat(Stat.Health);
            }
        }

        // called from fighter
        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage, 0);  // limit minimum health to 0
            if (healthPoints == 0 && !isDead)
            {
                AwardExperience(instigator);
                StartCoroutine(Die());
            }
        }

        // set the death animation - cancel the current action in place from the scheduler - turn off the enemy after corpse timer delay
        // rewards experience
        IEnumerator Die()
        {
            isDead = true;
            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();

            yield return new WaitForSeconds(CorpseTimer);
            this.gameObject.SetActive(false); ;
        }

        // Reward the player with experince by grabbing experience levels from basestats of enemy killed
        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;
            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        // getter for health levels - used for health displays
        public float GetPercentageHealth()
        {
            return healthPoints / baseStats.GetStat(Stat.Health) * 100;
        }

        public float GetHealth()
        {
            return healthPoints;
        }

        // getter for isdead
        public bool IsDead()
        {
            return isDead;
        }

        // resets the character animator, reset the isDead value
        void Resurrect()
        {
            isDead = false;
            animator.enabled = false;
            animator.enabled = true;
        }

        #region Save HP
        // saves health levels of this character
        public object CaptureState()
        {
            return healthPoints;
        }

        // loads health level of this character - check if dead to keep corpse or not dead to reset animator and bool value
        public void RestoreState(object state)
        {
            healthPoints = (float)state;
            if (healthPoints == 0 && !isDead) StartCoroutine(Die());
        }
        #endregion
    }
}