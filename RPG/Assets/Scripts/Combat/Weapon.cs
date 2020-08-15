using RPG.Resources;
using System;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;

        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float weaponDamage = 10f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        // removes previous weapon if any -- spawn new weapon in the correct hand and sets its name to weapon -- changes animator component accordingly
        public void Spawn(Transform rightHand,Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                GameObject weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            // sets new override - or resets controller to default (TODO = i dont understand this part)
            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        // destroy previously equipped weapon if exists
        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING"; // rename it right before destroying to avoid a bug
            Destroy(oldWeapon.gameObject);
        }

        // grabs which hand the weapon uses and set the trasform (set up from inspector)
        Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        // getter to check if weapon uses projectile system
        public bool HasProjectile()
        {
            return projectile != null;
        }

        // fire the projectile - if any exist - from corresponding hand transform unto the target
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator)
        {
            Projectile projectielInstrance = Instantiate(projectile, GetTransform(rightHand, leftHand).position,Quaternion.identity);
            projectielInstrance.SetTargetAndDamage(target, instigator ,weaponDamage);
        }

        // getter for the weaopon range
        public float GetWeaponRange()
        {
            return weaponRange;
        }

        // getter for the weaopon damage
        public float GetWeaponDamage()
        {
            return weaponDamage;
        }
    }
}