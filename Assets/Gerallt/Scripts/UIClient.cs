using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Gerallt
{
    public class UIClient : MonoBehaviour
    {
        
        
        // Start is called before the first frame update
        void Start()
        {
            //transform.parent = FindObjectOfType<UILobby>().JoinedClients.transform;
            transform.SetParent(FindObjectOfType<UILobby>().JoinedClients.transform);
            
            //var tmp = GetComponent<TextMeshPro>();
            //tmp.SetText(GetComponent<PlayerController>().playerName);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
