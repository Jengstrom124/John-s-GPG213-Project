using UnityEngine;

namespace Kevin
{
    public class KEat : MonoBehaviour, IPredator
    {
        public GameObject sealPrefab;
        private FishContainer fishContainer;
        
        public float scaleAmount = 0.1f;

        public void Start()
        {
            fishContainer = GetComponent<FishContainer>(); 
        }

        /*public void Update()
        {
            scaleAmount = fishContainer.totalFoodAmount / 10f;
        }*/
        void OnTriggerEnter(Collider other)
        {
            
            if (!other.isTrigger)
            {
                IEdible edible = other.GetComponent<IEdible>();
            
                if (edible != null)
                {
                    sealPrefab.transform.localScale += Vector3.one * scaleAmount * (1f + fishContainer.totalFoodAmount/100f); 
                }
            
                FishBase fish = other.GetComponent<FishBase>();
                if (fish != null)
                {
                    fishContainer.AddToStomach(fish);
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

