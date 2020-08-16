﻿using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 8f;
        [SerializeField] bool homing = false;
        [SerializeField] float expirationTime = 10f;

        Health target;
        GameObject instigator;
        float damage = 0;
        
        private void Start()
        {
            transform.LookAt(GetAimLocation());
            Destroy(gameObject, expirationTime);
        }

        private void Update()
        {
            // moves - rotates the projectiles towards target
            if (homing && !target.IsDead()) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTargetAndDamage(Health getTarget,GameObject instigator, float getDamage)
        {
            this.target = getTarget;
            this.damage = getDamage;
            this.instigator = instigator;
        }    

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            return target.transform.position + targetCapsule.center;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if (target.IsDead()) return;
            target.TakeDamage(instigator, damage);
            Destroy(gameObject);
        }
    }
}