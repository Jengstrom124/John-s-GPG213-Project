using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
	public List<ulong> TestClientList = new List<ulong>();

    // Start is called before the first frame update
    void Start()
    {
	    
    }

    // Update is called once per frame
    void Update()
    {
	    //TestClientList = FindObjectOfType<ServerManager>().ClientList;
    }
}
