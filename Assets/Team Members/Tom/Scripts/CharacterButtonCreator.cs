using System;
using System.Collections;
using System.Collections.Generic;
using Gerallt;
using Tanks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterButtonCreator : MonoBehaviour
{
    public EventSystem eventSystem;
    public UILobby uiLobby;
    public GameObject characterButtonPrefab;

    public GameObject characterListGO;

    public void Start()
    {
        GameObject[] characters = GameManager.Instance.characterPrefabs;

        // Dynamically creates buttons for each character and assigns index reference for spawning
        for (int i = 0; i < characters.Length; i++)
        {
            GameObject newButton = Instantiate(characterButtonPrefab, transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = characters[i].name;

            newButton.transform.SetParent(characterListGO.transform);
            // newButton.GetComponent<Button>().onClick.AddListener();

            // CharacterSelectButton button = newButton.GetComponent<CharacterSelectButton>();
            newButton.GetComponent<CharacterSelectButton>().characterIndex = i;
            newButton.GetComponent<CharacterSelectButton>().uiLobby = uiLobby;
            // button.eventSystem = eventSystem;
        }
    }
}
