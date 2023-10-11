using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 4;
    private const string GAMEPLAY_SCENE = "GameScene";
    private const string LOBBY_SCENE = "LobbyScene";

    public static InnGameMultiplayer Instance { get; private set; }

    public event EventHandler OnPlayerDataNetworkListChanged;
    
    private NetworkList<PlayerData> _playerDataNetworkList;
    // TEMP SERIALIZEDFIELD
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
        Debug.Log("Host Starting"); 
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(LOBBY_SCENE, LoadSceneMode.Single);
    }

    public void StartClient()
    {
        Debug.Log("Client Starting");
        
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        
        NetworkManager.Singleton.StartClient();
    }

    // this we made another function to then invoke the event so that other classes don't need to know about the player data network list
    // this tells subscribers, hey the player list changed, update stuff if needed.
    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }
    
    // Runs on Servers
    // Connect Player And enable their prefab, but not their visuals or gameplay controls
    // we need to make sure the player prefab visuals are disabled and their movement controls are disabled BEFORE we do anything else 
    private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData(clientId, _playerName));
        
        Debug.Log($"Server: {clientId} connected.");
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            Debug.Log(playerData.clientId);
        }
        
        SetPlayerNameServerRpc(GetPlayerName());
       // SetPlayerIdServerRpc();
    }
    
    // Runs on Clients
   
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
        //SetPlayerIdServerRpc();
        
        //SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
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

    // private void Update()
    // {
    //     Debug.Log($"Current Player List: {_playerDataNetworkList.Count}");
    // }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong obj)
    {
        throw new NotImplementedException();
    }
    
    // Tells the Server Something
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = _playerDataNetworkList[playerDataIndex];
        playerData.playerName = playerName;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }
    
    // [ServerRpc(RequireOwnership = false)]
    // private void SetPlayerIdServerRpc(ServerRpcParams serverRpcParams = default)
    // {
    //     int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
    //
    //     PlayerData playerData = _playerDataNetworkList[playerDataIndex];
    //
    //     //playerData.playerId = playerId;
    //
    //     _playerDataNetworkList[playerDataIndex] = playerData;
    //     
    //     // Should be telling the server that hey, give me this index and everything
    // }
    
    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }
    
    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        // THIS ISNT BEING UPDATED CORRECTLY -> PLAYER DATA NETWORK LIST NOT BEING SYCHRONIZED CORRECTLY?
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
    
    
    // NOT USED AT THE MOMENT
    public PlayerData GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerData playerData in _playerDataNetworkList)
        {
            if (playerData.clientId == clientId)
            {
                return playerData;
            }
        }

        return default;
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
