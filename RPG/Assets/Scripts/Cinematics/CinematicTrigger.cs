using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool alreadyTriggered = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.tag == "Player")
            {
                GetComponent<PlayableDirector>().Play();
                alreadyTriggered = true;
            }
        }

        // save if cinematic is triggered
        public object CaptureState()
        {
            return alreadyTriggered;
        }

        // load bool state of trigger
        public void RestoreState(object state)
        {
            alreadyTriggered = (bool)state;
        }
    }
}