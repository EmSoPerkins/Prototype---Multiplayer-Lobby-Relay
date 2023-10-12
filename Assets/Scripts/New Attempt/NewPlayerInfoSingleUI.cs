using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewPlayerInfoSingleUI : MonoBehaviour
{
	[SerializeField] private int playerIndex;
	[SerializeField] private TMP_Text playerNameText;

	private void Start()
	{
		NewGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += InnGameMultiplayer_OnPlayerDataNetworkListChanged;
		UpdatePlayer();
	}
	
	private void InnGameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
	{
		UpdatePlayer();
	}

	private void UpdatePlayer()
	{
		if (NewGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
		{
			Show();
			PlayerData playerData = NewGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			playerNameText.text = playerData.playerName.ToString();
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
