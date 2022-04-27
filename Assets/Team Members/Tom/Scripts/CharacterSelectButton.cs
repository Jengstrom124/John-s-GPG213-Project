using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectButton : MonoBehaviour
{
    public GameObject character;
    public EventSystem eventSystem;
    
    public void SetCharacter()
    {
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().selectedCharacter =
            character;
        eventSystem.SetSelectedGameObject(gameObject);
    }
}
