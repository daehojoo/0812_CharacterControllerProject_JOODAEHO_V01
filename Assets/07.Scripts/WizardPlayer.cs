using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WizardPlayer : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Camera")]
    [SerializeField] private Transform cameraTr;
    [SerializeField] private Transform cameraPivotTr;//ī�޶� �ǹ� ��ġ
    [SerializeField] private float cameraDistance = 10f;//ī�޶���� �Ÿ�
    [SerializeField] private Vector3 mouseMove = Vector3.zero;
    [SerializeField] private int playerLayer;


    private readonly int hashSpeed = Animator.StringToHash("MoveSpeed");

    [Header("Click")]
    public float m_doubleClickSecond = 0.25f;
    private bool m_isOneClicked = false; // �ѹ� Ŭ�� �Ǵ�
    private double m_Timer = 0d; // ��Ȯ�� �ð��̳� ������ ����ϱ� ���ؼ��� ������ ����.
    public BoxCollider attackPos;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 targetPos = Vector3.zero;
    bool attack =false;
    PlayerDamage PlayerDamage;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerLayer = LayerMask.NameToLayer("PLAYER");
        cameraTr = Camera.main.transform;
        cameraPivotTr = Camera.main.transform.parent;
        cameraDistance = 10f;
        agent.angularSpeed = 10000000f;
        PlayerDamage =GetComponent<PlayerDamage>();
    }

    void Update()
    {
        CameraDistanceCtrl();
        if (PlayerDamage.isPlayerDie) return;
        if (attack) return;
        ClickCheck();
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 30f, Color.red);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 7))
            {
                if (m_isOneClicked)
                {
                    agent.speed = 1.5f;
                    animator.SetFloat(hashSpeed, agent.speed);
                }
                else
                {
                    agent.speed = 3.0f;
                    animator.SetFloat(hashSpeed, agent.speed);
                }
                
                
                    targetPos = hit.point;
                    agent.SetDestination(targetPos);
                    agent.isStopped = false;
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetTrigger("attackTrigger");
            agent.speed = 0;
            animator.SetFloat(hashSpeed, agent.speed);
            agent.isStopped = true;
            attack = true;
            StartCoroutine(CoolTime());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetTrigger("buffTrigger");
            agent.speed = 0;
            animator.SetFloat(hashSpeed, agent.speed);
            agent.isStopped = true;
            attack = true;
            StartCoroutine(CoolTime());
        }


    }
    IEnumerator CoolTime()
    { 
    
        yield return new WaitForSeconds(1.5f);
        attack = false;



    }
    private void LateUpdate()
    {
        float heightCamera = 3f;
        cameraPivotTr.position = transform.position + (Vector3.up * heightCamera);
        //mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 100f * 0.1f, Input.GetAxisRaw("Mouse X") * 100f * 0.1f, 0f);
        //if (mouseMove.x < -30f)
        //    mouseMove.x = -30f;
        //else if (mouseMove.x > 30f)
        //    mouseMove.x = 30f;
        //cameraPivotTr.eulerAngles = mouseMove;
        RaycastHit hit;        
        Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;
        Ray ray = new Ray(cameraTr.position, dir);
        Debug.DrawRay(ray.origin, -ray.direction * 30f, Color.green);
        if (Physics.Raycast(cameraPivotTr.position, dir, out hit, cameraDistance, ~(1 << playerLayer)))
            cameraTr.localPosition = Vector3.back * hit.distance;
        else
            cameraTr.localPosition = Vector3.back * cameraDistance;
       


    }
    void CameraDistanceCtrl()//ī�޶� �Ÿ�����
    {
        cameraDistance -= Input.GetAxisRaw("Mouse ScrollWheel") * 5f;
    }
    private void ClickCheck()
    {
        
        if (m_isOneClicked && (Time.time - m_Timer) > m_doubleClickSecond)
        {
            m_isOneClicked = false;
            Debug.Log("OneClick");
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!m_isOneClicked)
            {
                m_Timer = Time.time;
                m_isOneClicked = true;
            }
            else if (m_isOneClicked && (Time.time - m_Timer) < m_doubleClickSecond)
            {
                Debug.Log("DoubleClick");
                m_isOneClicked = false;
            }
        }
        else
        {
            if (agent.remainingDistance <= 0.25f)
            {
                agent.speed = 0f;
                animator.SetFloat(hashSpeed, agent.speed);
            }
        }
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
