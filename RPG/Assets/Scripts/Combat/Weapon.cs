using RPG.Core;
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

        public void Spawn(Transform rightHand,Transform leftHand, Animator animator)
        {
            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                Instantiate(equippedPrefab, handTransform);
            }
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
        }

        // grabs which hand the weapon uses and set the trasform
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
        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
        {
            Projectile projectielInstrance = Instantiate(projectile, GetTransform(rightHand, leftHand).position,Quaternion.identity);
            projectielInstrance.SetTargetAndDamage(target,weaponDamage);
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