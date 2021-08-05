using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTank
{
    public Tank tank;
    public int camp;
}

public class BattleManager : MonoBehaviour
{

    public GameObject[] battleTnakPrefabs;

    // Start is called before the first frame update
    public static BattleManager instance;

    public GameObject aimCanvas;

    void Start()
    {
        instance = this;
        // StartTowCampBattle();
        // aimCanvas = GameObject.Find("AimCanvas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BattleTank[] battleTanks;

    public int GetCamp(GameObject tankObj)
    {
        if(tankObj == null)
        {
            Debug.LogError("tankObj is null");
            return 0;
        }
        int count = battleTanks.Length;
        for(int i = 0;i < count;i++)
        {
            if(battleTanks[i] == null)
            {
                Debug.LogError("battleTanks["+i+"] is null");
                continue;
            }
            if(battleTanks[i].tank.gameObject == tankObj)
                return battleTanks[i].camp;
        }
        return 0;
    }
    
    public bool IsSameCamp(GameObject tank1,GameObject tank2)
    {
        if(tank1 == null || tank2 == null)
            return false;
        return GetCamp(tank1) == GetCamp(tank2);
    }


    private int winCamp = -1;
    private BattleTank playerBattleTank;
    public bool IsWin(int camp)
    {
        int count = battleTanks.Length;
        for(int i = 0;i < count;i++)
        {
            if(battleTanks[i].camp != camp)
            {
                if(battleTanks[i].tank.CurrentHp > 0)
                    return false;
            }
        }
        Debug.Log("阵营"+camp+"获胜");
        winCamp = camp;
        aimCanvas.SetActive(false);
        bool isPlayerWin = IsPlayerWin();
        PanelManager.Instance().OpenPanel<GameOverPanel>("GameOverPanel",PanelLayer.Tips,null,isPlayerWin);
        ClearAllTank();
        return true;
    }

    public bool IsWin(GameObject tankObj)
    {
        int camp = GetCamp(tankObj);
        if(camp == 0)
        {
            Debug.LogError("此坦克阵营未知");
            return false;
        }
        return IsWin(camp);
    }

    private bool IsPlayerWin()
    {
        return playerBattleTank.camp == winCamp;
    }

    public void ClearAllTank()
    {
        GameObject[] tankObjs = GameObject.FindGameObjectsWithTag("Enemy");
        if(tankObjs.Length > 0)
        {
            int count = tankObjs.Length;
            for(int i = 0;i < count;i++)
            {
                Destroy(tankObjs[i]);
            }
        }
        tankObjs = GameObject.FindGameObjectsWithTag("Player");
        if(tankObjs.Length > 0)
        {
            int count = tankObjs.Length;
            for(int i = 0;i < count;i++)
            {
                Destroy(tankObjs[i]);
            }
        }
    }

    public void HideAimCanvas()
    {
        aimCanvas.SetActive(false);
    }

    public void StartTowCampBattle(int n1 = 2,int n2 = 2)
    {
        Transform birthPointParent = GameObject.Find("BirthPoints").transform;
        if(birthPointParent == null)
        {
            Debug.LogError("坦克出生点找不到");
            return;
        }
        Transform camp1BirthPointParent = birthPointParent.GetChild(0);
        Transform camp2BirthPointParent = birthPointParent.GetChild(1);
        if(camp1BirthPointParent.childCount < n1 || camp2BirthPointParent.childCount < n2)
        {
            Debug.LogError("坦克出生点数量不够");
            return;
        }
        ClearAllTank();

        battleTanks = new BattleTank[n1+n2];
        for(int i = 0;i < n1;i++)
        {
            GenerateTank(battleTnakPrefabs[0],1,i,camp1BirthPointParent.GetChild(i).position);
        }

        for(int j = 0;j < n2;j++)
        {
            GenerateTank(battleTnakPrefabs[1],2,n1+j,camp2BirthPointParent.GetChild(j).position);
        }

        int playerTankIndex = Random.Range(0,n1+n2);
        while(battleTanks[playerTankIndex] == null)
        {
            playerTankIndex = Random.Range(0,n1+n2);
        }
        Tank playerTank = battleTanks[playerTankIndex].tank;
        playerTank.transform.tag = "Player";
        playerTank.ctrlType = CtrlType.player;
        playerBattleTank = battleTanks[playerTankIndex];

        CameraFollow cameraFollow = Camera.main.transform.GetComponent<CameraFollow>();
        if(cameraFollow != null)
            cameraFollow.SetFollowTargetTank(playerTank.gameObject);

        aimCanvas.SetActive(true);
    }

    private void GenerateTank(GameObject tankPrefab,int camp,int index,Vector3 birthPos)
    {
        GameObject tankObj = GameObject.Instantiate(tankPrefab,birthPos,Quaternion.identity);
        tankObj.tag = "Enemy";

        Tank tank = tankObj.GetComponent<Tank>();
        tank.ctrlType = CtrlType.AI;

        BattleTank battleTank = new BattleTank();
        battleTank.camp = camp;
        battleTank.tank = tank;

        battleTanks[index] = battleTank;
    }
}
