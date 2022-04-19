using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Kevin
{
    public class Flock : SerializedMonoBehaviour, IStateBase
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void Enter()
        {
            Debug.Log("Flocking");
        }

        public void Execute()
        {
            
        }

        public void Exit()
        {
            Debug.Log("No Longer Flocking");
        }
    }
}

