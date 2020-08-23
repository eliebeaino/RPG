using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float physicalDamage = 10f;
        [SerializeField] float PercentageBonus = 0f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public Weapon SpawnWpn(Transform rightHand,Transform leftHand, Animator animator)
        {
            Weapon weapon = null;
            DestroyOldWeapon(rightHand, leftHand);
            if (equippedPrefab != null)
            {
                Transform handTransform = ChooseCorrectHand(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
            return weapon;
        }

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

        Transform ChooseCorrectHand(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectielInstrance = Instantiate(projectile, ChooseCorrectHand(rightHand, leftHand).position,Quaternion.identity);
            projectielInstrance.SetTargetAndDamage(target, instigator , calculatedDamage);
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public float GetWeaponPhysicalDamage()
        {
            return physicalDamage;
        }

        public float GetWeaponPercentModifier()
        {
            return PercentageBonus;
        }
    }
}