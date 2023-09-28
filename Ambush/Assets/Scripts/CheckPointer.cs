using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class CheckPointer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform enemy;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private LayerMask pointerHitMask;
    [SerializeField] private LayerMask EnemyHitMask;
    [SerializeField] private AudioClip onEnemyPointedSound;
    [SerializeField] private AudioClip GunShot;
    [SerializeField] private AudioClip gunReload;
    [SerializeField] private GameObject listenerObject;

    private float distanceInPercent;
    private float audioMixerVolume;
    [SerializeField] private bool EnemyPointSoundActivated;

    private Coroutine CheckPointerCor;

    private RaycastHit hit;

    public event EventHandler OnShotMissed;



    [SerializeField] private float gunReloadTime;
    private bool canShoot;

    private void Start()
    {
        canShoot = true;
        CheckPointerCor = StartCoroutine(SetVolumeBasedOnPosition());
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


    public IEnumerator SetVolumeBasedOnPosition()
    {
        while(true)
        {
            yield return new WaitForSeconds(0f);
            Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 1300f, pointerHitMask);
            if(hit.transform != null)
            {
                listenerObject.transform.position = hit.point;
                // float dist = Vector3.Distance(hit.point, enemy.transform.position);
                // distanceInPercent = 1f / dist;
                // audioMixerVolume = Mathf.Clamp(Mathf.Abs((0.0001f - 1) * distanceInPercent + 0.0001f), 0.0001f, 1);
                // audioMixer.SetFloat("EnemyVolume", Mathf.Log10(audioMixerVolume) * 20f);

               // if(dist < 3)
                {
                    Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit enemyHit, 1000f, EnemyHitMask);
                    if(enemyHit.transform != null)
                    {
                        if(!EnemyPointSoundActivated)
                        {
                            AudioSource.PlayClipAtPoint(onEnemyPointedSound, listenerObject.transform.position, 0.15f);
                            EnemyPointSoundActivated = true;
                        }
                    }
                    else
                    {
                        EnemyPointSoundActivated = false;
                    }
                }
            }
        }
    }

    public void Shoot()
    {
        if(canShoot)
        {
            canShoot = false;
            StartCoroutine(GunReload());
            AudioSource.PlayClipAtPoint(GunShot, listenerObject.transform.position, 0.15f);
            RaycastHit hit;
            Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 1300f, EnemyHitMask);
            if(hit.collider != null)
            {
                EnemyController enemyController;
                if(hit.transform.gameObject.GetComponent<EnemyController>() != null)
                {
                    
                    enemyController = hit.transform.gameObject.GetComponent<EnemyController>();
                    enemyController.Die();
                    EnemyPointSoundActivated = false;

                    
                }
                else
                {
                    OnShotMissed?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                OnShotMissed?.Invoke(this, EventArgs.Empty);
            } 
        }
        
    }

    public IEnumerator GunReload()
    {
        yield return new WaitForSeconds(gunReloadTime - 1);
        AudioSource.PlayClipAtPoint(gunReload, listenerObject.transform.position, 0.25f); 
        yield return new WaitForSeconds(1);
        canShoot = true;
    }
}
