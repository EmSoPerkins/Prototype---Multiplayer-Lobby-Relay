using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class Player : NetworkBehaviour
{
    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPlayerSpawned;
    
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private NetworkVariable<bool> playerIsSpawned = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private PlayerInputActions playerInputActions;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_Performed;
    }

    private void RemoveVisualsAndControls()
    {
        playerVisual.gameObject.SetActive(false);
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        playerIsSpawned.OnValueChanged += (value, newValue) =>
        {
            Debug.Log(OwnerClientId + "; has Spawned, Updating Players");
            UpdatePlayers();
        };
        
        if (IsServer)
        {
            UpdatePlayers();
            playerIsSpawned.Value = true;
        }
        else
        {
            RemoveVisualsAndControls();
        }
        
        //transform.position =
         //   spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];


  
    }
    
    
    // Update is called once per frame
    void Update()
    {
        //if we're not the owner or we're not active, just return
        if (!IsOwner || playerIsSpawned.Value == false) return;
        
        
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveSpeed = 7f;
        float moveDistance = moveSpeed * Time.deltaTime;
        
        transform.position += moveDir * moveDistance;
    }
    
    private void Interact_Performed(InputAction.CallbackContext obj)
    {
        if (IsOwner && playerIsSpawned.Value == false)
        {
            Debug.Log("Local Player Hit Interact Key to spawn Herself");
            playerIsSpawned.Value = true;
            InnGameManager.Instance.SpawnPlayer();
        }
    }

    private void UpdatePlayers()
    {
        Debug.Log("Updating Player Prefab with Owner of: " + OwnerClientId);
        transform.position =
            spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        AddPlayerVisuals();
    }
    
    private void SpawnPlayer()
    {
        Debug.Log("Player Data Index: " + InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId));
        transform.position =
            spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        AddPlayerVisuals();
    }
    
    // player is reenabled visually locally, but not on the server
    // the server will correctly position the characters on both
    // the client still won't be able to move afterwards
    private void AddPlayerVisuals()
    {
        playerVisual.gameObject.SetActive(true);
    }
    
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
