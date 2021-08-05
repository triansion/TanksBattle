using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Transform FollowTarget = null;
    [Range(5f,10f)]
    private float followInterval = 7f;

    private float followAngleInHorizontal = 0f;
    public float FollowAngleInHorizontal {
        get {
            return followAngleInHorizontal;
        }
    }
    private float followAngleInVertical = 30f;
    public float FollowAngleInVertical {
        get {
            return followAngleInVertical;
        }
    }

    // private float distanceInX = 0f;
    private float distanceInY = 0f;
    private float distanceInZ = 0f;

    private float cameraRotateSpeed = 20f;
    private float maxAngleInVertical = 30f;
    private float minAngleInVertical = -30f;

    private float cameraZoomSpeed = 1f;
    private float maxFollowInterval = 10f;
    private float minFollowInterval = 5f;


    // Start is called before the first frame update
    void Start()
    {
        // GameObject playerTank = GameObject.Find("Tank");
        // if(playerTank != null)
        // {
        //     SetTarget(playerTank);
        //     distanceInY = followInterval * Mathf.Sin((Mathf.PI/180)*followAngleInVertical);
        //     distanceInZ = followInterval * Mathf.Cos((Mathf.PI/180)*followAngleInVertical);
        //     followToTarget();
        // }
        // if(FollowTarget != null)
        // {
        //     if(FollowTarget.Find("TankRenderers/TankTurret/Camerapoint") != null)
        //         this.FollowTarget = FollowTarget.Find("TankRenderers/TankTurret/Camerapoint");
        //     distanceInY = followInterVal * Mathf.Sin((Mathf.PI/180)*followAngleInVertical);
        //     distanceInZ = followInterVal * Mathf.Cos((Mathf.PI/180)*followAngleInVertical);
        //     followToTarget();
        // }
        
    }

    public void SetFollowTargetTank(GameObject targetTank)
    {
        if(targetTank != null)
        {
            SetTarget(targetTank);
            distanceInY = followInterval * Mathf.Sin((Mathf.PI/180)*followAngleInVertical);
            distanceInZ = followInterval * Mathf.Cos((Mathf.PI/180)*followAngleInVertical);
            followToTarget();
        }
    }

    private void SetTarget(GameObject target)
    {
        if(target.transform.Find("Camerapoint") != null)
        {
            this.FollowTarget = target.transform.Find("Camerapoint");
        }
        else
        {
            this.FollowTarget = target.transform;
        }
    }

    private void cameraRotate()
    {
        // if(Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.05)
            followAngleInHorizontal -= Input.GetAxis("Mouse X") * cameraRotateSpeed;//"Mouse X"代表鼠标横向移动增量,即鼠标向左移动时Input.GetAxis("Mouse X")返回的是负数的增量;鼠标向右移动时Input.GetAxis("Mouse X")返回的是正数的增量.因为我们希望鼠标向左移动时摄像机向左旋转,向右移动时摄像机向右旋转,又因为Unity中使用的是左手坐标系,即旋转时顺着旋转轴的正方向看过去时是顺时针为旋转时的正方向.这里我们是逆着Y轴的正方向去看,因此正方向反过来了,即逆时针为正方向,也就是说当前旋转角度的正值为向左旋转,负值为向右旋转,因此这里使用的是-=.

        // if(Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.05)
            followAngleInVertical -= Input.GetAxis("Mouse Y") * cameraRotateSpeed;

        // Debug.Log("Input.GetAxis('Mouse X')"+Input.GetAxis("Mouse X"));
        // Debug.Log("Input.GetAxis('Mouse Y')"+Input.GetAxis("Mouse Y"));

        if(followAngleInVertical >= maxAngleInVertical)
            followAngleInVertical = maxAngleInVertical;
        else if(followAngleInVertical <= minAngleInVertical)
            followAngleInVertical = minAngleInVertical;

        distanceInY = followInterval * Mathf.Sin((Mathf.PI/180)*followAngleInVertical);
        distanceInZ = followInterval * Mathf.Cos((Mathf.PI/180)*followAngleInVertical);
    }

    private void cameraZoom()
    {
        // if(this.followInterval >= minFollowInterval && this.followInterval <=maxFollowInterval)
            // this.followInterval += Input.GetAxis("Mouse ScrollWheel") * cameraZoomSpeed;
        if(Input.GetAxis("Mouse ScrollWheel") > 0)//往上滚动
        {
            if(this.followInterval < maxFollowInterval)
                this.followInterval += cameraZoomSpeed;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)//往下滚动
        {
            if(this.followInterval > minFollowInterval)
                this.followInterval -= cameraZoomSpeed;
        }
    }

    private void followToTarget()
    {
        transform.position = new Vector3(FollowTarget.position.x+distanceInZ*Mathf.Sin((Mathf.PI/180)*followAngleInHorizontal),FollowTarget.position.y+distanceInY,FollowTarget.position.z-distanceInZ*Mathf.Cos((Mathf.PI/180)*followAngleInHorizontal));

        transform.LookAt(FollowTarget);
    }

    // private void FixedUpdate() {
    //     RayTest();
    // }

    void LateUpdate()
    {
        if(FollowTarget != null)
        {
            cameraRotate();
            followToTarget();
            cameraZoom();
        }
    }

    // private Ray screenRay;
    // private RaycastHit screenRaycastHit;
    // private int maxRayCastDistance = 360;
    // private void RayTest()
    // {
        
    //     screenRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
    //     // screenRay = new Ray(transform.position,transform.forward);
    //     // screenRay = Ray()
    //     // Physics.Raycast(screenRay, maxRayCastDistance,)
    //     if(Physics.Raycast(screenRay,out screenRaycastHit,maxRayCastDistance))
    //     {
    //         Debug.Log("碰撞体名称:"+screenRaycastHit.collider.gameObject.name);
    //         // Debug.Log("碰撞点位置:"+screenRaycastHit.point);
    //         Debug.DrawLine(screenRay.origin,screenRaycastHit.point,Color.red);
    //     }
    //     else
    //     {
    //         Debug.DrawLine(screenRay.origin,screenRay.GetPoint(360),Color.red);
    //     }
    // }
}
