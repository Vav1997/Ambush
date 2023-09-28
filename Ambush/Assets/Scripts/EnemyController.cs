using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public enum EnemyType {WatchTower, Moving, General, WrongTarget}

    public EnemyType enemyType;
    private Animator anim;
    private CapsuleCollider capsuleCollider;
    [SerializeField] private GameObject soundGo;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float stoppingTime;
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float rotationSpeed;
    private int currentWaypos;
    private Coroutine PatrolCor;
    private AudioSource enemyAudio;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        enemyAudio = GetComponentInChildren<AudioSource>();
    }
    private void Start()
    {
        currentWaypos = 0;
        if(enemyType == EnemyType.Moving && waypoints.Length > 0)
        {
           PatrolCor = StartCoroutine(Patol());
        }

        if(enemyType == EnemyType.General || enemyType == EnemyType.WrongTarget)
        {
            anim.SetTrigger("Sit");
        }
    }


    public void Die()
    {
        if(PatrolCor != null)
        {
            StopCoroutine(PatrolCor);
        }
        
        capsuleCollider.enabled = false;
        soundGo.SetActive(false);
        anim.SetTrigger("Die");
        MissionManager.instance.GetActiveMission().OnEnemyEliminated(this);
        StartCoroutine(DieDelay());

    }


    public IEnumerator DieDelay()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }


    public void ChangeAudioSourceDistance(float newDist)
    {
        enemyAudio.maxDistance = newDist;
    }

    public IEnumerator Patol()
    {
        anim.SetBool("Walk", true);

        while(true)
        {
            if ((Vector3.Distance(waypoints[currentWaypos].transform.position, transform.position) < stoppingDistance))
            {
                anim.SetBool("Walk", false);
                yield return new WaitForSeconds(stoppingTime);
                anim.SetBool("Walk", true);
                currentWaypos++;
                if (currentWaypos >= waypoints.Length)
                {
                    currentWaypos = 0;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypos].transform.position, Time.deltaTime * walkSpeed);


            // Calculate the rotation towards the current waypoint
            Vector3 movementDir = waypoints[currentWaypos].transform.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(movementDir, transform.up);

            // Gradually rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }

    public void RunToDestination(Transform destination)
    {
        anim.SetTrigger("Run");
        StartCoroutine(RunToDest(destination));

    }

    public IEnumerator RunToDest(Transform destination)
    {
        while((Vector3.Distance(transform.position, destination.position)  > stoppingDistance))
        {
            transform.position = Vector3.MoveTowards(transform.position, destination.position, Time.deltaTime * runSpeed);
            Vector3 movementDir = destination.position - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(movementDir, transform.up);

            // Gradually rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            yield return null;
        }
    }
}
