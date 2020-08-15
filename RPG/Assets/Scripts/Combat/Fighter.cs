using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;
using RPG.Resources;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;

        Weapon currentWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;  // allows attack directly upon first contact
        bool isAttacking = false;

        Health target;
        Mover mover;
        Animator animator;
        
        // stores component on awake
        private void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
        }

        // equips default weapon
        private void Start()
        {
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
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

        // look at the target - set animator to attack - reset the "stopAttack" trigger - and add delay between attacks
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

        // check the range from player to enemy target
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeapon.GetWeaponRange();
        }

        // check if target isdead to allow attacking or not
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        // set the target - from the playercontroller upon input - cancel previous action (if any)
        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();       
        }

        // Equip the weapon (default weapon at the start of load) or pick up weapon saved to current weapon
        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon = weapon;
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        // getter for target - used to check players current target
        public Health GetTarget()
        {
            return target;
        }

        // removes the enemy target & stop the attack animation - reset "attack" trigger - cancel the mover action if any
        public void Cancel()
        {
            target = null;
            mover.Cancel();
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        // animator events
        #region
        // animation event for melee to deal damage - for ranged attacks to create a projectile from the weapons location and launch towards target
        private void Hit()
        {
            if (target == null) return;
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject);
            }
            else
            {
                target.TakeDamage(gameObject, currentWeapon.GetWeaponDamage());
            }
        }

        // stops the characters movement at the start of attack animation
        private void StopMovement()
        {
            mover.SetSpeed(0);
            isAttacking = true;
        }

        // resumes the characters movement before the end of attack animation
        private void CanMoveAgain()
        {
            mover.SetSpeed(1);
            isAttacking = false;
        }

        // referenced in controller scripts to stop all other actions till attack is finished
        public bool IsAttacking()
        {
            return isAttacking;
        }
        #endregion

        // saving system
        #region
        // saves the weapon name
        public object CaptureState()
        {
            return currentWeapon.name;
        }
        

        // load the weapon used from save - equips it by loading it from the ressource folder from the string reference
        public void RestoreState(object state)
          {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
        #endregion
    }
}