using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0f; // TEMP health pick up
        [SerializeField] float respawnTime = 10f;
        [SerializeField] float itemPickupRange = 3f;

        private void OnTriggerEnter(Collider other)
        {
            // TODO - we don't want all pickups to be on collision in the future
            if (other.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject subject)
        {
            // equip the item on pickup
            if (weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }

            // TEMP health pick up
            if (healthToRestore >0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            StartCoroutine(RespawnPickup());
        }

        IEnumerator RespawnPickup()
        {
            // disables pickup for a certain time than re-enables it to give the idea of respawn
            ShowOrHidePickup(false);

            yield return new WaitForSeconds(respawnTime);

            ShowOrHidePickup(true);
        }

        private void ShowOrHidePickup(bool shouldShow)
        {
            // show or hide the pickup depending on given bool value
            gameObject.GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            // checks for input - if within range grabs the item - if not run towards it
            if (Input.GetMouseButtonDown(0))
            {
                float distance = Vector3.Distance(callingController.gameObject.transform.position, transform.position);
                if (distance <= itemPickupRange)
                {
                    Pickup(callingController.gameObject);
                }
                else
                {
                    // TODO missing code to grab the pick up once reached destination
                    Vector3 pickupOffsetPosition = SetPickupOffsetDestination(callingController.transform);
                    callingController.GetComponent<Mover>().StartMoveAction(pickupOffsetPosition);
                }
            }
            return true;
        }

        Vector3 SetPickupOffsetDestination(Transform player)
        {
            // sets an offset for the player to run towards to pickup the item
            Vector3 distanceVector = player.position - transform.position;
            Vector3 distanceVectorNormalized = distanceVector.normalized;
            return transform.position + (distanceVectorNormalized * itemPickupRange);
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }
}