using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class Menu : SingletonBehaviour<Menu>
{
    public bool IsVisible => gameObject.activeSelf;
    protected override void Initialize()
    {

    }

    protected override void Shutdown()
    {

    }

    public void Show(bool show = true)
    {
        gameObject.SetActive(show);
        HUD.Instance.gameObject.SetActive(!show);

        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void StartGame()
    {
        Show(false);
        GameManager.Instance.Reset();
    }

    public void Exit()
    {
        Application.Quit();
    }
}