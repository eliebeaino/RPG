using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffects : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestory = null;

        private void Update()
        {
            if (!GetComponent<ParticleSystem>().IsAlive())
            {
                if (targetToDestory!= null)
                {
                    Destroy(targetToDestory);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}