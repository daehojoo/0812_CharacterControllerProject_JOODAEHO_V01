using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCtrl : MonoBehaviour
{
    public float doubleClick;
   [Tooltip("더블클릭 확인용")][SerializeField]private bool isDoublceClick = false;
    NavMeshAgent agent;
    void Start()
    {
        doubleClick = Time.time;
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (isDoublceClick)
        {
            isDoublceClick = false;
        }
        RaycastHit hit;
        Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit,Mathf.Infinity))
        {
            agent.destination = hit.point;
            
        
        }






    }
    private void OnMouseUp()
    {
        if (Time.time - doubleClick < 0.25f)
        {
            isDoublceClick=true;
            doubleClick = Time.time;
            Debug.Log("DoubleClickOn");
        
        }
        else
        {
            isDoublceClick = false;
            doubleClick = Time.time;
        }

    }
    
}
