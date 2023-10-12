using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnGameManager : NetworkBehaviour
{
    public static InnGameManager Instance { get; private set;  }
    
    [SerializeField] private Transform playerPrefab;


    private Dictionary<ulong, bool> playerSpawnedDictionary;
    [SerializeField] private bool isLocalPlayerSpawned;
    
    private void Awake()
    {
        Instance = this;
        
        playerSpawnedDictionary = new Dictionary<ulong, bool>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // subscribe to the network scene manager to see if we've changed scenes
        
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log($"Is Server So Subscribing to LoadEventComplete");
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
            //InnGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += InnGameMultiplayer_OnPlayerDataNetworkListChanged;
            
        }
    }

    private void Player_OnPlayerSpawned(object sender, EventArgs e)
    {
        isLocalPlayerSpawned = true;
    }

    private void InnGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("InnGameMultiplayer_OnPlayerDataNetworkListChanged");
        //ConnectPlayer();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }
    }   
    
    // this is useful after we already have everyone connected and we're trying to load different scenes
    private void SceneManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        Debug.Log("SceneManager_OnLoadEventCompleted");
        isLocalPlayerSpawned = true;
        
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"Spawning Player for {clientId}");
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    public void UpdatePlayers()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"Player: {clientId} is in the playerSpawnedDictionary {playerSpawnedDictionary.ContainsKey(clientId)}");
            if (playerSpawnedDictionary.ContainsKey(clientId) && playerSpawnedDictionary[clientId] == false)
            {
                Debug.Log($"Spawning Player for {clientId}");
                Transform playerTransform = Instantiate(playerPrefab);
                playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

                // Transform playerTransform = Instantiate(playerPrefab); // the server spawns the client's player prefab for them
                // playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                // playerSpawnedDictionary[clientId] = true;
                //isLocalPlayerSpawned = true;
                //Player.LocalInstance.OnPlayerSpawned += Player_OnPlayerSpawned;
                //}

                //Debug.Log($"Player: {clientId} - is Spawned: {playerSpawnedDictionary[clientId]}");
            }
        }
    }

    public void SpawnPlayer()
    {
        isLocalPlayerSpawned = true;
        SetPlayerSpawnedServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerSpawnedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;
        playerSpawnedDictionary[clientId] = true;
        Debug.Log($"Spawning Player for {clientId}");
        Transform playerTransform = Instantiate(playerPrefab);
        playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
    
    
    public bool IsLocalPlayerSpawned()
    {
        return isLocalPlayerSpawned;
    }
}
