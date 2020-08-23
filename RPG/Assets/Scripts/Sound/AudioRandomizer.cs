using UnityEngine;

namespace RPG.Sound
{
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] audioClips = null;

        [Header ("Clip Random Attributes")]
        [SerializeField] float pitchMin = 0.8f;
        [SerializeField] float pitchMax = 1.2f;
        [SerializeField] float volumeMin = 0.8f;
        [SerializeField] float volumeMax = 1f;

        AudioSource audioSource;
        int clipIndex = 0;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            // set initial clip
            audioSource.clip = audioClips[clipIndex];
        }
        public void PlayRoundRobin()
        {
            RandomizePitchAndVolume();
            // play the next variation in whatever order established in the array, starting over when it reaches the end of the array.
            if (clipIndex < audioClips.Length)
            {
                audioSource.PlayOneShot(audioClips[clipIndex]);
                clipIndex++;
            }
            else
            {
                clipIndex = 0;
                audioSource.PlayOneShot(audioClips[clipIndex]);
                clipIndex++;
            }
        }

        public void PlayRandom()
        {
            // plays random sound in no order - without sequence repitition
            clipIndex = RepeatCheck(clipIndex, audioClips.Length); ;
            audioSource.PlayOneShot(audioClips[clipIndex]);
        }

        private int RepeatCheck(int previousIndex, int range)
        {
            // checks for non-repetitive sound
            int index = Random.Range(0, range);

            while (index == previousIndex)
            {
                index = Random.Range(0, range);
            }
            return index;
        }
        
        private void RandomizePitchAndVolume()
        {
            // Random Pitch And Volume for each clip played
            audioSource.pitch = Random.Range(pitchMin, pitchMax);
            audioSource.volume = Random.Range(volumeMin, volumeMax);
        }
    }
}