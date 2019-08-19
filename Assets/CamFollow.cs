using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Camera Options")]
    [SerializeField]
    float camMovSpeed;
    [SerializeField]
    float sensitivity;
    [SerializeField]
    float minAngle, maxAngle;

    [Header("Misc")]
    [SerializeField]
    Transform followObject;
    float mouseX, mouseY;
    float rotX, rotY;

    void Start()
    {
        Vector3 startRotation = transform.eulerAngles;
        rotX = startRotation.x;
        rotY = startRotation.y;

        //lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //set rotation
        rotX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //clamp y rotation
        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        //rotation
        transform.rotation = Quaternion.Euler(rotY, -rotX, 0);

        //movement
        transform.position = Vector3.MoveTowards(transform.position, followObject.position, camMovSpeed * Time.deltaTime);
    }
}
