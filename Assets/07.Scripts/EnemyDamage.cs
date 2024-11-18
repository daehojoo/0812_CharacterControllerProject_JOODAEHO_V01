using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyDamage : MonoBehaviour
{
   
    [SerializeField] private float Hp = 0f;
   


    void Start()
    {
        
        Hp = 100f;
        
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("PlayerAttack"))
    //    {
    //        Debug.Log("hit");
    //        Hp -= 25;
    //        Hp = Mathf.Clamp(Hp, 0f, 100f);
    //        if (Hp <= 0f)
    //        {
    //            Die(); 
    //        }
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerAttack"))
        {
            Debug.Log("hit");
            Hp -= 25;
            Hp = Mathf.Clamp(Hp, 0f, 100f);
            if (Hp <= 0f)
            {
                Die();
            }
        }
    }
    void Die()
    {
        Debug.Log("»ç¸Á!");
        GetComponent<EnemyAi>().state = EnemyAi.State.DIE;
    }
}
