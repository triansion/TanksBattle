using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelLayer
{
    Panel = 0,
    Tips
}

public class PanelManager : Singleton<PanelManager>
{
    // public static PanelManager instance;
    public GameObject UICanvas;
    public Transform[] PanelLayerParents;
    private Dictionary<System.Type,List<PanelBase>> mActivePanels = new Dictionary<System.Type, List<PanelBase>>(32);

    protected override void OnCreate()
    {
        UICanvas = GameObject.Find("UICanvas");
        int count = UICanvas.transform.childCount;
        PanelLayerParents = new Transform[count];
        for(int i = 0;i < count;i++)
        {
            PanelLayerParents[i] = UICanvas.transform.GetChild(i);
        }
    }

    public void InitPanelManager()
    {

    }

    public void ClearPanelDic()
    {
        mActivePanels.Clear();
    }

    // public bool HasPanel<T>() where T : PanelBase
    // {

    // }

    protected GameObject LoadPrefabFromRes(string prefabName)
    {
        if(string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("prefabName is null or empty");
            return null;
        }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/"+prefabName);
        if(prefab == null)
        {
            Debug.LogError(prefabName+" load prefab from Resources fail");
            return null;
        }

        return prefab;
    }

    public T OpenPanel<T>(string panelPrefabName,PanelLayer layer = PanelLayer.Panel,Transform panelParent = null,object[] args = null) where T : PanelBase
    {
        var type = typeof(T);
        if(!mActivePanels.ContainsKey(type))
        {
            mActivePanels.Add(type,new List<PanelBase>(32));
        }

        var panels = mActivePanels[type];

        if(panelParent == null)
        {
            if(layer == PanelLayer.Panel)
                panelParent = PanelLayerParents[0];
            else
                panelParent = PanelLayerParents[1];
        }

        GameObject panelPrefab = LoadPrefabFromRes(panelPrefabName);

        if(panelPrefab == null)
        {
            Debug.LogError("Open Panel fail");
            return null;
        }

        GameObject panelInstance = GameObject.Instantiate(panelPrefab,panelParent);
        T panel = panelInstance.GetComponent<T>();

        panels.Add(panel);

        panel.Open(panelInstance,panelParent,args);
        return panel;
    }

    public void ClosePanel<T>() where T : PanelBase
    {
        if(!mActivePanels.ContainsKey(typeof(T)))
            return;

        var panels = mActivePanels[typeof(T)];
        for(int i = 0;i < panels.Count;i++)
        {
            if(panels[i] != null)
            {
                panels[i].Close();
            }
        }
    }

    public void RemovePanel<T>(T panel) where T : PanelBase
    {
        var type = panel.GetType();
        if(!mActivePanels.ContainsKey(type))
            return;
        else
            mActivePanels[type].Remove(panel);
    }
}
