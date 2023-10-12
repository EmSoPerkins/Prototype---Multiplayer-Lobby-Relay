using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewMultiplayerMenuUI : MonoBehaviour
{
    [SerializeField] private NewMainMenuUI _mainMenuUI;
    [SerializeField] private Button _startHostButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _startClientButton;

    
    void Start()
    {
        _startHostButton.onClick.AddListener(() =>
            {
                Debug.Log("Starting Host");
                NewGameMultiplayer.Instance.StartHost();
                Hide();
            }
        );
        _startClientButton.onClick.AddListener(() =>
            {
                NewGameMultiplayer.Instance.StartClient();
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
