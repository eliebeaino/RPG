using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover mover;
        Health health;

        private void Start()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if (health.IsDead()) return; // stops everything when dead

            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
            print("nothing else to do");
        }

        // check if targetting a - LIVING - enemy and attack it
        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButtonDown(0))
                {  
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                return true;
            }
            return false;
        }

        // gets player input and moves the player to the cursor when left click 
        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        // get the ray from camera to mouse point
        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
