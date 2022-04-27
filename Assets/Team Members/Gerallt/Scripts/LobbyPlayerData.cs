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
            public NetworkableString ClientIPAddress;
            public int SelectedPlayerIndex;
            public int characterIndex;

            public LobbyPlayerData(ulong clientId, string name, int playerNum)
            {
                ClientId = clientId;
                SelectedPlayerIndex = playerNum;
                PlayerName = name;
                ClientIPAddress = string.Empty;
                characterIndex = 0;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref PlayerName);
                serializer.SerializeValue(ref SelectedPlayerIndex);
                serializer.SerializeValue(ref ClientIPAddress);
                serializer.SerializeValue(ref characterIndex);
            }

            public bool Equals(LobbyPlayerData other)
            {
                return ClientId == other.ClientId &&
                       PlayerName.Equals(other.PlayerName) &&
                       SelectedPlayerIndex == other.SelectedPlayerIndex;
            }
        }
}