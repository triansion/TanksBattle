using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPath
{
    public Vector3[] wayPoints;
    public int currentWayPointIndex = -1;
    public Vector3 currentWayPoint;
    public bool isLoop = false;
    public bool isFinish = false;
    public float deviation = 5f;
    private float distance = 0f;

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

        if(currentWayPointIndex < wayPoints.Length - 1)
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
        wayPoints = new Vector3[count];
        for(int i = 0;i < count;i++)
        {
            wayPoints[i] = wayPointParent.GetChild(i).position;
        }
        currentWayPointIndex = 0;
        currentWayPoint = wayPoints[currentWayPointIndex];
        this.isLoop = isLoop;
        isFinish = false;
    }
}
