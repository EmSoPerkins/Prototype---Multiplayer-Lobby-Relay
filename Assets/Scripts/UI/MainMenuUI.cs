using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SingleplayerMenuUI _singleplayerMenuUI;
    [SerializeField] private MultiplayerMenuUI _multiplayerMenuUI;
    [SerializeField] private Button _singleplayerButton;
    [SerializeField] private Button _multiplayerButton;

    void Start()
    {
        _singleplayerButton.onClick.AddListener(() =>
            {
                _singleplayerMenuUI.Show();
                Hide();
            }
        );
        _multiplayerButton.onClick.AddListener(() =>
            {
                _multiplayerMenuUI.Show();
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
