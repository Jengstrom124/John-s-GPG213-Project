using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Gerallt
{ 
    public struct PlayerTransform : INetworkSerializable
        {
            public ulong clientId;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 velocity;
            public Vector3 angularVelocity;

            public PlayerTransform(ulong clientId, Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
            {
                this.clientId = clientId;
                this.position = position;
                this.rotation = rotation;
                this.velocity = velocity;
                this.angularVelocity = angularVelocity;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref clientId);
                serializer.SerializeValue(ref position);
                serializer.SerializeValue(ref rotation);
                serializer.SerializeValue(ref velocity);
            }
        }
}