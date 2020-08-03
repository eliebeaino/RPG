using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) return; // allows for only one to spawn by checking the static bool
            hasSpawned = true;

            SpawnPersistentObjects();
        }

        // spawn root object of persistant objects and add don't destory on load
        private void SpawnPersistentObjects()
        {
            GameObject gameObject = Instantiate(persistentObjectPrefab);
            DontDestroyOnLoad(gameObject);
        }
    }
}