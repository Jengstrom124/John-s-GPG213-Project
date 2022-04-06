using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{ 
    public struct LobbyPlayerData : INetworkSerializable, IEquatable<LobbyPlayerData>
        {
            public ulong ClientId;

            public NetworkableString PlayerName;
            //private NetworkableString playerName;
            public int SelectedPlayerIndex;

            // public string PlayerName
            // {
            //     get
            //     {
            //         return playerName;
            //     }
            //     set
            //     {
            //         playerName = value;
            //     }
            // }
            
            public LobbyPlayerData(ulong clientId, string name, int playerNum)
            {
                ClientId = clientId;
                SelectedPlayerIndex = playerNum;
                //playerName = new NetworkableString();
                //PlayerName = name;
                PlayerName = name;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref PlayerName);
                serializer.SerializeValue(ref SelectedPlayerIndex);
            }

            public bool Equals(LobbyPlayerData other)
            {
                return ClientId == other.ClientId &&
                       PlayerName.Equals(other.PlayerName) &&
                       SelectedPlayerIndex == other.SelectedPlayerIndex;
            }
        }
}