using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameIntroducePanel : PanelBase
{
    public Button CloseBtn;

    public GameIntroducePanel():base()
    {

    }

    protected override void OnShowed()
    {
        CloseBtn = transform.Find("BG/CloseBtn").GetComponent<Button>();
    }

    public void OnCloseBtnClicked()
    {
        PanelManager.Instance().ClosePanel<GameIntroducePanel>();
    }
}
