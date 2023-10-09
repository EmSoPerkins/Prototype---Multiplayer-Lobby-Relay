using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _startClientButton;

    
    void Start()
    {
        _startHostButton.onClick.AddListener(() =>
            {
                InnGameMultiplayer.Instance.StartHost();
                Hide();
            }
        );
        _startClientButton.onClick.AddListener(() =>
            {
                InnGameMultiplayer.Instance.StartClient();
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
