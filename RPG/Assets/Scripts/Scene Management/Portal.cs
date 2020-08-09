﻿using System.Collections;
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
        [SerializeField] Transform spawnPoint;
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

        // checks if collision to player to start the loading functionality
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        // loads the next scene in the correct spawn point --- fades out -> pause -> fade in new scene
        IEnumerator Transition()
        {
            // Preserves the object through the scene transition
            DontDestroyOnLoad(gameObject);

            // Fade Out
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            // Saves the game
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            // loads next scene and load the game parameters from the save file
            yield return SceneManager.LoadSceneAsync(currentScene + sceneToLoad);
            savingWrapper.Load();

            // find portal in new scene and warp player to new location
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            // saves the location of last portal used - checkpoint
            savingWrapper.Save();

            // pause fade screen slightly -> Fade In
            yield return new WaitForSeconds(pauseTimeBetweenFades);
            yield return fader.FadeIn(fadeInTime);

            // remove this portal from the new loaded scene as we no longer need it
            Destroy(gameObject); 
        }

        // spawns the player in the spawn point of the new scene spawn point location - preserve ( position & rotation )
        private void UpdatePlayer(Portal otherPortal)
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