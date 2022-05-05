using Unity.Netcode;
using UnityEngine;

namespace Kevin
{
    public class KEat : NetworkBehaviour, IPredator
    {
        public GameObject sealPrefab;
        private FishContainer fishContainer;
        
        public float scaleAmount = 0.1f;

        public void Start()
        {
            fishContainer = GetComponent<FishContainer>(); 
        }
        void OnTriggerEnter(Collider other)
        {
            
            if (!other.isTrigger && IsServer)
            {
                IEdible edible = other.GetComponent<IEdible>();
                FishBase fish = other.GetComponent<FishBase>();
                
                if (edible != null)
                {
                    fishContainer.AddToStomach(fish);
                }
                
                if (fish != null)
                {
                    sealPrefab.GetComponent<Seal>().foodLevel = fishContainer.totalFoodAmount;
                }
            }
        }

      

        public Vector3 GetBumPosition()
        {
            throw new System.NotImplementedException();
        }
    }
}

