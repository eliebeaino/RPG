using RPG.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        NavMeshAgent NavMeshAgent;

        private void Start()
        {
            NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<Fighter>().Cancel();
            MoveTo(destination);
        }    

        // move the object towards the destination point
        public void MoveTo(Vector3 destination)
        {
            NavMeshAgent.destination = destination;
            NavMeshAgent.isStopped = false;
        }

        public void Stop()
        {
            NavMeshAgent.isStopped = true;
        }

        // update the animator depending on speed using blend tree
        private void UpdateAnimator()
        {
            Vector3 velocity = NavMeshAgent.velocity;                                   // getting velocity from the nav mesh agent
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);      // converting the velocity to local space  
            float speed = localVelocity.z;                                              // storing speed value from z axis (only anxis that interests us for animation, moving forward or not)
            GetComponent<Animator>().SetFloat("Forward Speed", speed);                  // setting the blend speed value
        }
    }
}