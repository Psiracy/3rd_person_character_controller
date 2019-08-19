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
    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float accelarationinKMH, forceinKMH, walkSpeedinKMH, runSpeedinKMH;
    float accelaration, desiredSpeed, force;
    public float mass;

    MoveSpeed moveSpeed = MoveSpeed.idle;
    MoveDirection moveDirection;

    Vector3 movement;
    Vector3 targetPos;
    Vector3 velocity;
    Vector3 desiredVelocity;

    [Header("Turning")]
    Vector3 previusRatation;
    [SerializeField]
    float walkingTurnSpeed, runningTurnSpeed;

    [Header("Animation")]
    Animator animator;

    [Header("Misc")]
    [SerializeField]
    Transform camPivot;
    Camera cam;
    bool isRunning;
    Vector3 offset;

    void Start()
    {
        accelaration = accelarationinKMH / 3.6f;
        force = forceinKMH / 3.6f;
        animator = GetComponent<Animator>();
        offset = Vector3.zero;
        cam = Camera.main;
    }

    void FixedUpdate()
    {
        #region animations
        //movement animation checker
        movementSpeed = velocity.magnitude * 3.6f;

        //set animation based on speed
        if (animator.GetBool("IsMoveing") == false)
        {
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

        #endregion animations

        #region movement
        //get movement input
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        movement = new Vector3(horizontal, 0, vertical);

        //turn character the correct way
        #region turning
        //set movement direction
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

        //rotate the character
        Quaternion desireredRotation = Quaternion.identity;

        switch (moveDirection)
        {
            case MoveDirection.forward:
                desireredRotation = Quaternion.Euler(Vector3.zero);
                //diagonal
                //right
                if (movement.x > .8f && movement.z > .5f)
                {
                    desireredRotation = Quaternion.Euler(new Vector3(0, 30, 0));
                }
                //left
                if (movement.x < -.8f && movement.z > .5f)
                {
                    desireredRotation = Quaternion.Euler(new Vector3(0, -30, 0));
                }
                break;
            case MoveDirection.left:
                desireredRotation = Quaternion.Euler(new Vector3(0, -90, 0));
                break;
            case MoveDirection.right:
                desireredRotation = Quaternion.Euler(new Vector3(0, 90, 0));
                break;
            case MoveDirection.backwards:
                desireredRotation = Quaternion.Euler(new Vector3(0, 180, 0));
                //diagonal
                //right
                if (movement.x > .8f && movement.z < -.5f)
                {
                    desireredRotation = Quaternion.Euler(new Vector3(0, 180 - 30, 0));
                }
                //left
                if (movement.x < -.8f && movement.z < -.5f)
                {
                    desireredRotation = Quaternion.Euler(new Vector3(0, 180 + 30, 0));
                }
                break;
            default:
                break;
        }

        //add the camare rotatate to the desired rotation
        desireredRotation *= Quaternion.Euler(0, camPivot.eulerAngles.y, 0);

        //rotate speed depens on movement speed
        switch (moveSpeed)
        {
            case MoveSpeed.walking:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desireredRotation, Time.deltaTime * walkingTurnSpeed);
                break;
            case MoveSpeed.running:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desireredRotation, Time.deltaTime * runningTurnSpeed);
                break;
            default:
                break;
        }
        #endregion turning

        //set speed
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
        targetPos = transform.position + camPivot.TransformDirection(movement * desiredSpeed);
        targetPos.y = 0;
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
        transform.position += velocity;
        #endregion movement
    }
}
