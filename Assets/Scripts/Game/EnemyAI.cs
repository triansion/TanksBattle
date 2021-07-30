using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum Status
    {
        Patrol = 0,
        Attack,
        Escape
    }
    private Status m_status = Status.Patrol;
    public Status AI_Status
    {
        get 
        {
            return m_status;
        }
        set
        {
            m_status = value;
        }
    }

    public Tank aiTank;
    
    public void ChangeAIStatus(Status status)
    {
        if(status == Status.Patrol)
        {
            PatrolStart();
        }
        else if(status == Status.Attack)
        {
            AttackStart();
        }
        else if(status == Status.Escape)
        {
            EscapeStart();
        }
    }

    private Transform turret;
    private Transform launchPos;

    private AIPath aIPath;

    void Start()
    {
        turret = transform.Find("TankRenderers/TankTurret");
        launchPos = turret.GetChild(0);
        tankCenterPos = new Vector3(0,transform.localScale.y,0);
        aIPath = new AIPath();

        InitAIWayPoints();

        InitaiTankToWayPointDir();
    }


    private void PatrolStart()
    {

    }

    private void AttackStart()
    {

    }

    private void EscapeStart()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(aiTank != null || aiTank.ctrlType != CtrlType.AI)
            return;

        if(m_status == Status.Patrol)
        {
            PatrolUpdate();
        }
        else if(m_status == Status.Attack)
        {
            AttackUpdate();
        }
        else if(m_status == Status.Escape)
        {
            EscapeUpdate();
        }
    }

    private void PatrolUpdate()
    {
        
    }
    private void AttackUpdate()
    {
        
    }
    private void EscapeUpdate()
    {
        
    }

    void FixedUpdate() {

        SearchPlayerTank();

        QueryWayPoint();
    }

    public bool IsTakeTarget()
    {
        return target != null;
    }

    #region 主动搜索目标逻辑
    private GameObject target;
    private float searchDistance = 50f;
    private float searchInterval = 3f;
    private float lastSearchTime = 0;

    private void SearchPlayerTank()
    {
        float interval = Time.time - lastSearchTime;
        if(interval <= searchInterval)
            return;

        lastSearchTime = Time.time;
        if(target != null)
        {
            HasTarget();
        }
        else
        {
            NoTarget();
        }
    }

    private void HasTarget()
    {
        Tank targetTank = target.GetComponent<Tank>();
        if(targetTank.ctrlType == CtrlType.none)
        {
            target = null;
            Debug.Log("目标死亡,丢失目标");
            return;
        }
        
        if(Vector3.Distance(transform.position,target.transform.position) > searchDistance)
        {
            target = null;
            Debug.Log("距离太远,丢失目标");
        }
    }

    private Collider[] nearbyColliders;
    private void NoTarget()
    {
        Debug.Log("当前没有目标,主动搜索目标");
        nearbyColliders = Physics.OverlapSphere(transform.position,searchDistance,LayerMask.GetMask("Tank"));
        for(int i=0;i<nearbyColliders.Length;i++)
        {
            if(nearbyColliders[i].transform.tag != "Player")
                continue;

            Tank playerTank = nearbyColliders[i].gameObject.GetComponent<Tank>();
            if(playerTank.ctrlType == CtrlType.none)
                continue;

            target = playerTank.gameObject;
        }
        if(target != null)
        {
            Debug.Log("捕获目标"+target.name);
        }
    }
    #endregion

    #region 被动搜索目标逻辑(被攻击时锁定目标)
    public void OnBeAttacked(GameObject attacker)
    {
        target = attacker;
        Debug.Log("被攻击,捕获目标"+target.name);
    }
    #endregion

    #region AI旋转炮塔炮管逻辑
    private Vector3 turretDir;
    private Vector3 gunDir;
    private Vector3 tankCenterPos;
    public void CalculateTurretRotate(out Quaternion turretRotateAngle,out Quaternion gunRotateAngle)
    {
        if(target == null)
        {
            turretRotateAngle = Quaternion.Euler(aiTank.transform.forward);
            gunRotateAngle = Quaternion.Euler(aiTank.transform.forward);
        }
        else
        {
            turretDir = target.transform.position + tankCenterPos - turret.position;
            gunDir = target.transform.position + tankCenterPos - launchPos.position;
            turretRotateAngle = Quaternion.LookRotation(turretDir);
            gunRotateAngle = Quaternion.LookRotation(gunDir);
        }
            // Debug.Log("AI炮塔旋转角度:"+turretRotateAngle.eulerAngles);
            // Debug.Log("AI炮管旋转角度:"+gunRotateAngle.eulerAngles);
    }

    #endregion

    #region AI发射炮弹逻辑

    public bool IsShouldShot()
    {
        if(target == null)
            return false;
        else
        {
            return true;
        }
    }

    #endregion

    #region AI寻路逻辑
    
    private void InitAIWayPoints()
    {
        Transform wayPointsParent = GameObject.Find("WayPointsParent").transform;

        // aIPath.GenerateWayPoints(wayPointsParent,true);

        // Transform wayPoint = wayPointsParent.GetChild(0);
        int wayPointCount = wayPointsParent.childCount;
        if(wayPointCount > 0)
        {
            Transform wayPoint;
            for(int i = 0; i < wayPointCount; i++)
            {
                wayPoint = wayPointsParent.GetChild(i);
                if(i == 0)
                    aIPath.GenerateWayPointsByNavMesh(transform,wayPoint,true,true);
                else
                    aIPath.GenerateWayPointsByNavMesh(wayPointsParent.GetChild(i-1),wayPoint,true,false);
            }
        }
    }
    
    private void QueryWayPoint()
    {
        if(aIPath.IsReachWayPoint(transform))
        {
            aIPath.NextWayPoint();
        }
    }

    private void InitaiTankToWayPointDir()
    {
        aiTankToWayPointDirInWold = new Vector3(0,0,0);
        aiTankToWayPointDirInLocal = new Vector3(0,0,0);
    }

    private Vector3 aiTankToWayPointDirInWold = new Vector3(0,0,0);
    private Vector3 aiTankToWayPointDirInLocal = new Vector3(0,0,0);

    private void CalculateAITankToWayPointDir()
    {
        aiTankToWayPointDirInWold = aIPath.currentWayPoint - transform.position;
        aiTankToWayPointDirInLocal = transform.InverseTransformDirection(aiTankToWayPointDirInWold);
    }

    public float GetSteering()
    {
        if(aiTank == null || aIPath == null)
            return 0;

        CalculateAITankToWayPointDir();

        if(aiTankToWayPointDirInLocal.x > 1)
            return aiTank.maxSteeringAngle;
        else if(aiTankToWayPointDirInLocal.x < -1)
            return -aiTank.maxSteeringAngle;
        else
            return 0;
    }

    public float GetMotor()
    {
        if(aiTank == null || aIPath == null)
            return 0;
        
        CalculateAITankToWayPointDir();
        if(aiTankToWayPointDirInLocal.z >= 0)
            return aiTank.maxMotor;
        else
            return -aiTank.maxMotor;
    }

    public float GetBrake()
    {
        if(aiTank == null || aIPath == null)
            return 0;

        if(aIPath.isFinish)
            return aiTank.maxBrake;
        else
            return 0;
    }

    void OnDrawGizmos() {
        if(aIPath != null)
            aIPath.DrawWayPoints();
    }

    #endregion

}
