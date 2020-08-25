using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 5.66f;
        [SerializeField] float maxNavPathLenght = 40f;

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

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.isStopped = false;
        }
        
        public bool canMoveTo(Vector3 destination)
        {
            // Stores the path, checks if exists || full path to access || path is not long
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLenght(path) > maxNavPathLenght) return false;

            return true;
        }

        private float GetPathLenght(NavMeshPath path)
        {
            // calculates the total lenght of the path for the navmesh to travel - needed to check if the path is too long to avoid moving
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }


        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        // TODO CHANGE TO GET FROM PROGRESSION
        public void SetSpeed(float speedFactor)
        {
            navMeshAgent.speed = maxSpeed * speedFactor;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = navMeshAgent.velocity;                                   // getting velocity from the nav mesh agent
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);      // converting the velocity to local space  
            float speed = localVelocity.z;                                              // storing speed value from z axis (only anxis that interests us for animation, moving forward or not)
            GetComponent<Animator>().SetFloat("Forward Speed", speed);                  // setting the blend speed value
        }

        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }
        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            navMeshAgent.Warp(data.position.ToVector());
            transform.eulerAngles = data.rotation.ToVector();
        }
    }
}