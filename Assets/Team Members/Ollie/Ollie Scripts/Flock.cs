using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ollie
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
            print("go flock yourself");
        }

        public void Execute()
        {
            
        }

        public void Exit()
        {
            print("finished flocking");
        }
    }
}
