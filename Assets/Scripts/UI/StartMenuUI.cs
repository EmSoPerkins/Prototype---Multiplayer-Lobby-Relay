using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private Button _startGameButton;
    
    private void Start()
    {
        _startGameButton.onClick.AddListener(ShowMainMenu);
        Show();
    }

    private void ShowMainMenu()
    {
        _mainMenuUI.Show();
        Hide();
    }
    
    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
