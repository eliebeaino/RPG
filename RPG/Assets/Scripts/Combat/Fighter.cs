using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage = 10f;

        float timeSinceLastAttack = 0;
        Transform target;
        Mover mover;

        private void Start()
        {
            mover = GetComponent<Mover>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (!GetIsInRange())
            {
                mover.MoveTo(target.position);
            }
            else
            {
                mover.Cancel();
                AttackBehavior();
            }
        }

        // set animator to attack
        private void AttackBehavior()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                GetComponent<Animator>().SetTrigger("attack");      // this will trigger hit event
                timeSinceLastAttack = 0;
            }
        }

        // check the range from player to enemy target
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        // set the target - from the playercontroller upon input 
        // and cancel previous action (if any)
        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;       
        }

        // removes the enemy target
        public void Cancel()
        {
            target = null;
        }

        // animation event
        private void Hit()
        {
            target.GetComponent<Health>().TakeDamage(weaponDamage);
        }
    }
}