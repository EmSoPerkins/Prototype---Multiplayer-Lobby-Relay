using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NewGameManager : NetworkBehaviour
{
    public static NewGameManager Instance { get; private set;  }
    
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private List<Vector3> _spawnPositionList;
    
    private int currentSpawnPoint = 0;
    
    private void Awake()
    {
        Instance = this;
        
        //playerSpawnedDictionary = new Dictionary<ulong, bool>();
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log($"Is Server So Subscribing to LoadEventComplete");
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }
    
    private void SceneManager_OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        Debug.Log("SceneManager_OnLoadEventCompleted");
        
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log($"Spawning Player for {clientId}");
            Transform playerTransform = Instantiate(playerPrefab, _spawnPositionList[currentSpawnPoint], quaternion.identity);
            currentSpawnPoint++;
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }
}
