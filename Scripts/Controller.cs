using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float damp;
    [Range(1, 20)]
    public float rotationSpeed;
    [Range(1, 20)]
    public float strafeTurnSpeed;
    float normalFov;
    public float sprintFov;
    bool isGrounded;
    bool isJumping;


    float maxSpeed;

    //fall speed
    float ySpeed;

    public float jumpHorizontalSpeed;


    [SerializeField] float jump;
    float inputX;
    float inputZ;
    public Transform model;

    [SerializeField] private LayerMask groundMask;

    public GameOverScript gameOver;

    Animator anim;
    Vector3 moveDirection;
    Camera mainCam;
    Rigidbody rb;
    CapsuleCollider cc;

    public KeyCode SprintButton = KeyCode.LeftShift;
    public KeyCode walkButton = KeyCode.C;
    public KeyCode jumpButton = KeyCode.Space;

    public enum MovementType
    {
        Directional,
        Strafe
    };

    public MovementType movementType;
    bool isStrafeMoving;
    public PlayerManager pm;

    void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;
        normalFov = mainCam.fieldOfView;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    public float GetYSpeed()
    {
        return ySpeed;
    }
    private void FixedUpdate()
    {
        ySpeed = rb.velocity.y;
        isGrounded = Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y - 0.1f, cc.bounds.center.z), 0.18f, groundMask);
        Movement();
        
        //velocity vector3 veriyor y sini al
        if (isGrounded == false)
        {
            if (moveDirection != Vector3.zero)
            {
                Vector3 velocity = moveDirection * jumpHorizontalSpeed; 
                velocity.y = ySpeed;
                velocity = transform.forward;
                rb.MovePosition(rb.position += velocity * jumpHorizontalSpeed * Time.deltaTime);
            }
        }
        
        void Movement()
        {

            if (movementType == MovementType.Strafe)
            {
                inputX = Input.GetAxis("Horizontal");
                inputZ = Input.GetAxis("Vertical");
                anim.SetFloat("inputX", inputX, damp, Time.deltaTime * 10000);
                anim.SetFloat("inputZ", inputZ, damp, Time.deltaTime * 10000);
                //Jump();
                var isMoving = inputX != 0 || inputZ != 0;
                if (isMoving)
                {
                    moveDirection = new Vector3(inputX, 0, inputZ);
                    float yawCamera = mainCam.transform.rotation.eulerAngles.y;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), strafeTurnSpeed * Time.fixedDeltaTime);
                    anim.SetBool("strafeMoving", true);
                }
                else
                {
                    anim.SetBool("strafeMoving", false);
                }
            }
            if (movementType == MovementType.Directional)
            {
                moveDirection = new Vector3(inputX, 0, inputZ);
                InputMove();
                InputRotation();
                Jump();
                if (Input.GetKey(SprintButton))
                {
                    mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, sprintFov, Time.deltaTime * 2);

                    maxSpeed = 2f;
                    inputX = 2 * Input.GetAxis("Horizontal");
                    inputZ = 2 * Input.GetAxis("Vertical");
                }
                else if (Input.GetKey(walkButton))
                {
                    mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, normalFov, Time.deltaTime * 2);
                    maxSpeed = 0.2f;
                    inputX = Input.GetAxis("Horizontal");
                    inputZ = Input.GetAxis("Vertical");
                }
                else
                {
                    mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, normalFov, Time.deltaTime * 2);
                    maxSpeed = 1f;
                    inputX = Input.GetAxis("Horizontal");
                    inputZ = Input.GetAxis("Vertical");
                }
            }
        }
        void InputMove()
        {
            anim.SetFloat("speed", Vector3.ClampMagnitude(moveDirection, maxSpeed).magnitude, damp, Time.deltaTime * 10);
        }
        void InputRotation()
        {
            Vector3 rotOfSet = mainCam.transform.TransformDirection(moveDirection);
            rotOfSet.y = 0;
            model.forward = Vector3.Slerp(model.forward, rotOfSet, Time.deltaTime * rotationSpeed);
        }
        void Jump()
        {
            if (isGrounded)
            {
                anim.SetBool("isGrounded", true);
                isGrounded = true;
                //anim.applyRootMotion = false;
                anim.SetBool("isJumping", false);
                isJumping = false;
                anim.SetBool("isFalling", false);
                if (Input.GetKey(jumpButton))
                {
                    anim.SetBool("isJumping", true);
                    anim.SetTrigger("Jump");
                    //CSE 364 lecture discussion FORCEMODE.IMPULSE 
                    rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
                    FindObjectOfType<AudioManager>().Play("Jump");
                    
                    //Animator variables
                    isJumping = true;
                }
            }
            else
            {
                anim.SetBool("isGrounded", false);
                isGrounded = false;
                if ((isJumping && ySpeed < 0) || ySpeed < -0.3)
                {
                    anim.SetBool("isFalling", true);
                }
            }
        }
    }
}    
