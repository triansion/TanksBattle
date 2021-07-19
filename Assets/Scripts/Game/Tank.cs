using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
    // Start is called before the first frame update
    private CameraFollow cameraFollow = null;

    private Transform turret = null;
    void Start()
    {
        turret = transform.Find("TankRenderers/TankTurret");
        cameraFollow = Camera.main.transform.GetComponent<CameraFollow>();
    }

    private float moveSpeed = 10f;
    private float unitRotationAngle = 30f;
    private float rotationAngle = 0f;

    
    // Update is called once per frame
    void Update()
    {
        //简单地控制坦克移动:上下左右键控制坦克移动的方向,每次按下按钮时相对应方向旋转并移动一个单位
        // if(Input.GetKeyDown(KeyCode.UpArrow))
        // {
        //     transform.eulerAngles = new Vector3(0,0,0);
        //     transform.position += transform.forward * moveSpeed;
        // }
        // else if(Input.GetKeyDown(KeyCode.DownArrow))
        // {
        //     transform.eulerAngles = new Vector3(0,180,0);
        //     transform.position += transform.forward * moveSpeed;
        // }
        // else if(Input.GetKeyDown(KeyCode.LeftArrow))
        // {
        //     transform.eulerAngles = new Vector3(0,-90,0);
        //     transform.position += transform.forward * moveSpeed;
        // }
        // else if(Input.GetKeyDown(KeyCode.RightArrow))
        // {
        //     transform.eulerAngles = new Vector3(0,90,0);
        //     transform.position += transform.forward * moveSpeed;
        // }

        //另一种控制方案:水平方向的按钮(左右方向上)控制坦克旋转;竖直方向的按钮(上下方向上)控制坦克前后移动
        rotationAngle = Input.GetAxis("Horizontal") * unitRotationAngle * Time.deltaTime;
        transform.Rotate(0,rotationAngle,0,Space.World);
        transform.position += Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime * transform.forward;
        if(turret != null)
            rotateTurret();
    }
    private void rotateTurret()
    {
        // turret.rotation = new Quaternion(0,Camera.main.transform.rotation.y,0,turret.rotation.w);
        //由于使用Quaternion.Euler设置当前物体的旋转时,参数为正数时表示逆着旋转轴的正方向看过去顺时针方向为正方向,因此要将从cameraFollow中获取到的旋转角度取相反数才能正确旋转炮塔
        //另外,修改rotation时最好使用Quaternion.Euler来进行设置比较好,不要直接使用new Vector4(x,y,z,w)这样的方式去设置.
        turret.rotation = Quaternion.Euler(0f,-cameraFollow.FollowAngleInHorizontal,0);
    }
}
