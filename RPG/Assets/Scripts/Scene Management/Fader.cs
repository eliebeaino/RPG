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

        // setup as coroutine and not as ienumerator so we can call them without yield returning them in portal if we want too
        // that way they can do the coroutine while doing other stuff too instead of waiting for it to finish

        public Coroutine FadeOut(float time)
        {
            // fade to max alpha
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            // fade to 0 alpha
            return Fade(0, time);
        }

        public Coroutine Fade(float target, float time)
        {
            // sets an active coroutine and stops the old one so fade in/out don't overlap
            if (currentActiveFade != null)
            {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return currentActiveFade;
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