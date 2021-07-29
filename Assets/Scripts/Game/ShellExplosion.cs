using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    [Tooltip("用于区分炮弹爆炸影响的目标物体层级,这里应该设置为'Tank'")]
    /// <summary>
    /// 用于区分炮弹爆炸影响的物体
    /// </summary>
    public LayerMask targetLayerMask;

    [Tooltip("爆炸音效源组件")]
    /// <summary>
    /// 爆炸音效源组件
    /// </summary>
    public AudioSource explosionAudioSource = null;

    [Tooltip("爆炸粒子特效")]
    /// <summary>
    /// 爆炸粒子特效
    /// </summary>
    public ParticleSystem explosionEffect = null;

    [Tooltip("炮弹最长存在时间")]
    /// <summary>
    /// 炮弹最长存在时间
    /// </summary>
    public float maxLifeTime = 2f;

    public float explosionRadius = 5f;

    public float maxExplosionDamage = 100f;

    public float explosionForce = 1000f;

    // public int launcherID = -1;
    public GameObject launcher;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,maxLifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        // if(other.transform.tag == "Player")
        //     return;
        if(other.gameObject == launcher)
            return;

        Collider[] colliders = Physics.OverlapSphere(transform.position,explosionRadius,targetLayerMask);

        for(int i = 0;i < colliders.Length;i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if(targetRigidbody == null)
                continue;

            targetRigidbody.AddExplosionForce(explosionForce,transform.position,explosionRadius);

            Tank targetTank = targetRigidbody.transform.GetComponent<Tank>();
            if(targetTank == null)
                continue;

            float damage = CalculateDamage(targetRigidbody.transform.position);
            targetTank.BeAttacked(damage,launcher);
        }

        explosionEffect.transform.parent = null;

        explosionEffect.Play();

        explosionAudioSource.Play();

        // exlosionEffect.main.duration
        Destroy(explosionEffect.gameObject,explosionEffect.main.duration);

        Destroy(gameObject);
    }

    private float CalculateDamage(Vector3 targetPosion)
    {
        float relativeDistance = (targetPosion - transform.position).magnitude;

        float damage = ((explosionRadius - relativeDistance) / explosionRadius) * maxExplosionDamage;

        return damage;
    }
}
