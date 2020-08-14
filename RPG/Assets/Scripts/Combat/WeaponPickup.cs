using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon weapon;
        [SerializeField] float respawnTime = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds());
            }
        }

        // respawn timer for pick up -- sets timer to hide all the pickup componenets necessary for it to function 
        IEnumerator HideForSeconds()
        {
            ShowPickup(false);

            yield return new WaitForSeconds(respawnTime);

            ShowPickup(true);
        }

        // iterate between enabling and disabling pickup - collider and child componenets
        private void ShowPickup(bool shouldShow)
        {
            gameObject.GetComponent<Collider>().enabled = shouldShow;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }
    }
}