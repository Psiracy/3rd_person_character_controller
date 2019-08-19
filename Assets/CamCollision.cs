using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollision : MonoBehaviour
{
    [Header("Distance")]
    [SerializeField]
    float minDistance;
    float maxDistance;
    [SerializeField]
    float closeInSpeed;

    [Header("Misc")]
    [Range(0, 1), SerializeField]
    float groundClipPercentage;
    Vector3 direction;
    float distance;

    void Start()
    {
        direction = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        maxDistance = distance;
    }

    void Update()
    {
        //get cam position
        Vector3 desiredCamPos = transform.parent.TransformPoint(direction * maxDistance);

        //check if line gets interupted
        RaycastHit hit;
        if (Physics.Linecast(transform.parent.position, desiredCamPos, out hit))
        {
            distance = Mathf.Clamp((hit.distance * groundClipPercentage), minDistance, maxDistance);

        }
        else
        {
            distance = maxDistance;
        }

        //move the camara accordingly 
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, direction * distance, Time.deltaTime * closeInSpeed);
    }
}
