using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 1f;
    public float rotateSpeed = 10f;
    public float jumpForce = 0.5f;
    public float gravity = -9.81f;
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.01f;

    private CharacterController controller;
    private Animator anim;
    private Rigidbody rb;
    private int scoreCount;
    private Vector3 currentVelocity;
    private Vector3 hitPos;
    private BoxCollider boxCollider;
    private Vector3 bottomCenter;

    public bool isGrounded;
    public bool isJumping;
    private float verticalVelocity;


    void Start()
    {
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = CheckGrounded();
        HandleMovement();
        HandleRotation();
        HandleJump();
        if (isGrounded && isJumping)
            isJumping = false;
    }

    private void HandleRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, -90f * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 90f * Time.deltaTime, 0);
        }
    }

    private void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump") && transform.up == Vector3.up)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            isGrounded = false;
        }

        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        else if (verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (isJumping)
        {
            Vector3 moveVector = Vector3.up * verticalVelocity * Time.deltaTime;
            controller.Move(moveVector);

            anim.SetBool("IsJumping", !isGrounded);
        }
    }

    private void HandleMovement()
    {
        Vector3 input = GetMovementInput();

        if (input.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

            Vector3 moveDir = targetRotation * Vector3.forward;

            if (isGrounded)
            {
                MoveUsingCurvedRaycast(moveDir);
            }
            else
            {
                controller.Move(moveDir * speed * Time.deltaTime);
            }

            anim.SetBool("IsWalk", true);
        }
        else
        {
            anim.SetBool("IsWalk", false);
        }
    }

    private void MoveUsingCurvedRaycast(Vector3 moveDirection)
    {
        float arcAngle = 270f;
        float arcRadius = speed * Time.deltaTime;
        int arcResolution = 6;
        if (PhysicsExtension.ArcCast(transform.position, Quaternion.LookRotation(moveDirection, transform.up), arcAngle, arcRadius, arcResolution, groundLayer, out RaycastHit hit))
        {
            hitPos = hit.point;
            Vector3 newPosition = hitPos;

            currentVelocity = (newPosition - transform.position) / Time.deltaTime;
            transform.position = newPosition;
            transform.MatchUp(hit.normal);
        }
    }

    private bool CheckGrounded()
    {
        Vector3 topCenter = transform.position + transform.up * boxCollider.size.y;
        return Physics.Raycast(topCenter, -transform.up, boxCollider.size.y + groundCheckDistance, groundLayer);
        // return m_Ctrl.isGrounded;
    }

    public Vector3 GetVelocity()
    {
        return currentVelocity;
    }

    private Vector3 GetMovementInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        return new Vector3(h, 0, v).normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(hitPos, 0.01f);

        Gizmos.color = Color.green;
        Gizmos.DrawCube(bottomCenter, new Vector3(0.001f, 0.001f, 0.001f));
    }
}