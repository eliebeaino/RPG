using System.Collections;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private float CorpseTimer = 30f;

        private bool isDead = false;

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
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();

            yield return new WaitForSeconds(CorpseTimer);
            Destroy(gameObject);
        }

        // getter for isdead
        public bool IsDead()
        {
            return isDead;
        }    
    }
}