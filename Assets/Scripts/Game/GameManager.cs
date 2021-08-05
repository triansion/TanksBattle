using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitGame()
    {
        PanelManager.Instance().InitPanelManager();
        PanelManager.Instance().OpenPanel<StartPanel>("StartPanel");
    }
}
