using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    
    private const string NEW_LOBBY_SCENE = "NewLobbyScene";
    
    public static NewGameMultiplayer Instance { get; private set; }
    
    public event EventHandler OnPlayerDataNetworkListChanged;

    private NetworkList<PlayerData> _playerDataNetworkList;
    
    [SerializeField] string _playerName = "Default";


    
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _playerDataNetworkList = new NetworkList<PlayerData>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public void StartHost()
    { 
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        
        Debug.Log("Host Starting"); 
        Debug.Log("Host Loading Next Scene");
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(NEW_LOBBY_SCENE, LoadSceneMode.Single);
    }
    
    
    public void StartClient()
    {
        Debug.Log("Client Starting");
        
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }
    
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }
    
    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData(clientId, _playerName));
        
        Debug.Log($"Server: {clientId} connected.");
        Debug.Log($"Current Players:");

        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            Debug.Log($"ClientID: {playerData.clientId}");
        }
        
        SetPlayerNameServerRpc(GetPlayerName());
    }
    
    private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
    {
        Debug.Log($"Client: {clientId} connected.");
        Debug.Log($"Current Players:");

        
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            Debug.Log($"ClientID: {playerData.clientId}");
        }
        
        Debug.Log($"Setting Player Name");
        
        SetPlayerNameServerRpc(GetPlayerName());
    }
    
    private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
    {
        for (int index = 0; index < _playerDataNetworkList.Count; index++)
        {
            if (_playerDataNetworkList[index].clientId == clientId)
            {
                _playerDataNetworkList.RemoveAt(index);
                break;
            }
        }
    }
    
    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        throw new NotImplementedException();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }
    
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }
    
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        Debug.Log("Player Data Network List Count: " + _playerDataNetworkList.Count);
        
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].clientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }
    
    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }
    
    public string GetPlayerName()
    {
        return _playerName;
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_Server_OnClientDisconnectCallback;
    }
}
