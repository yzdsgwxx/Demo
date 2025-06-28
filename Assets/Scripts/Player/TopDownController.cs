using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Title:TopDown控制器
 * Description: 移动，跳跃，冲刺，相机跟随，空中保留起跳速度，平滑转身。
 * TODO:跟随相机。优化跳跃手感，斜坡停止弹跳bug.
 */
public class TopDownController : MonoBehaviour
{
    [Header("移动配置")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;
    public float rotateSpeed = 5f;
    public int groundCheckRayNum = 8;
    //选择MoveSpeed;
    public float MoveSpeed
    {
        get
        {
            if (isSprint)
            {
                return sprintSpeed;
            }
            else
            {
                return walkSpeed;
            }
        }
    }
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.2f;

    [Header("状态记录")]
    public bool isGrounded;
    public bool isSprint;
    public Vector3 direction;
    private Vector3 deltaCamPos;
    private Vector3 jumpStartSpeed;

    private Camera followCamera;
    private Rigidbody rb;
    private Collider col;
    [Header("脚本引用")]
    [SerializeField] private GameObject groundCheck;

    [Header("变量")]
    [SerializeField] private List<Transform> groundCheckPoints;
    [SerializeField] private Vector3 groundNormal;
    private void Awake()
    {
        followCamera = FindObjectOfType<Camera>();
        rb = GetComponentInChildren<Rigidbody>();
        col = GetComponentInChildren<Collider>();

        deltaCamPos = followCamera.transform.position - rb.transform.position;
        //从圆上取出若干个点。
        groundCheckPoints.Clear();
        for (int i = 0; i < groundCheckRayNum; i++)
        {
            var go = new GameObject();
            groundCheckPoints.Add(go.transform);
            go.transform.parent = groundCheck.transform;
            Vector3 forward = transform.forward * col.bounds.extents.x;
            //Quaternion * vec 绕轴旋转。
            go.transform.position = groundCheck.transform.position + Quaternion.AngleAxis(360 / groundCheckRayNum * i, transform.up) * forward;
        }

        ////包含自身
        //groundCheckPoints = groundCheck.GetComponentsInChildren<Transform>().Where(t => t != groundCheck.transform).ToList<Transform>();
    }

    private void Update()
    {
        MoveCheck();
        JumpCheck();
        OrientToMovement();
        CameraFollow();
    }

    void OrientToMovement()
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            //位移需要考虑DeltaTime。
            rb.transform.rotation = Quaternion.Lerp(rb.transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void MoveCheck()
    {

        isSprint = InputManager.instance.InState("sprint", KeyState.Hold);

        direction = Vector3.zero;
        Transform cordinate = followCamera.transform;
        if (InputManager.instance.InState("forward", KeyState.Hold))
        {
            direction += Utils.RemoveZFromDirection(cordinate.forward);
        }
        if (InputManager.instance.InState("backward", KeyState.Hold))
        {
            direction += Utils.RemoveZFromDirection(cordinate.forward * -1);
        }
        if (InputManager.instance.InState("left", KeyState.Hold))
        {
            direction += cordinate.right * -1;
        }
        if (InputManager.instance.InState("right", KeyState.Hold))
        {
            direction += cordinate.right;
        }
        //平行于地面并且normalize
        direction = Vector3.ProjectOnPlane(direction, groundNormal).normalized;
        Vector3 speed = isGrounded ? direction * MoveSpeed : jumpStartSpeed;
        rb.velocity = new Vector3(speed.x, rb.velocity.y, speed.z);
        DebugDirection();
    }

    void JumpCheck()
    {

        CheckGrounded();

        if (InputManager.instance.InState("jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CameraFollow()
    {
        followCamera.transform.position = rb.position + deltaCamPos;
    }

    /// <summary>
    /// 自己放检测点
    /// </summary>
    void CheckGrounded()
    {

        bool oldIsGround = isGrounded;
        bool bGrounded = false;

        RaycastHit hit;
        foreach (var point in groundCheckPoints)
        {
            //检测不出来检查设置Layer了没有！！！！
            if (Physics.Raycast(point.position, transform.up * -1, out hit, groundCheckDistance, groundLayer))
            {
                groundNormal = hit.normal;
                bGrounded = true;
                break;
            }
        }

        // 地面检测
        //起始点往身体插进去一点
        //记录完JumpStartSpeed再记录isgrounded.
        if (oldIsGround == true && bGrounded == false)
        {
            jumpStartSpeed = rb.velocity;
        }
        isGrounded = bGrounded;

        DebugGroundCheck();
    }

    private void DebugGroundCheck()
    {
        foreach (var point in groundCheckPoints)
        {
            Debug.DrawRay(point.position, transform.up * -1 * groundCheckDistance);
        }
    }

    private void DebugDirection()
    {
        Debug.DrawRay(transform.position, direction * 5);
    }
}
