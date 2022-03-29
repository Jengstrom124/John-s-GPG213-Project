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
            private NetworkableString playerName;
            public int SelectedPlayerIndex;

            public string PlayerName
            {
                get
                {
                    return playerName;
                }
                private set
                {
                    playerName = value;
                }
            }
            
            public LobbyPlayerData(ulong clientId, string name, int playerNum)
            {
                ClientId = clientId;
                SelectedPlayerIndex = playerNum;
                playerName = new NetworkableString();

                PlayerName = name;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref playerName);
                serializer.SerializeValue(ref SelectedPlayerIndex);
            }

            public bool Equals(LobbyPlayerData other)
            {
                return ClientId == other.ClientId &&
                       playerName.Equals(other.playerName) &&
                       SelectedPlayerIndex == other.SelectedPlayerIndex;
            }
        }
}