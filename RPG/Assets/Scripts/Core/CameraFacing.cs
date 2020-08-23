using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class CameraFacing : MonoBehaviour
    {
        private void LateUpdate()
        {
            // late update to avoid visual bug, gets called last
            transform.forward = Camera.main.transform.forward;
        }
    }
}
