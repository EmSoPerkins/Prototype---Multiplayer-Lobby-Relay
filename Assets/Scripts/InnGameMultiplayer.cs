using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InnGameMultiplayer : MonoBehaviour
{
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
    }

    public void StartClient()
    {
        Debug.Log("Client Started");
        NetworkManager.Singleton.StartClient();
    }
}
