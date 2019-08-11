using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MoveSpeed
{
    idle,
    walking,
    running
}

enum MoveDirection
{
    forward,
    left,
    right,
    backwards
}

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    public float movementSpeed;
    public float accelarationinKMH, forceinKMH, walkSpeedinKMH, runSpeedinKMH;
    float accelaration, desiredSpeed, force;
    public float mass;

    MoveSpeed moveSpeed = MoveSpeed.idle;
    MoveDirection moveDirection;

    Vector3 movement;
    Vector3 targetPos;
    public Vector3 velocity;
    public Vector3 desiredVelocity;

    [Header("Turning")]
    public Transform hip;
    Vector3 previusRatation;
    public float walkingTurnSpeed, runningTurnSpeed;

    [Header("Camera")]
    public float camTurnSpeed;
    public GameObject camPivot, mainCam;

    [Header("Animation")]
    Animator animator;

    [Header("Misc")]
    public ParticleSystem landingPartical;
    bool isJumping, isGrounded, isRunning;
    Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        accelaration = accelarationinKMH / 3.6f;
        force = forceinKMH / 3.6f;
        animator = GetComponent<Animator>();
        offset = Vector3.zero;
    }

    void FixedUpdate()
    {
        #region camara
        //camera
        //cam movement
        camPivot.transform.position = new Vector3(transform.position.x, camPivot.transform.position.y, transform.position.z);
        //cam rotation
        camPivot.transform.rotation = Quaternion.Euler(new Vector3(0, Input.mousePosition.x * camTurnSpeed, 0));
        #endregion camera

        #region animations
        //movement animation checker
        movementSpeed = velocity.magnitude * 3.6f;

        if (animator.GetBool("IsMoveing") == false)
        {
            Debug.Log("wee");
            animator.SetFloat("Speed", 0);
            moveSpeed = MoveSpeed.idle;
        }
        else if (isRunning == false)
        {
            animator.SetFloat("Speed", 1);
            moveSpeed = MoveSpeed.walking;
        }
        else
        {
            animator.SetFloat("Speed", 2);
            moveSpeed = MoveSpeed.running;
        }

        //set animation based on speed
        if (!Input.GetButton("Fire2"))
        {
            switch (moveSpeed)
            {
                case MoveSpeed.idle:
                    transform.rotation = camPivot.transform.rotation;
                    break;
                case MoveSpeed.walking:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, camPivot.transform.rotation, Time.deltaTime * walkingTurnSpeed);
                    break;
                case MoveSpeed.running:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, camPivot.transform.rotation, Time.deltaTime * runningTurnSpeed);
                    break;
                default:
                    break;
            }

        }
        #endregion animations

        #region movement
        //movement
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        movement = new Vector3(horizontal, 0, vertical);

        #region turning
        //left
        if (movement.x < 0)
        {
            moveDirection = MoveDirection.left;
        }
        //right
        if (movement.x > 0)
        {
            moveDirection = MoveDirection.right;
        }
        //forward
        if (movement.z > 0)
        {
            moveDirection = MoveDirection.forward;
        }
        //backwards
        if (movement.z < 0)
        {
            moveDirection = MoveDirection.backwards;
        }
        //diagonal
        switch (moveDirection)
        {
            case MoveDirection.forward:
                //right
                if (movement.x > .8f && movement.z > .5f)
                {
                    hip.localEulerAngles = new Vector3(0, 30, 0);
                }
                //left
                if (movement.x < -.8f && movement.z > .5f)
                {
                    hip.localEulerAngles = new Vector3(0, -30, 0);
                }
                break;
            case MoveDirection.backwards:
                //right
                if (movement.x > .8f && movement.z < -.5f)
                {
                    hip.localEulerAngles = new Vector3(0, -30, 0);
                }
                //left
                if (movement.x < -.8f && movement.z < -.5f)
                {
                    hip.localEulerAngles = new Vector3(0, 30, 0);
                }
                break;
            default:
                hip.localEulerAngles = Vector3.zero;
                break;
        }

        animator.SetFloat("WalkingDirection", (float)moveDirection);
        #endregion turning

        //run
        if (Input.GetKey(KeyCode.LeftShift))
        {
            accelaration = 5 / 3.6f;
            desiredSpeed = runSpeedinKMH / 3.6f;
            isRunning = true;
        }
        else
        {
            accelaration = 1 / 3.6f;
            desiredSpeed = walkSpeedinKMH / 3.6f;
            isRunning = false;
        }

        //desired velocity
        targetPos = transform.position + transform.TransformDirection(movement * desiredSpeed);
        if (movement != Vector3.zero)
        {
            desiredVelocity = Vector3.Normalize(targetPos - transform.position) * desiredSpeed;
            animator.SetBool("IsMoveing", true);
        }
        else
        {
            animator.SetBool("IsMoveing", false);
            desiredVelocity = Vector3.zero;
        }

        //steering
        Vector3 steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, force);
        steering /= mass;

        //velocity
        velocity = Vector3.ClampMagnitude(velocity + steering, desiredSpeed * Time.deltaTime);
        if (isJumping == false)
        {
            transform.position += velocity;
        }
        else
        {
            transform.position += velocity * 1.5f;
        }

        //jump
        if (Input.GetButtonDown("Jump"))
        {
            animator.SetFloat("JumpType", Random.Range(0, 2));
            animator.SetTrigger("Jump");
            isJumping = true;
            isGrounded = false;
        }

        if (isGrounded == true)
        {
            isJumping = false;
        }
        #endregion movement
    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    public void StartPartical()
    {
        landingPartical.Play();
    }
}
