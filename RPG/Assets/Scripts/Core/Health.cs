using RPG.Saving;
using System.Collections;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float CorpseTimer = 30f;

        private bool isDead = false;
        Animator animator;
        ActionScheduler actionScheduler;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        // called from fighter
        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage,0);  // limit minimum health to 0
            if (health == 0 && !isDead) StartCoroutine(Die());
        }

        // set the death animation - cancel the current action in place from the scheduler - destory the enemy after corpse timer delay
        IEnumerator Die()
        {
            isDead = true;
            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();

            yield return new WaitForSeconds(CorpseTimer);
            Destroy(gameObject);
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
            return health;
        }

        // loads health level of this character - check if dead to keep corpse or not dead to reset animator and bool value
        public void RestoreState(object state)
        {
            health = (float)state;
            if (health == 0 && !isDead) StartCoroutine(Die());
        }
    }
}