using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMoveAgent : MonoBehaviour
{
    [SerializeField] private Transform enemyTr;
    [SerializeField] private NavMeshAgent agent;
    public List<Transform> wayPointList;
    public int nexIdx = 0;    
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 3.0f;
    private float damping = 1.0f;
    
    private bool _patrolling;

    public bool patrolling//프로퍼티
    {
        get { return _patrolling; }
        set
        {
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MovewayPoint();
            }
        }
    }
    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }
    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    void Start()
    {
        enemyTr = transform;
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.updateRotation = false;
        var group = GameObject.Find("Paths");
        if (group != null)
        {           
            group.GetComponentsInChildren<Transform>(wayPointList);
            wayPointList.RemoveAt(0);
        }
        nexIdx = UnityEngine.Random.Range(0, wayPointList.Count);
        MovewayPoint();
    }
    void Update()
    {
        if (agent.isStopped == false)
        {                   
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }
        if (_patrolling == false) return;

        //다음 도착지점이 0.5보다 작거나 같다면
        //float dist = Vector3.Distance(transform.position, wayPointList[nexIdx].position);

        if (agent.remainingDistance <= 0.5f)
        {
            nexIdx = UnityEngine.Random.Range(0, wayPointList.Count);
            nexIdx = ++nexIdx % wayPointList.Count;
            MovewayPoint();
        }
    }
    void MovewayPoint()
    {
        if (agent.isPathStale) return;
        agent.destination = wayPointList[nexIdx].position;
        agent.isStopped = false;
    }
    private void TraceTarget(Vector3 pos)
    {
        if (agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}
