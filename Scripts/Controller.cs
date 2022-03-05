using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    
    public float damp;
    public float rotationSpeed;
    float normalFov;
    public float sprintFov;
    bool isGrounded;

    float maxSpeed;
    [SerializeField] float jump;
    float inputX;
    float inputZ;



    public Transform model;

    [SerializeField] private LayerMask groundMask;
    Animator anim;
    Vector3 moveDirection;
    Camera mainCam;
    Rigidbody rb;
    CapsuleCollider cc;


    public KeyCode SprintButton = KeyCode.LeftShift;

    public KeyCode walkButton = KeyCode.C;

    public KeyCode jumpButton = KeyCode.Space;
    void Start()
    {
        anim = GetComponent<Animator>();
        mainCam = Camera.main;
        normalFov = mainCam.fieldOfView;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        //isGrounded = Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y - 0.1f, cc.bounds.center.z), 0.18f);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        isGrounded = Physics.CheckCapsule(cc.bounds.center, new Vector3(cc.bounds.center.x, cc.bounds.min.y - 0.1f, cc.bounds.center.z), 0.18f,groundMask);
        InputMove();
        InputRotation();
        Movement();
        if (isGrounded) { Jump(); }
        
    
    }
    void Movement()
    {
        moveDirection = new Vector3(inputX, 0, inputZ);

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
        if (Input.GetKey(jumpButton))
        {
            rb.AddForce(Vector3.up * jump);
        }

    }
}