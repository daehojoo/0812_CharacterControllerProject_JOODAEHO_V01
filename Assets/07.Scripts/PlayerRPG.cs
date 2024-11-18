using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRPG : MonoBehaviour
{
    public enum PlayerState { IDLE = 0, ATTACK, UNDER_ATTACK, DEAD}
    public PlayerState playerState = PlayerState.IDLE;
    [Tooltip("�ȱ� �ӵ�")] public float walkSpeed = 5f;
    [Tooltip("�޸��� �ӵ�")] public float runSpeed = 10f;
    [Header("Camera ���� ����")]
    [SerializeField] private Transform cameraTr;//ī�޶� ��ġ
    [SerializeField] private Transform cameraPivotTr;//ī�޶� �ǹ� ��ġ
    [SerializeField] private float cameraDistance = 0f;//ī�޶���� �Ÿ�
    [SerializeField] private Vector3 mouseMove = Vector3.zero;//���콺 �̵� ��ǥ
    [SerializeField] private int playerLayer;//�÷��̾� ���̾�
    [Header("Player Move ���� ����")]
    [SerializeField] private Transform modelTr;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Vector3 moveVelocity = Vector3.zero;//������ ����
    [SerializeField] private bool isGrounded = false;
    


    private readonly int hashShieldAttack = Animator.StringToHash("attackShieldTrigger");
    private readonly int hashAttack = Animator.StringToHash("attackTrigger");
    private readonly int hashSpeedX = Animator.StringToHash("SpeedX");
    private readonly int hashSpeedY = Animator.StringToHash("SpeedY");

    [SerializeField] private bool isRun = false;
   
    public bool IsRun
    {
        get { return isRun; }
        set 
        { 
            isRun = value;
            animator.SetBool("isRun", value);
        }
    }

 
    void Start()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = Camera.main.transform.parent;
        playerLayer = LayerMask.NameToLayer("PLAYER");
        //modelTr = GetComponentsInChildren<Transform>()[1];
        animator = transform.GetChild(0).GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraDistance = 5f;//ī�޶��� �Ÿ�
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        animator.SetBool("isGrounded", true);
    }

    void Update()
    {
        CameraDistanceCtrl();
        FreezeXZ();
        switch (playerState)
        { 
            case PlayerState.IDLE:
                PlayerIdleAndMove();
                break;
            case PlayerState.ATTACK:
                AttackTimeState();
               
                break;
            case PlayerState.UNDER_ATTACK:

                break;
            case PlayerState.DEAD:

                break;
                
        }
        

        
    }
    private void LateUpdate() //���⼭ ī�޶� ����
    {
        float cameraHeight = 1.3f;
        cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);//ī�޶� ���̸� ���� ���� ��ġ��
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 100f * 0.1f, Input.GetAxisRaw("Mouse X") * 100f* 0.1f , 0f);
        //���콺 ���Ϸ� ī�޶�� x��ȸ��, ���콺 �¿�� ī�޶�� y�� ȸ�� ����Ƽ ��ǥ�� x = ���� y = �������� z = �����
        if (mouseMove.x < -40f)
            mouseMove.x = -40f;
        else if (mouseMove.x > 40f)
            mouseMove.x = 40f;     
        cameraPivotTr.eulerAngles = mouseMove;
        RaycastHit hit;
        //���� ī�޶� ��ġ���� - ī�޶� �ǹ� ��ġ�� ���� ������ ���Ѵ�.
        Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;
        Ray ray = new Ray(cameraTr.position, dir);
        Debug.DrawRay(ray.origin, -ray.direction * 30f, Color.red);
        if (Physics.Raycast(cameraPivotTr.position, dir, out hit, cameraDistance, ~(1 << playerLayer)))
            cameraTr.localPosition = Vector3.back * hit.distance;
        else 
            cameraTr.localPosition = Vector3.back * cameraDistance;

    }
    void PlayerIdleAndMove()
    {
        RunCheck();
        if (characterController.isGrounded)
        {
            if (isGrounded == false)
            {
                isGrounded = true;
            }
            animator.SetBool("isGrounded", true);
            CalcInputMove();//�������� ���
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))
            {
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;
                //�׶��忡 ��Ҵٸ� �����ǵ峪 ��ũ���ǵ常ŭ �־ �绡��
                //�ٽ� ã���ϰ� ���� �������� �ϴٸ�                 
            }
            else
                moveVelocity.y = -1f;
            PlayerAttack();
        }
        else
        {
            if (isGrounded == false) isGrounded = true;
            else
                animator.SetBool("isGrounded", false);
            moveVelocity += Physics.gravity * Time.deltaTime;
        }
        characterController.Move(moveVelocity * Time.deltaTime);

    }

    void RunCheck()
    {
        if (IsRun == false && Input.GetKey(KeyCode.LeftShift)&&Input.GetKey(KeyCode.W))
        {
            IsRun = true;
        }
        else if (IsRun == true && Input.GetKeyUp(KeyCode.LeftShift))
        { 
            IsRun =false;
        
        }
        else if (IsRun == true && Input.GetKeyUp(KeyCode.W))
        {
            IsRun = false;

        }
        else if (IsRun == true && Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            IsRun = false;
        }
        
    }
    void CalcInputMove()
    {
        moveVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * (IsRun ? runSpeed : walkSpeed);
        animator.SetFloat(hashSpeedX, Input.GetAxis("Horizontal"));
        animator.SetFloat(hashSpeedY, Input.GetAxis("Vertical"));
        moveVelocity = transform.TransformDirection(moveVelocity); //������ǥ(������ǥ)�� �ݴ��� ���� InverseTransformDirection �̴�
        if (0.01f < moveVelocity.sqrMagnitude)
        {
            Quaternion cameraRot = cameraPivotTr.rotation; //���ʹϾ� ������ ��������
            cameraRot.x = cameraRot.z = 0f; //��ǥ�� ��ȯ
            transform.rotation = cameraRot; //�����̰� ���� �� ī�޶� ȸ�� ����
            if (IsRun)
            {
                Quaternion characterRot = Quaternion.LookRotation(moveVelocity);
                characterRot.x = characterRot.z = 0f;
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, characterRot, Time.deltaTime * 10f);
            }
            else
            {
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, cameraRot, Time.deltaTime * 10f);                        
            }
        }
    }
    void CameraDistanceCtrl()//ī�޶� �Ÿ�����
    {
        cameraDistance -= Input.GetAxisRaw("Mouse ScrollWheel")*5f;        
    }
    void FreezeXZ()//ĳ���� ��Ʈ�ѷ��� ȸ�� ���� x�� ȸ���� z�� ȸ��
    { 
        transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);           
    }
    bool GroundCheck(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.1f);
    }
    private float nextTime = 0f;
    void AttackTimeState()
    { 
        nextTime += Time.deltaTime;
        if (1f <= nextTime)
        {
            nextTime += Time.deltaTime;
            playerState = PlayerState.IDLE;
        }    
    }

    void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            playerState = PlayerState.ATTACK;
            animator.SetTrigger(hashAttack);
            animator.SetFloat(hashSpeedX, 0f);
            animator.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            playerState = PlayerState.ATTACK;
            animator.SetTrigger(hashShieldAttack);
            animator.SetFloat(hashSpeedX, 0f);
            animator.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Wall"))
            Debug.Log($"Wall{hit.point}");



    }
   






}
