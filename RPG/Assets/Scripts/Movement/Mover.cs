using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent navMeshAgent;
        Health health;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) navMeshAgent.enabled = false;    // disables navmesh when dead

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

        // update the animator depending on speed using blend tree
        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;                                   // getting velocity from the nav mesh agent
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);      // converting the velocity to local space  
            float speed = localVelocity.z;                                              // storing speed value from z axis (only anxis that interests us for animation, moving forward or not)
            GetComponent<Animator>().SetFloat("Forward Speed", speed);                  // setting the blend speed value
        }
    }
}