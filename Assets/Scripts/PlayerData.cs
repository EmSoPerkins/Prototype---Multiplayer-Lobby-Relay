using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
	public ulong clientId;
	public FixedString64Bytes playerName;

	public bool isSpawned;
	//public FixedString64Bytes playerId;

	
	public PlayerData(ulong clientId, FixedString64Bytes playerName, bool isSpawned = false)
	{
		this.clientId = clientId;
		this.playerName = playerName;
		this.isSpawned = isSpawned;
	}
	
	public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	{
		serializer.SerializeValue(ref clientId);
		serializer.SerializeValue(ref playerName);
		serializer.SerializeValue(ref isSpawned);
	}

	public bool Equals(PlayerData other)
	{
		return clientId == other.clientId &&
		       playerName == other.playerName &&
		       isSpawned == other.isSpawned;
	}
	
	// public PlayerData(ulong clientId, FixedString64Bytes playerName, FixedString64Bytes playerId)
	// {
	// 	this.clientId = clientId;
	// 	this.playerName = playerName;
	// 	this.playerId = playerId;
	// }
	//
	// public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
	// {
	// 	serializer.SerializeValue(ref clientId);
	// 	serializer.SerializeValue(ref playerName);
	// 	serializer.SerializeValue(ref playerId);
	// }
	//
	// public bool Equals(PlayerData other)
	// {
	// 	return clientId == other.clientId &&
	// 	       playerName == other.playerName &&
	// 	       playerId == other.playerId;
	// }
}
