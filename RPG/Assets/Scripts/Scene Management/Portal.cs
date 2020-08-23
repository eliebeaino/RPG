using RPG.Control;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A,B,C,D,E
        }

        [Header("Scene Propreties")]
        [SerializeField] [Range (-1,1)] int sceneToLoad = 1;
        [SerializeField] Transform spawnPoint = null;
        [SerializeField] DestinationIdentifier destination;

        [Header("Fading Propreties")]
        [SerializeField] float fadeOutTime = 3f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float pauseTimeBetweenFades = 1f;

        int currentScene;

        private void Awake()
        {
            currentScene = SceneManager.GetActiveScene().buildIndex;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        // fades out -> pause -> fade in new scene
        IEnumerator Transition()
        {
            // Preserves the object through the scene transition
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();

            // Remove Player Control
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;

            // Fade Out
            yield return fader.FadeOut(fadeOutTime);

            // Saves the game
            savingWrapper.Save();

            // loads next scene and load the game parameters from the save file
            yield return SceneManager.LoadSceneAsync(currentScene + sceneToLoad);

            // Remove "NEW" Player Control after scene loaded
            PlayerController newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;

            savingWrapper.Load();

            // find portal in new scene and warp player to new location
            Portal otherPortal = GetOtherPortal();
            WarpPlayerToSpawnPoint(otherPortal);

            // saves the location of last portal used - checkpoint
            savingWrapper.Save();

            // pause fade screen slightly -> Fade In
            yield return new WaitForSeconds(pauseTimeBetweenFades);
            fader.FadeIn(fadeInTime);

            // Restore Player Control
            newPlayerController.enabled = true;

            // remove this portal from the new loaded scene as we no longer need it
            Destroy(gameObject); 
        }

        private void WarpPlayerToSpawnPoint(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(otherPortal.spawnPoint.position); // use this to teleport player instead of transform to avoid conflicting bug navmesh
            player.transform.rotation = otherPortal.spawnPoint.rotation;
        }

        // scans for other portals in scene and grabs the new one - uses singleton
        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal.destination != destination) continue;
                return portal;           
            }
            return null;
        }
    }
}