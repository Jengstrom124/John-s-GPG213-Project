using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gerallt
{
    public class UIClient : MonoBehaviour
    {
        public TextMeshProUGUI UITextMeshPro;
        public void UpdateUI(PlayerController playerController)
        {
            UITextMeshPro.SetText(playerController.playerName);
        }
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
