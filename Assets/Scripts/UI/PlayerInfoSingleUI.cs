using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInfoSingleUI : MonoBehaviour
{
	[SerializeField] private int playerIndex;
	[SerializeField] private TMP_Text playerNameText;

	private void Start()
	{
		InnGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += InnGameMultiplayer_OnPlayerDataNetworkListChanged;
		UpdatePlayer();
	}
	
	private void InnGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
	{
		// hey the game changed number of players or player details, see if this object needs to be updated.
		UpdatePlayer();
	}

	private void UpdatePlayer()
	{
		// this is just seeing that if serialized playerIndex, i.e. 0, 1, 2, 3 that we set in the inspector is connected,
		// if so we show ourselves.
		
		if (InnGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
		{
			Show();
			PlayerData playerData = InnGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			//readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
			
			playerNameText.text = playerData.playerName.ToString();
			
			//_playerVisual.SetPlayerColor(InnGameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
		}
		else
		{
			Hide();
		}
	}
	
	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}
	
	private void OnDestroy()
	{
		InnGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= InnGameMultiplayer_OnPlayerDataNetworkListChanged;
	}
}
