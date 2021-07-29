using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankShot : MonoBehaviour
{
    [Tooltip("带刚体的炮弹预制体")]
    /// <summary>
    /// 带刚体的炮弹预制体
    /// </summary>
    public Rigidbody shellPrefab = null;

    [Tooltip("炮弹发射音效源组件")]
    /// <summary>
    /// 炮弹发射音效源组件
    /// </summary>
    public AudioSource shotAudioSource = null;

    [Tooltip("炮弹发射音效")]
    /// <summary>
    /// 炮弹发射音效
    /// </summary>
    public AudioClip fireClip = null;

    [Tooltip("发射炮弹前蓄力音效")]
    /// <summary>
    /// 发射炮弹前蓄力音效
    /// </summary>
    public AudioClip chargingClip = null;

    [Tooltip("炮弹物体生成并发射时的初始位置的Transform组件")]
    /// <summary>
    /// 炮弹物体生成并发射时的初始位置的Transform组件
    /// </summary>
    public Transform launchPos = null;

    [Tooltip("最小的发射作用力")]
    /// <summary>
    /// 最小的发射作用力
    /// </summary>
    public float minLaunchForce = 15f;

    [Tooltip("最大的发射作用力")]
    /// <summary>
    /// 最大的发射作用力
    /// </summary>
    public float maxLaunchForce = 20f;

    [Tooltip("最长炮弹蓄力时间")]
    /// <summary>
    /// 最长炮弹蓄力时间
    /// </summary>
    public float maxChargeTime = 0.75f;

    [Tooltip("蓄力显示UI")]
    /// <summary>
    /// 蓄力显示UI
    /// </summary>
    public Slider chargeSlider = null;

    /// <summary>
    /// 当前发射作用力
    /// </summary>
    private float currentLaunchForce = 15f;

    /// <summary>
    /// 当前炮弹是否已经发射(主要是为了处理长按发射按钮时达到最长的蓄力时间后自动发射炮弹这种情况)
    /// </summary>
    private bool isFired = false;

    /// <summary>
    /// 蓄力速度
    /// </summary>
    private float chargeSpeed = 0f;

    /// <summary>
    /// 发射间隔(避免玩家疯狂点击发射造成的不合理)
    /// </summary>
    private float shotInterval = 0.5f;

    /// <summary>
    /// 最近一次发射的时间戳
    /// </summary>
    private float lastShotTime = 0f;

    private CtrlType ctrlType = CtrlType.player;

    private EnemyAI enemyAI;

    // Start is called before the first frame update
    void Start()
    {
        //设置蓄力速度
        chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        //设置蓄力显示UI的相关参数(Slider的最大最小值,以及初始时Silder的值)
        chargeSlider.minValue = minLaunchForce;
        chargeSlider.maxValue = maxLaunchForce;
        chargeSlider.value = minLaunchForce;

        ctrlType = transform.GetComponent<Tank>().ctrlType;
        if(ctrlType == CtrlType.AI)
            enemyAI = transform.GetComponent<EnemyAI>();
        else
            enemyAI = null;
    }

    // Update is called once per frame
    void Update()
    {
        shot();
    }

    /// <summary>
    /// 发射炮弹
    /// </summary>
    private void shot()
    {
        
        //若最近一次发射的时间距离当前时间小于发射间隔则不会往下处理
        if(Time.time - lastShotTime < shotInterval)
            return;
        
        //玩家控制发射炮弹
        if(ctrlType == CtrlType.player)
        {

            //若当前发射作用力已经达到最大发射作用力,并且还没有发射,则自动发射并重置蓄力显示UI
            if(currentLaunchForce >= maxLaunchForce && !isFired)
            {
                fire();

                chargeSlider.value = chargeSlider.minValue;
            }
            //当玩家按下发射按钮时,重置发射状态(isFired)、当前发射作用力(currentLaunchForce)并且播放蓄力音效
            else if(Input.GetMouseButtonDown(0))
            {
                isFired = false;

                currentLaunchForce = minLaunchForce;

                shotAudioSource.clip = chargingClip;
                shotAudioSource.Play();
            }
            //当玩家抬起发射按钮时(并且还没有发射炮弹),发射炮弹
            else if(Input.GetMouseButtonUp(0) && !isFired)
            {
                fire();
            }
            //当玩家长按发射按钮时(并且还没有发射炮弹),当前发射作用力会随时间以蓄力速度开始增大.直到达到最大作用力;同时将蓄力效果反映到UI上
            else if(Input.GetMouseButton(0) && !isFired)
            {
                currentLaunchForce += chargeSpeed * Time.deltaTime;

                chargeSlider.value = currentLaunchForce;
            }
        }
        else if(ctrlType == CtrlType.AI)
        {
            if(enemyAI.IsShouldShot())
            {
                currentLaunchForce = maxLaunchForce;
                fire();
            }
        }
    }

    /// <summary>
    /// 发射炮弹
    /// </summary>
    private void fire()
    {
        //更新发射状态
        isFired = true;

        //在指定位置以指定旋转状态生成炮弹
        Rigidbody shellInstance = Instantiate<Rigidbody>(shellPrefab,launchPos.position,launchPos.rotation);

        //为该炮弹设置一个初速度,该初始速度的大小由当前的发射作用力大小决定,方向由发射位置的前向方向决定(即炮管方向)
        shellInstance.velocity = currentLaunchForce * launchPos.forward;

        shellInstance.GetComponent<ShellExplosion>().launcher = gameObject;

        //播放发射炮弹音效
        shotAudioSource.clip = fireClip;
        shotAudioSource.Play();

        //重置当前发射作用力大小,重置蓄力显示UI
        currentLaunchForce = minLaunchForce;
        chargeSlider.value = chargeSlider.minValue;

        //更新最近一次发射的时间戳
        lastShotTime = Time.time;
    }

    public void DisableShot()
    {
        ctrlType = CtrlType.none;
    }
}
