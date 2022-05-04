using UnityEngine;

namespace Kevin
{
    public class KEat : MonoBehaviour, IPredator
    {
        public GameObject sealPrefab;
        public float foodLevel = 10f;
        public float scaleAmount = 0.5f;
        void OnTriggerEnter(Collider other)
        {
            IEdible edible = other.GetComponent<IEdible>();
            FishBase fish = other.GetComponent<FishBase>();
            if (edible != null)
            {
                sealPrefab.transform.localScale += Vector3.one * scaleAmount; 
            }

            if (fish != null)
            {
                //fishcontainer
            }
        }

      

        public Vector3 GetBumPosition()
        {
            throw new System.NotImplementedException();
        }
    }
}

