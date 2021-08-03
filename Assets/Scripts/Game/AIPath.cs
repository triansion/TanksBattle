using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPath
{
    public List<Vector3> wayPoints;
    public int currentWayPointIndex = -1;
    public Vector3 currentWayPoint;
    public bool isLoop = false;
    public bool isFinish = false;
    public float deviation = 3f;
    private float distance = 0f;
    private NavMeshPath navMeshPath;

    public bool IsReachWayPoint(Transform enemyTankTrans)
    {
        if(wayPoints == null)
            return false;
        distance = Vector3.Distance(currentWayPoint,enemyTankTrans.position);
        return distance <= deviation;
    }

    public void NextWayPoint()
    {
        if(currentWayPointIndex < 0)
            return;

        if(currentWayPointIndex < wayPoints.Count - 1)
        {
            currentWayPointIndex++;
        }
        else
        {
            if(isLoop)
            {
                currentWayPointIndex = 0;
            }
            else
            {
                isFinish = true;
            }
        }
        currentWayPoint = wayPoints[currentWayPointIndex];
    }

    public void GenerateWayPoints(Transform wayPointParent,bool isLoop = false)
    {
        if(wayPointParent.childCount <= 0)
        {
            wayPoints = null;
            currentWayPointIndex = -1;
            isLoop = false;
            isFinish = false;
            return;
        }

        int count = wayPointParent.childCount;
        wayPoints = new List<Vector3>();
        for(int i = 0;i < count;i++)
        {
            // wayPoints[i] = wayPointParent.GetChild(i).position;
            wayPoints.Add(navMeshPath.corners[i]);
        }
        currentWayPointIndex = 0;
        currentWayPoint = wayPoints[currentWayPointIndex];
        this.isLoop = isLoop;
        isFinish = false;
    }

    public void GenerateWayPointsByNavMesh(Transform srcPos, Transform destPos, bool isLoop = false, bool isNeedReset = true)
    {
        if(navMeshPath == null)
            navMeshPath = new NavMeshPath();

        bool hasCalculatePath = NavMesh.CalculatePath(srcPos.position,destPos.position,NavMesh.AllAreas,navMeshPath);

        if(!hasCalculatePath)
            return;

        int count = navMeshPath.corners.Length;
        if(isNeedReset)
        {
            wayPoints = null;
            currentWayPointIndex = -1;
            wayPoints = new List<Vector3>();
        }
        // Debug.Log("自动生成的路点: "+navMeshPath.corners);
        // int currentListCount = wayPoints.Count;
        for(int i = 0;i < count;i++)
        {
            // wayPoints[i] = navMeshPath.corners[i];
            wayPoints.Add(navMeshPath.corners[i]);
            // Debug.Log("自动生成的路点"+i+": "+navMeshPath.corners[i]);
        }

        currentWayPointIndex = 0;
        currentWayPoint = wayPoints[currentWayPointIndex];
        this.isLoop = isLoop;
        isFinish = false;
    }

    public void DrawWayPoints()
    {
        if(wayPoints == null)
            return;

        if(wayPoints.Count != 0)
        {
            int count = wayPoints.Count;
            for(int i = 0;i < count;i++)
            {
                if(i == currentWayPointIndex)
                    Gizmos.DrawSphere(wayPoints[i],1);
                else
                    Gizmos.DrawCube(wayPoints[i],Vector3.one);
            }
        }
    }
}
