using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;

        NavMeshAgent navMeshAgent;
        Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();            // enables navmesh on start or disables navmesh when dead 

            UpdateAnimator();
        }

        // start the move action and cancel previous action (if any)
        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }

        // move the object towards the destination point
        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }

        // stops the movement of this character
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public void SetSpeed(float speedFactor)
        {
            navMeshAgent.speed = maxSpeed * speedFactor;
        }

        // update the animator depending on speed using blend tree
        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;                                   // getting velocity from the nav mesh agent
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);      // converting the velocity to local space  
            float speed = localVelocity.z;                                              // storing speed value from z axis (only anxis that interests us for animation, moving forward or not)
            GetComponent<Animator>().SetFloat("Forward Speed", speed);                  // setting the blend speed value
        }

        // saves the position of the character
        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }
        // loads the position of the character
        public void RestoreState(object state)
        {
            SerializableVector3 positionState = (SerializableVector3)state;
            navMeshAgent.Warp(positionState.ToVector());
        }
    }
}