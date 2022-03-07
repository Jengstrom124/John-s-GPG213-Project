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

        foreach (CharacterButton b in buttons)
        {
            CharacterIndex.Add(index, b.character);
            // int buttonIndex = index; // Need this temporary int so listener isn't overwritten when index increments
            b.characterIndex = index;
            b.button.onClick.AddListener(() => SelectCharacter(b.characterIndex));
            index++;
        }
    }

    public void SelectCharacter(int characterIndex)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            AssignCharacter(characterIndex);
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            RequestCharacterSelectServerRpc(characterIndex);
        }
        // Debug.Log("You selected " + character.characterName + "!");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestCharacterSelectServerRpc(int characterIndex)
    {
        AssignCharacter(characterIndex);
    }

    private void AssignCharacter(int characterIndex)
    {
        PlayerController player =
            NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>();
        player.selectedCharacter = CharacterIndex[characterIndex];
    }
}