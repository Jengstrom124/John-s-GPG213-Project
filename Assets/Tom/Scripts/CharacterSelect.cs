using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : NetworkBehaviour
{
    [Serializable]
    public class CharacterButton
    {
        public Button button;
        public CharacterBase character;
        public int characterIndex;
    }

    public CharacterButton[] buttons;

    public Dictionary<int, CharacterBase> CharacterIndex = new Dictionary<int, CharacterBase>();

    private void Awake()
    {
        int index = 0;

        // Keeps character select modular so you can add more characters
        foreach (CharacterButton b in buttons)
        {
            CharacterIndex.Add(index, b.character);
            b.characterIndex = index;
            b.button.onClick.AddListener(() => SelectCharacter(b.characterIndex));
            index++;
        }
    }

    public void SelectCharacter(int characterIndex)
    {
        ulong clientID = NetworkManager.Singleton.LocalClientId;
        if (NetworkManager.Singleton.IsServer)
        {
            AssignCharacter(clientID, characterIndex);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            RequestCharacterSelectServerRpc(clientID, characterIndex);
        }
        // Debug.Log("You selected " + character.characterName + "!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestCharacterSelectServerRpc(ulong client, int characterIndex)
    {
        AssignCharacter(client, characterIndex);
    }

    private void AssignCharacter(ulong client, int characterIndex)
    {
        // Allows character to be selected for given client
        // Server can call this directly, client can request the server selects the character on the client's player
        PlayerController player =
            NetworkManager.Singleton.ConnectedClients[client].PlayerObject.GetComponent<PlayerController>();
        player.selectedCharacter = CharacterIndex[characterIndex];
    }
}