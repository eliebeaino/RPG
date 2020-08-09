using RPG.Core;
using System;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 8f;
        Health target;
        float damage = 0;

        private void Update()
        {
            // moves - rotates the projectiles towards target
            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        // sets the target and the damage of the projectile
        public void SetTargetAndDamage(Health getTarget, float getDamage)
        {
            this.target = getTarget;
            this.damage = getDamage;
        }    

        // grabs the location of the center of target's collider
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            return target.transform.position + targetCapsule.center;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}