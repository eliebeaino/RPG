using RPG.Combat;
using RPG.Attributes;
using RPG.Movement;
using UnityEngine;
using RPG.Core;
using GameDevTV.Utils;
using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float shoutDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float aggroCooldownTime = 5f;
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
        float timeSinceLastAggro = Mathf.Infinity;
        int currentWaypointIndex = 0;
        bool hasBeenAggroedRecently = false;
        bool hasBeenAggroedByAlly = false;


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
            if (health.IsDead()) return;  // stops everything when dead

            if (IsAggroed() && fighter.canAttack(player))
            {
                // if aggro the player and player is attackable
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                // if last seen player too long - stop the chase and stand in place doing nothing
                SuspiciousBehavior(); // TODO chance this behavior for enemies to run back to guard position immediately
            }
            else
            {
                // patrol or move back to guarding position
                PatrolBehavior();
            }

            UpdateTimer();
            AggroRestter();
        }

        private void UpdateTimer()
        {
            // update AI counters connstantly - unless dead
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastAggro += Time.deltaTime;            
        }

        private void AggroRestter()
        {
            // checks last aggro time and last time the player was seen - the second check is done to avoid enemies loop feeding aggro to each other
            if (timeSinceLastAggro > aggroCooldownTime)
            {
                hasBeenAggroedRecently = false;
            }
        }

        public void Aggro()
        {
            // UNITY EVENT aggros the player when attacked
            // resets aggro timer
            timeSinceLastAggro = 0;
            hasBeenAggroedRecently = true;
        }

        private void AggroAllies()
        {
            // shouts to nearby ally enemies to attack the player - triggers itself as well
            if (hasBeenAggroedByAlly == true) return;
            timeSinceLastAggro = 0f;
            hasBeenAggroedByAlly = true;
        }

        private void AggroNearbyAllies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.transform.GetComponent<AIController>();
                if (ai == null || ai == this) continue;
                ai.AggroAllies();
            }
        }

        private bool IsAggroed()
        {
            // check if its attacked and aggros the player if attacked
            if (timeSinceLastAggro < aggroCooldownTime) return true;

            // check the distance if within aggro range
            float DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            return DistanceToPlayer < chaseDistance;
        }

        private void AttackBehavior()
        {
            // resets last seen timer - attacks the player - set the movement speed - shouts nearby enemies to attack
            if (!hasBeenAggroedRecently)
            {
                Aggro();
            }
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            mover.SetSpeed(chaseSpeedFactor);

            AggroNearbyAllies();
        }

        private void SuspiciousBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            // patrol along path or go back to guard position
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

            if (Vector3.Distance(transform.position, nextPosition) <waypointTolerance)
            {
                hasBeenAggroedRecently = false;
                hasBeenAggroedByAlly = false;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}