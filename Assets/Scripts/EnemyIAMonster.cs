using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using Jam.Events;

public class EnemyIAMonster : MonoBehaviour
{
    public GameRules gameRules;
    public Animator animator;

    public NavMeshAgent agent;

    public Transform player;
    public Transform monsterBody;

    public LayerMask whatIsGround, whatIsPlayer;

    //Properties
    [Header("Properties")]
    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    
    //Stunned
    public float stunnedTimeLimit;
    public float stunAmountLimit;
    public bool stunned = false;

    private float stunnedTime = 0f;
    private float stunAmount = 0f;
    private bool isBeingFocused = false;

    //Speed
    [Header("Velocity")]
    public float patrollingSpeed;
    public float chasingSpeed;

    //Patrolling
    [Header("Patrolling")]
    public Vector3 walkPoint;   
    public float walkPointRange;

    public Vector3[] staticWalkPoints;
    public Vector3 lastStaticPoint = Vector3.zero;
    public float timeToNextStaticPointLimit;
    private float counterTNSP;

    [SerializeField] bool walkPointSet;

    //Attacking
    [Header("Attacking")]
    public float timeBetweenAttacks;

    bool alreadyAttacked;

    //Chasing
    Vector3 lastPlayerPos = Vector3.zero;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        EnemyFocusEvent.EnemyFocus += beingFocused;
    }

    private void Update()
    {

        if(gameRules.isPause)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
        }
        
            //Check if player is on sight and attack range
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
            if (!stunned && !gameRules.isPause)
            {
                if (!playerInSightRange && !playerInAttackRange) patrolling();
                if (playerInSightRange && !playerInAttackRange) chasePlayer();
                if (playerInSightRange && playerInAttackRange) attackPlayer();

                // Change Speed and Animation
                if (agent.velocity.magnitude > 0 && !playerInSightRange)
                {
                    agent.speed = patrollingSpeed;
                    animator.SetFloat("running", 0.5f);
                }
                else if (agent.velocity.magnitude > 0 && playerInSightRange)
                {
                    agent.speed = chasingSpeed;
                    animator.SetFloat("running", 1f);
                }
                else
                {
                    animator.SetFloat("running", 0f);
                }


                //Reset focusAmount if exited
                if (!isBeingFocused && stunAmount > 0) stunAmount = 0;

            }
            else
            {
                animator.SetBool("stunned", true);
                agent.isStopped = true;

                stunnedTime += Time.deltaTime;
                if (stunnedTime > stunnedTimeLimit)
                {
                    stunned = false;
                    agent.isStopped = false;
                    stunnedTime = 0f;
                    animator.SetBool("stunned", false);
                }
            }

    }

    private void patrolling()
    {
        if (!walkPointSet) searchWalkPoint();

        //Set Patrolling Point to lastPlayerPos
        if (lastPlayerPos != Vector3.zero)
        {
            walkPointSet = true;
            walkPoint = lastPlayerPos;
            lastPlayerPos = Vector3.zero;
        }

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        if (counterTNSP > timeToNextStaticPointLimit) changeStaticPointToGo(2);

        counterTNSP += Time.deltaTime;
        Debug.Log("Point: "+counterTNSP);
    }

    private void changeStaticPointToGo(int maxRand)
    {
        int index = Random.Range(0, staticWalkPoints.Length - 1);

        if (lastStaticPoint == staticWalkPoints[index] && maxRand > 0) changeStaticPointToGo(maxRand-1);

        lastStaticPoint = staticWalkPoints[index];
        walkPoint = lastStaticPoint;
        walkPointSet = true;
        counterTNSP = 0f;
    }

    private void searchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y + 0.7f, transform.position.z + randomZ);
        Debug.DrawRay(walkPoint, -transform.up);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;

    }

    private void chasePlayer()
    {
        agent.SetDestination(player.position);
        lastPlayerPos = player.position;
    }

    private void attackPlayer()
    {
        agent.SetDestination(transform.position);
        //transform.LookAt(player);

        if (!alreadyAttacked)
        {
            /// Attack code here
            /// 
            /// END
            //animator.SetBool("attacking", true);
            EnemyAttackEvent.InvokeGameOver();
            EnemyAttackEvent.InvokeEnemyAttack(monsterBody);
            float defaultx = transform.rotation.x;


            this.transform.LookAt(player);

            alreadyAttacked = true;
            Invoke(nameof(resetAttack), timeBetweenAttacks);
        }
    }

    private void resetAttack()
    {
        alreadyAttacked = false;
    }

    private void beingFocused(float amount)
    {
        isBeingFocused = true;
        stunAmount += amount;

        if (stunAmount > stunAmountLimit)
        {
            stunAmount = 0f;
            stunned = true;
        }
        agent.speed = patrollingSpeed/2f;
    }

    private void exitFocus()
    {
        isBeingFocused = false;
    }
}
