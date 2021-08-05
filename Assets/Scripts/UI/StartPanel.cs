using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartPanel : PanelBase
{
    public Button startBtn;
    public Button introduceBtn;
    public Button quitBtn;

    public StartPanel()
    {

    }

    protected override void OnShowed()
    {
        startBtn = transform.Find("StartBtn").GetComponent<Button>();
        introduceBtn = transform.Find("IntroduceBtn").GetComponent<Button>();
        quitBtn = transform.Find("QuitBtn").GetComponent<Button>();
    }
    
    public void OnStartBtnClicked()
    {
        BattleManager.instance.StartTowCampBattle();
        PanelManager.Instance().ClosePanel<StartPanel>();
    }
    
    public void OnIntroduceBtnClicked()
    {
        PanelManager.Instance().OpenPanel<GameIntroducePanel>("GameIntroducePanel",PanelLayer.Tips);
    }
    
    public void OnQuitBtnClicked()
    {
        Application.Quit();
    }
}
