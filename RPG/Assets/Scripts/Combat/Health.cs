using UnityEngine;

namespace RPG.Combat
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health = 100f;

        // called from fighter
        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage,0);  // limit minimum health to 0
            if (health <= 0) Die();
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }
}