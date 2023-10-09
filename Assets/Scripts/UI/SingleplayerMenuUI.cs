using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _mainMenuButton;

    void Start()
    {
        _startButton.onClick.AddListener(() =>
            {
                Debug.Log("Game Starting");
                Hide();
            }
        );
        _mainMenuButton.onClick.AddListener(() =>
            {
                _mainMenuUI.Show();
                Hide();
            }
        );
    }
    
    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
