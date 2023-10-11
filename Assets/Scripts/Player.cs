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
    
    private PlayerInputActions playerInputActions;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_Performed;
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        // Debug.Log("Client ID: " + OwnerClientId);
        // Debug.Log("Player Data Index: " + InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId));
        //     
        // transform.position =
        //     spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner || !InnGameManager.Instance.IsLocalPlayerSpawned()) return;

       
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        
        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
        float moveSpeed = 7f;
        float moveDistance = moveSpeed * Time.deltaTime;
        //float playerRadius = 0.7f;
        
        transform.position += moveDir * moveDistance;
    }
    
    private void Interact_Performed(InputAction.CallbackContext obj)
    {
        Debug.Log("player hit e");
        if (!InnGameManager.Instance.IsLocalPlayerSpawned())
        {
             Debug.Log("Player Data Index: " + InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId));
             transform.position =
                 spawnPositionList[InnGameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
             OnPlayerSpawned?.Invoke(this, EventArgs.Empty);
        }
    }
}
