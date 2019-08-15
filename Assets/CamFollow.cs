using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [Header("Camera Options")]
    public float camMovSpeed;
    public float sensitivity;
    public float minAngle, maxAngle;

    [Header("Misc")]
    public Transform followObject;

    float mouseX, mouseY;
    float rotX, rotY;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startRotation = transform.eulerAngles;
        rotX = startRotation.x;
        rotY = startRotation.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //set rotation
        rotX += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        //clamp rotation
        rotY = Mathf.Clamp(rotY, minAngle, maxAngle);

        //rotation
        transform.rotation = Quaternion.Euler(rotY, -rotX, 0);
        //movement
        transform.position = Vector3.MoveTowards(transform.position, followObject.position, camMovSpeed * Time.deltaTime);
    }
}
