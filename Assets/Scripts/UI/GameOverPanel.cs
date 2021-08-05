using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : PanelBase
{
    public GameObject winView;
    public GameObject failView;
    public Button closeBtn;

    private bool isWin = false;

    protected override void OnShowed()
    {
        isWin = args.Length > 0 ? (bool)args[0] : false;
        winView.SetActive(isWin);
        failView.SetActive(!isWin);
    }

    public void ShowWinView()
    {
        winView.SetActive(true);
    }

    public void ShowFailView()
    {
        failView.SetActive(true);
    }

    public void OnCloseBtnClicked()
    {
        PanelManager.Instance().ClosePanel<GameOverPanel>();
        PanelManager.Instance().OpenPanel<StartPanel>("StartPanel");
    }
}
