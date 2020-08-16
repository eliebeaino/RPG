using RPG.Combat;
using RPG.Resources;
using RPG.Movement;
using UnityEngine;
using RPG.Core;
using GameDevTV.Utils;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float chaseSpeedFactor = 0.5f;

        [Header("Patrol")]
        [SerializeField] PatrolPath patrolPath = null;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] float patrolSpeedFactor = 0.2f;

        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead() || fighter.IsAttacking()) return;  // stops everything when dead

            if (InAttackRange() && fighter.HasValidTarget(player))
            {
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspiciousBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimer();
        }

        private void UpdateTimer()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            mover.SetSpeed(chaseSpeedFactor);
        }

        private void SuspiciousBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        // patrol along path or go back to guard position
        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition.value;
            if (patrolPath != null)
            {
                if (IsAtWaypoint())
                {
                    CycleNextWaypoint();
                    timeSinceArrivedAtWaypoint = 0;        
                }
                nextPosition = GetCurrentWaypoint();
                mover.SetSpeed(patrolSpeedFactor);
            }
            if (timeSinceArrivedAtWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition);
            }
        }

        private bool IsAtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleNextWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool InAttackRange()
        {
            float DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return DistanceToPlayer < chaseDistance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}