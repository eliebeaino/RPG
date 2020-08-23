using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        float timeSinceLastAttack = Mathf.Infinity;  // allows attack directly upon first contact
        bool isAttacking = false;

        Health target;
        Mover mover;
        Animator animator;
        BaseStats baseStats;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;
        
        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }
        
        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                mover.MoveTo(target.transform.position);
            }
            else
            {
                mover.Cancel();
                AttackBehavior();
            }
        }

        private void AttackBehavior()
        {  
            transform.LookAt(target.transform);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                animator.ResetTrigger("stopAttack");
                animator.SetTrigger("attack");              // this will trigger hit event
                timeSinceLastAttack = 0;
            }
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();
        }

        public bool HasValidTarget(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();       
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.SpawnWpn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public void Cancel()
        {
            target = null;
            mover.Cancel();
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.PhysicalDamage)
            {
                yield return currentWeaponConfig.GetWeaponPhysicalDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.PhysicalDamage)
            {
                yield return currentWeaponConfig.GetWeaponPercentModifier();
            }
        }

        #region Animation Triggers Or Related
        private void Hit()
        {
            if (target == null) return;

            float damage = baseStats.GetStat(Stat.PhysicalDamage);

            // triggers onhit event - calls sound
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        private void StopMovement()
        {
            mover.SetSpeed(0);
            isAttacking = true;
        }

        private void CanMoveAgain()
        {
            mover.SetSpeed(1);
            isAttacking = false;
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }
        #endregion

        #region Save Weapon
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }
        
        public void RestoreState(object state)
          {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
        #endregion
    }
}