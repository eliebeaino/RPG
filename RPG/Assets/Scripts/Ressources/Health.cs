using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using System.Collections;
using UnityEngine;

namespace RPG.Resources
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float healthPoints = 100f;
        [SerializeField] private float CorpseTimer = 30f;

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

        private void Start()
        {
            healthPoints = baseStats.GetHealth(); // TODO fix loading health not resetting
            if (isDead) this.gameObject.SetActive(false);
        }

        // called from fighter
        public void TakeDamage(float damage)
        {
            healthPoints = Mathf.Max(healthPoints - damage,0);  // limit minimum health to 0
            if (healthPoints == 0 && !isDead) StartCoroutine(Die());
        }

        // set the death animation - cancel the current action in place from the scheduler - destory the enemy after corpse timer delay
        IEnumerator Die()
        {
            isDead = true;
            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();

            yield return new WaitForSeconds(CorpseTimer);
            this.gameObject.SetActive(false); ;
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
    }
}