using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    private readonly int hashAttack = Animator.StringToHash("AttackTrigger");
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemyTr;
    private readonly float damping = 10.0f;
    public bool isAttack = false;

    private float lastAttackTime;
     private float attackCooldown = 2.0f;

    [SerializeField] private BoxCollider attackPos;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown; 
        
    }

    void Update()
    {
       
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (isAttack)
            {
                Attack();
                lastAttackTime = Time.time; 
            }
           
        }
    }

    private void Attack()
    {
       
        animator.SetTrigger(hashAttack);
        //if (attackClip != null)
        //{
        //    AudioSource.PlayClipAtPoint(attackClip, transform.position);
        //}
        
        
    }
    void BoxColEnable()
    {
        attackPos.enabled = true;
        //meshRenderer.enabled = true;


    }
    void BoxColDisable()
    {
        attackPos.enabled = false;
        //meshRenderer.enabled = true;


    }
}
