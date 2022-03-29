using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterBase selectedCharacter;

    public string playerName;

    public NetworkVariable<Color> playerColour;
}
