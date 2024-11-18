using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRPG : MonoBehaviour
{
    public enum PlayerState { IDLE = 0, ATTACK, UNDER_ATTACK, DEAD}
    public PlayerState playerState = PlayerState.IDLE;
    [Tooltip("걷기 속도")] public float walkSpeed = 5f;
    [Tooltip("달리기 속도")] public float runSpeed = 10f;
    [Header("Camera 관련 변수")]
    [SerializeField] private Transform cameraTr;//카메라 위치
    [SerializeField] private Transform cameraPivotTr;//카메라 피벗 위치
    [SerializeField] private float cameraDistance = 0f;//카메라와의 거리
    [SerializeField] private Vector3 mouseMove = Vector3.zero;//마우스 이동 좌표
    [SerializeField] private int playerLayer;//플레이어 레이어
    [Header("Player Move 관련 변수")]
    [SerializeField] private Transform modelTr;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Vector3 moveVelocity = Vector3.zero;//움직임 방향
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
        cameraDistance = 5f;//카메라의 거리
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
    private void LateUpdate() //여기서 카메라 조정
    {
        float cameraHeight = 1.3f;
        cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);//카메라 높이를 가슴 높이 위치로
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 100f * 0.1f, Input.GetAxisRaw("Mouse X") * 100f* 0.1f , 0f);
        //마우스 상하로 카메라는 x축회전, 마우스 좌우로 카메라는 y축 회전 유니티 좌표는 x = 끄덕 y = 절래절래 z = 갸우뚱
        if (mouseMove.x < -40f)
            mouseMove.x = -40f;
        else if (mouseMove.x > 40f)
            mouseMove.x = 40f;     
        cameraPivotTr.eulerAngles = mouseMove;
        RaycastHit hit;
        //실제 카메라 위치에서 - 카메라 피벗 위치를 빼서 방향을 구한다.
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
            CalcInputMove();//움직임을 계산
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))
            {
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;
                //그라운드에 닿았다면 런스피드나 워크스피드만큼 주어서 재빨리
                //다시 찾기하게 땅이 울퉁불퉁 하다면                 
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
        moveVelocity = transform.TransformDirection(moveVelocity); //절대좌표(월드좌표)로 반대의 경우는 InverseTransformDirection 이다
        if (0.01f < moveVelocity.sqrMagnitude)
        {
            Quaternion cameraRot = cameraPivotTr.rotation; //쿼터니언 값으로 가져오기
            cameraRot.x = cameraRot.z = 0f; //좌표값 변환
            transform.rotation = cameraRot; //움직이고 있을 때 카메라 회전 제한
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
    void CameraDistanceCtrl()//카메라 거리조절
    {
        cameraDistance -= Input.GetAxisRaw("Mouse ScrollWheel")*5f;        
    }
    void FreezeXZ()//캐릭터 컨트롤러의 회전 제한 x축 회전과 z축 회전
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
