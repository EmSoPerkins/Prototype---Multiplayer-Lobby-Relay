using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public static Player LocalInstance { get; private set; }

    public event EventHandler OnPlayerSpawned;
    
    [SerializeField] private List<Vector3> spawnPositionList;
    [SerializeField] private Transform playerVisual;
    [SerializeField] private bool playerIsActive;
    
    private PlayerInputActions playerInputActions;
    
    // Start is called before the first frame update
    void Start()
    {
        // if (IsServer)
        // {
        //     SpawnPlayer();
        // }
        // else
        // {
        //     RemoveVisualsAndControls();
        // }
        
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_Performed;
    }

    private void RemoveVisualsAndControls()
    {
        playerVisual.gameObject.SetActive(false);
        playerIsActive = false;
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }
        
        //transform.position =
         //   spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        
        RemoveVisualsAndControls();
    }
    
    
    // Update is called once per frame
    void Update()
    {
        //if we're not the owner or we're not active, just return
        if (!IsOwner || !playerIsActive) return;
        
        
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveSpeed = 7f;
        float moveDistance = moveSpeed * Time.deltaTime;
        
        transform.position += moveDir * moveDistance;
    }
    
    private void Interact_Performed(InputAction.CallbackContext obj)
    {
        if (IsOwner && !playerIsActive)
        {
            Debug.Log("Player Hit Interact Key");
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        Debug.Log("Player Data Index: " + InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId));
        transform.position =
            spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        AddVisualsAndControls();
    }
    
    // player is reenabled visually locally, but not on the server
    // the server will correctly position the characters on both
    // the client still won't be able to move afterwards
    private void AddVisualsAndControls()
    {
        playerVisual.gameObject.SetActive(true);
        playerIsActive = true;
    }
    
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
