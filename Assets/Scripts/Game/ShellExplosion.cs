using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public AudioSource explosionAudioSource = null;
    public ParticleSystem explosionEffect = null;
    public float maxLifeTime = 2f;
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
        if(other.transform.tag == "Player")
            return;

        explosionEffect.transform.parent = null;

        explosionEffect.Play();

        explosionAudioSource.Play();

        // exlosionEffect.main.duration
        Destroy(explosionEffect.gameObject,explosionEffect.main.duration);

        Destroy(gameObject);
    }
}
