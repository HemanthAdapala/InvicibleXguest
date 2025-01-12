using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    public FixedString32Bytes playerName;
    public FixedString32Bytes playerId;

    public bool Equals(PlayerData other)
    {
        clientId = other.clientId;
        colorId = other.colorId;
        playerName = other.playerName;
        playerId = other.playerId;
        return true;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}