using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup = null;
        Coroutine currentActiveFade = null;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            // instant max alpha to block the screen
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            // fade to max alpha
            return Fade(1, time);
        }

        public IEnumerator FadeIn(float time)
        {
            // fade to 0 alpha
            return Fade(0, time);
        }

        public IEnumerator Fade(float target, float time)
        {
            // sets an active coroutine and stops the old one so fade in/out don't overlap
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            yield return currentActiveFade;
        }    

        private IEnumerator FadeRoutine(float target, float time)
        {
            // changing alpha towards the target value by either increasing or decreasing the value
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}