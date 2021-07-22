using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CtrlType
{
    none = 0,
    player,
    AI
}


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
    [Tooltip("当前坦克的控制类型")]
    /// <summary>
    /// 当前坦克的控制类型
    /// </summary>
    public CtrlType ctrlType = CtrlType.player;

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

    [Tooltip("坦克爆炸粒子特效预制体")]
    /// <summary>
    /// 坦克爆炸粒子特效预制体
    /// </summary>
    public ParticleSystem tankExplosionEffctPrefab = null;

    [Tooltip("坦克爆炸音效")]
    /// <summary>
    /// 坦克爆炸音效
    /// </summary>
    public AudioClip tankExplosionAudioClip = null;

    /// <summary>
    /// 前后方向上的输入增量
    /// </summary>
    private float moveInputValue = 0f;

    /// <summary>
    /// 左右方向上的输入增量
    /// </summary>
    private float turnInputValue = 0f;

    /// <summary>
    /// 相机跟随类对象
    /// </summary>
    private CameraFollow cameraFollow = null;

    /// <summary>
    /// 炮塔的Transform组件对象
    /// </summary>
    private Transform turret = null;

    /// <summary>
    /// 初始音频播放速度
    /// </summary>
    private float originPitch = 0f;
    /// <summary>
    /// 音频播放速度变化范围
    /// </summary>
    private float pitchRange = 0.2f;

    private float maxHp = 100f;
    private float currentHp = 100f;
    private bool isDead = false;

    private ParticleSystem explosionEffect = null;

    void Start()
    {
        turret = transform.Find("TankRenderers/TankTurret");

        cameraFollow = Camera.main.transform.GetComponent<CameraFollow>();
        // audioSource = transform.GetComponent<AudioSource>();
        if(audioSource != null)
            originPitch = audioSource.pitch;

        explosionEffect = Instantiate<ParticleSystem>(tankExplosionEffctPrefab);
        explosionEffect.transform.SetParent(transform);
        explosionEffect.gameObject.SetActive(false);

        if(turret != null)
            launchPos = turret.GetChild(0);
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
        PlayerControl();
        ControlWheelToMove();
        EngineAudio();
    }

    private Vector3 turretDir;
    private Vector3 gunDir;
    private Transform launchPos;
    private Quaternion turretRotateAngle;
    private Quaternion gunRotateAngle;

    /// <summary>
    /// 根据摄像机视野转向控制炮塔转向
    /// </summary>
    private void RotateTurret()
    {
        // turret.rotation = new Quaternion(0,Camera.main.transform.rotation.y,0,turret.rotation.w);
        //由于使用Quaternion.Euler设置当前物体的旋转时,参数为正数时表示逆着旋转轴的正方向看过去顺时针方向为正方向,因此要将从cameraFollow中获取到的旋转角度取相反数才能正确旋转炮塔
        //另外,修改rotation时最好使用Quaternion.Euler来进行设置比较好,不要直接使用new Vector4(x,y,z,w)这样的方式去设置.
        if(turret == null)
            return;

        // turret.rotation = Quaternion.Euler(0f,-cameraFollow.FollowAngleInHorizontal,0);

        // turret.GetChild(0).transform.localEulerAngles = new Vector3(cameraFollow.FollowAngleInVertical,0,0);

        turretDir = raycastHitPos - turret.transform.position;
        gunDir = raycastHitPos - launchPos.position;
        turretRotateAngle = Quaternion.LookRotation(turretDir);
        gunRotateAngle = Quaternion.LookRotation(gunDir);
        turret.rotation = turretRotateAngle;
        launchPos.rotation = gunRotateAngle;
    }
    
    /// <summary>
    /// 玩家控制坦克前进后退以及左右转向
    /// </summary>
    private void PlayerControl()
    {
        if(ctrlType != CtrlType.player)
            return;

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
        CalculateTargetSignPos();
        RotateTurret();
    }

    /// <summary>
    /// 通过控制车轮碰撞器从而控制坦克前进后退或转向
    /// </summary>
    private void ControlWheelToMove()
    {
        //遍历车轴信息列表,为所有的车轮碰撞器设置相关状态.主要通过设置车轮碰撞器的motorTorque、steerAngle和brakeTorque来实现对坦克移动转向的控制。其中motorTorque表示作用在该车轮上的电机力矩(马力);steerAngle表示该车轮当前的转向角度;brakeTorque表示作用在该车轮的当前的制动力矩.
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

    /// <summary>
    /// 播放发动机音效,坦克静止或玩家没有控制坦克时播放发动机默认音效;否则播放发动机运转音效
    /// </summary>
    private void EngineAudio()
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

    public void BeAttacked(float amount)
    {
        if(currentHp > 0)
            currentHp -= amount;

        if(currentHp <= 0 && !isDead)
            OnDead();
    }

    private void OnDead()
    {
        isDead = true;

        // ParticleSystem explosionEffect = Instantiate<ParticleSystem>(tankExplosionEffctPrefab);
        // explosionEffect.transform.SetParent(transform);
        // explosionEffect.transform.SetParent(null);
        explosionEffect.transform.position = transform.position;
        explosionEffect.gameObject.SetActive(true);
        // Destroy(explosionEffect,explosionEffect.main.duration);

        explosionEffect.Play();

        audioSource.clip = tankExplosionAudioClip;
        audioSource.Play();

        ctrlType = CtrlType.none;
        
        // yield return new WaitForSeconds(tankExplosionAudioClip.length);
        // gameObject.SetActive(false);
    }

    private Ray screenRay;
    private RaycastHit screenRaycastHit;
    private int maxRayCastDistance = 400;
    private Vector3 raycastHitPos;
    private void CalculateTargetSignPos()
    {
        screenRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2,Screen.height/2,0));
        if(Physics.Raycast(screenRay,out screenRaycastHit,maxRayCastDistance))
        {
            // Debug.Log("碰撞体名称:"+screenRaycastHit.collider.gameObject.name);
            // Debug.Log("碰撞点位置:"+screenRaycastHit.point);
            Debug.DrawLine(screenRay.origin,screenRaycastHit.point,Color.red);
            raycastHitPos = screenRaycastHit.point;
        }
        else
        {
            Debug.DrawLine(screenRay.origin,screenRay.GetPoint(maxRayCastDistance),Color.red);
            raycastHitPos = screenRay.GetPoint(maxRayCastDistance);
        }
    }
}
