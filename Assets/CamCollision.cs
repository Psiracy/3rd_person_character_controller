using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollision : MonoBehaviour
{
    [Header("Distance")]
    public float minDistance;
    float maxDistance;
    public float closeInSpeed;

    [Header("Misc")]
    [Range(0,1)]
    public float groundClipPercentage;
    Vector3 direction;
    float distance;

    // Use this for initialization
    void Start()
    {
        direction = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
        maxDistance = distance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCamPos = transform.parent.TransformPoint(direction * maxDistance);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCamPos, out hit))
        {
            distance = Mathf.Clamp((hit.distance * groundClipPercentage), minDistance, maxDistance);

        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, direction * distance, Time.deltaTime * closeInSpeed);
    }
}
