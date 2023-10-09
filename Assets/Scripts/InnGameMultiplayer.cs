using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InnGameMultiplayer : MonoBehaviour
{
    private const string GAMEPLAY_SCENE = "GameScene";
    private const string LOBBY_SCENE = "LobbyScene";

    public static InnGameMultiplayer Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void StartHost()
    { 
        Debug.Log("Host Started"); 
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene(LOBBY_SCENE, LoadSceneMode.Single);
    }

    public void StartClient()
    {
        Debug.Log("Client Started");
        NetworkManager.Singleton.StartClient();
    }
}
