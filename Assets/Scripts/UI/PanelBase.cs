using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBase : MonoBehaviour
{
    protected GameObject instance = null;
    protected Transform layer;
    protected object[] args;
    protected bool isOpen = false;

    public PanelBase()
    {

    }

    #region UI面板生命周期

    protected virtual void OnShowing() {}

    protected virtual void OnShowed() {}

    protected virtual void OnClosing() {}

    protected virtual void OnClosed() {}

    #endregion

    #region UI面板操作
    public void Open(GameObject panelInstance,Transform parent,object[] args)
    {
        this.args = args;

        instance = panelInstance;
        isOpen = true;

        OnShowed();
    }

    public void Close()
    {
        if(!isOpen)
            return;

        isOpen = false;

        PanelManager.Instance().RemovePanel(this);

        OnClosed();

        if(instance != null)
        {
            Object.Destroy(instance);
            instance = null;
        }
    }

    #endregion
}
