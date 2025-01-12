using System;
using Unity.Netcode;
using UnityEngine;

public struct KeyObjectData : IEquatable<KeyObjectData>,INetworkSerializable
{
    public bool isHeld;
    public ulong ownerId;
    public bool Equals(KeyObjectData other)
    {
        isHeld = other.isHeld;
        ownerId = other.ownerId;
        return true;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref isHeld);
        serializer.SerializeValue(ref ownerId);
    }
}
