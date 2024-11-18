
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyAi : MonoBehaviour
{
    public enum State
    {
        PTROL = 0, TRACE, ATTACK, DIE
    }
    public State state = State.PTROL;
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Animator animator;
    private EnemyMoveAgent moveAgent;
    private EnemyAttack enemyAttack;
    public EnemyDamage enemyDamage;

    public float attackDist = 3.0f;
    public float traceDist = 10.0f;
    public bool isDie = false;//사망여부 판단

    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int hashDie = Animator.StringToHash("DieTrigger");
    
    private readonly int hashOffset = Animator.StringToHash("OffSet");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");

    public PlayerDamage playerDamage;

    void Start()
    {
        moveAgent = GetComponent<EnemyMoveAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        enemyDamage = GetComponent<EnemyDamage>();
        animator = GetComponent<Animator>();
        enemyTr = transform;
        playerTr = GameObject.FindWithTag("Player").transform;
        //animator.SetFloat(hashOffset, 1f);
        //animator.SetFloat(hashWalkSpeed, 1f);
        StartCoroutine(CheckState());
        StartCoroutine(Action());


    }
    
    void Update()
    {
        if (playerDamage.isPlayerDie) return;
        {
            
        }
        animator.SetFloat(hashSpeed, moveAgent.speed);
        

    }
    IEnumerator CheckState()
    {
        yield return new WaitForSeconds(0.3f);
        if (playerDamage.isPlayerDie) yield break;
        while (!isDie)
        {
            if (state == State.DIE) yield break;
            float dist = (playerTr.position - enemyTr.position).magnitude;
            if (dist <= attackDist)
            {
                    state = State.ATTACK;
            }
            else if (dist <= traceDist)
            {
                    state = State.TRACE;
            }
            else
            {
                state = State.PTROL;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            switch (state)
            {
                case State.PTROL:
                    GetComponent<Rigidbody>().isKinematic = false;
                    enemyAttack.isAttack = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    GetComponent<Rigidbody>().isKinematic = true;
                    if (enemyAttack.isAttack == false)
                        enemyAttack.isAttack = true;
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);

                    break;
                case State.TRACE:
                    GetComponent<Rigidbody>().isKinematic = false;
                    enemyAttack.isAttack = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.DIE:
                   
                   
                   EnemyStateDie();
                    break;                
            }
        }
    }
    public void EnemyStateDie()
    {
        animator.SetTrigger(hashDie);
        enemyAttack.isAttack = false;
        GetComponent<Rigidbody>().isKinematic = false;
        animator.SetBool(hashMove, false);
        isDie = true;

        moveAgent.Stop();
        StartCoroutine(DieCoolTime());
    }

    IEnumerator DieCoolTime()
    {
        
        yield return new WaitForSeconds(4.3f);
        Destroy(gameObject);
    }
   
}
