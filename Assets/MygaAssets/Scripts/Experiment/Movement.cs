using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MygaClient
{
    [RequireComponent(typeof(MygaTransform))]
    public class Movement : MonoBehaviour
    {
        private MygaTransform serverTransform;

        void Start()
        {
            serverTransform = GetComponent<MygaTransform>();
        }

        void Update()
        {
            float left = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
            transform.position += new Vector3(left * Time.deltaTime, 0, 0);
        }  
    }

}
