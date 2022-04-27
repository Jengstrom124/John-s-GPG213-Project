using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterButtonCreator : MonoBehaviour
{
    public EventSystem eventSystem;
    public UILobby uiLobby;
    public GameObject characterButtonPrefab;

    public void Start()
    {
        GameObject[] characters = GameManager.Instance.characterPrefabs;

        // Dynamically creates buttons for each character and assigns index reference for spawning
        for (int i = 0; i < characters.Length; i++)
        {
            GameObject newButton = Instantiate(characterButtonPrefab, transform);
            CharacterSelectButton button = newButton.GetComponent<CharacterSelectButton>();
            button.characterIndex = i;
            button.uiLobby = uiLobby;
            button.eventSystem = eventSystem;
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = characters[i].name;
        }
    }
}
