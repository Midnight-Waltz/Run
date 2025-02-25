using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Trap
{
    public class Rotator : MonoBehaviour
    {
        public Vector3 rotationSpeed;

        void FixedUpdate()
        {
            transform.localEulerAngles += rotationSpeed;
        }
    }
}

