using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] private int hpInit = 0;
    [SerializeField] private int hpMax = 100;
   
    public bool isPlayerDie = false;
    Animator animator;

    public delegate void PlayerDieHandler();
    public static event PlayerDieHandler OnPlayerDie;//
    void Start()
    {
        hpInit = hpMax;
        animator = GetComponent<Animator>();



    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyAttack")&&!isPlayerDie)
        {
            hpInit -= 5;
        
        }
        if (hpInit <= 0)
        {
            isPlayerDie = true;
            Debug.Log("die");
            animator.SetTrigger("dieTrigger");
            OnPlayerDie();
        }
    }
    void Update()
    {
        
    }
}
