using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo//车轴信息类:车的前后轴
{
    [Tooltip("左轮车轮碰撞体")]//使用[Header("xxx")]可以使该属性直接在属性面板中显示"xxx"提示信息;使用[Tooltip("xxx")]可以使该属性在属性面板中当鼠标停留在该属性上使显示"xxx"提示信息
    /// <summary>
    /// 左轮车轮碰撞体
    /// </summary>
    public WheelCollider leftWheel;

    [Tooltip("右轮车轮碰撞体")]
    /// <summary>
    /// 右轮车轮碰撞体
    /// </summary>
    public WheelCollider rightWheel;

    [Tooltip("发动机是否将动力传送给该轴上的车轮,例如一般家用车大多数都是后轮驱动,那么定义后轮车轴信息类对象时就需要将此属性设为true")]
    /// <summary>
    /// 发动机是否将动力传送给该轴上的车轮,例如一般家用车大多数都是后轮驱动,那么定义后轮车轴信息类对象时就需要将此属性设为true
    /// </summary>
    public bool isMotor;

    [Tooltip("该轴是否可以控制转向,例如一般家用车大多数都是前轮控制转向,那么定义前轮车轴信息类对象时就需要将此属性设为true")]
    /// <summary>
    /// 该轴是否可以控制转向,例如一般家用车大多数都是前轮控制转向,那么定义前轮车轴信息类对象时就需要将此属性设为true
    /// </summary>
    public bool isSteering;
}

public class Tank : MonoBehaviour
{
    private float moveInputValue = 0f;

    private float turnInputValue = 0f;

    /// <summary>
    /// 相机跟随类对象
    /// </summary>
    private CameraFollow cameraFollow = null;

    /// <summary>
    /// 炮塔的Transform组件对象
    /// </summary>
    private Transform turret = null;

    [Tooltip("车轴信息类列表")]
    /// <summary>
    /// 车轴信息类列表
    /// </summary>
    public List<AxleInfo> axleInfos;

    /// <summary>
    /// 发动机马力
    /// </summary>
    private float motor = 0f;

    [Tooltip("发动机最大马力")]
    /// <summary>
    /// 发动机最大马力
    /// </summary>
    public float maxMotor = 3000f;

    /// <summary>
    /// 制动
    /// </summary>
    private float brake = 0f;

    [Tooltip("最大制动")]
    /// <summary>
    /// 最大制动
    /// </summary>
    public float maxBrake = 5000f;

    /// <summary>
    /// 转向角
    /// </summary>
    private float steeringAngle = 0f;

    [Tooltip("最大转向角")]
    /// <summary>
    /// 最大转向角
    /// </summary>
    public float maxSteeringAngle = 90f;

    /// <summary>
    /// 音频源组件
    /// </summary>
    public AudioSource audioSource = null;

    [Tooltip("坦克静止或玩家没有输入时播放的发动机音效")]
    /// <summary>
    /// 坦克静止或玩家没有输入时播放的发动机音效
    /// </summary>
    public AudioClip engineIdleClip = null;

    [Tooltip("玩家输入时播放的发动机音效")]
    /// <summary>
    /// 玩家输入时播放的发动机音效
    /// </summary>
    public AudioClip engineDriveClip = null;

    private float originPitch = 0f;
    private float pitchRange = 0.2f;

    void Start()
    {
        turret = transform.Find("TankRenderers/TankTurret");
        cameraFollow = Camera.main.transform.GetComponent<CameraFollow>();
        // audioSource = transform.GetComponent<AudioSource>();
        if(audioSource != null)
            originPitch = audioSource.pitch;
    }

    // private float moveSpeed = 10f;
    // private float unitRotationAngle = 30f;
    // private float rotationAngle = 0f;


    // Update is called once per frame
    void Update()
    {
        #region 旧版本坦克移动控制代码
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
        // rotationAngle = Input.GetAxis("Horizontal") * unitRotationAngle * Time.deltaTime;
        // transform.Rotate(0,rotationAngle,0,Space.World);
        // transform.position += Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime * transform.forward;
        // if(turret != null)
        //     rotateTurret();
        #endregion

    }

    void FixedUpdate() {
        //将坦克控制的代码分离到playerControl函数中
        playerControl();
        controlWheelToMove();
        rotateTurret();
        engineAudio();
    }

    private void rotateTurret()
    {
        // turret.rotation = new Quaternion(0,Camera.main.transform.rotation.y,0,turret.rotation.w);
        //由于使用Quaternion.Euler设置当前物体的旋转时,参数为正数时表示逆着旋转轴的正方向看过去顺时针方向为正方向,因此要将从cameraFollow中获取到的旋转角度取相反数才能正确旋转炮塔
        //另外,修改rotation时最好使用Quaternion.Euler来进行设置比较好,不要直接使用new Vector4(x,y,z,w)这样的方式去设置.
        if(turret == null)
            return;

        turret.rotation = Quaternion.Euler(0f,-cameraFollow.FollowAngleInHorizontal,0);
    }

    private void playerControl()
    {
        moveInputValue = Input.GetAxis("Vertical");
        //当玩家按下竖直方向上的按键时设置此时相应的发动机马力,其中正值表示前进马力,负值表示后退马力
        motor = maxMotor * moveInputValue;

        turnInputValue = Input.GetAxis("Horizontal");
        //当玩家按下水平方向上的按键时设置此时对应的转向角,其中正值表示右转角度,负值表示左转角度
        steeringAngle = maxSteeringAngle * turnInputValue;

        //制动
        brake = 0f;
        for(int i = 0;i < axleInfos.Count;i++)
        {
            if(axleInfos[i].rightWheel.motorTorque > 0 && motor < 0)
            {
                brake = maxBrake;
            }
            else if(axleInfos[i].rightWheel.motorTorque < 0 && motor > 0)
            {
                brake = maxBrake;
            }
        }
    }

    private void controlWheelToMove()
    {
        for(int i = 0;i < axleInfos.Count;i++)
        {
            if(axleInfos[i].isMotor)
            {
                axleInfos[i].leftWheel.motorTorque = motor;
                axleInfos[i].rightWheel.motorTorque = motor;
            }
            if(axleInfos[i].isSteering)
            {
                axleInfos[i].leftWheel.steerAngle = steeringAngle;
                axleInfos[i].rightWheel.steerAngle = steeringAngle;
            }
            axleInfos[i].leftWheel.brakeTorque = brake;
            axleInfos[i].rightWheel.brakeTorque = brake;
        }
    }

    private void engineAudio()
    {
        if(audioSource == null || engineDriveClip == null || engineIdleClip == null)
            return;

        if(Mathf.Abs(moveInputValue) >= 0.1 || Mathf.Abs(turnInputValue) >= 0.1)
        {
            if(audioSource.clip == engineIdleClip || audioSource.clip == null)
            {
                audioSource.clip = engineDriveClip;
                audioSource.pitch = Random.Range(originPitch - pitchRange, originPitch + pitchRange);
                audioSource.Play();
            }
        }
        else
        {
            if(audioSource.clip == engineDriveClip || audioSource.clip == null)
            {
                audioSource.clip = engineIdleClip;
                audioSource.pitch = Random.Range(originPitch - pitchRange, originPitch + pitchRange);
                audioSource.Play();
            }
        }
    }
}
