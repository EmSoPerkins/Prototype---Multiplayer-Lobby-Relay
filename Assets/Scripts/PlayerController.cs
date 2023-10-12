using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.UI;

public class PlayerController : NetworkBehaviour
{
	
	private PlayerInputActions _playerInputActions;
    
	// Start is called before the first frame update
	void Initialize()
	{
		_playerInputActions = new PlayerInputActions();
		_playerInputActions.Player.Enable();
		//playerInputActions.Player.Interact.performed += Interact_Performed;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		Initialize();
	}

	void Update()
	{
		if (!IsOwner) return;
        
        
		Vector2 inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;
        
		Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
		float moveSpeed = 7f;
		float moveDistance = moveSpeed * Time.deltaTime;
        
		transform.position += moveDir * moveDistance;
	}
}
