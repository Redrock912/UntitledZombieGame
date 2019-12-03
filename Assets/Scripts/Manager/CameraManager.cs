using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public float viewDistanceMultiplier;
    public Vector3 viewOffset;
    public Transform playerTransform;
    public float smoothTime;

    float magnifyMultiplier = 1.0f;
    public float maximumDistance;

    private void Update()
    {

        // camera zoom in/out
        if (Input.mouseScrollDelta.y > 0)
        {
            magnifyMultiplier = 0.9f;

        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            magnifyMultiplier = 1.1f;

        }
        else
        {
            magnifyMultiplier = 1.0f;
        }

        viewOffset *= magnifyMultiplier;
        viewOffset = Vector3.ClampMagnitude(viewOffset, maximumDistance);
    }

    void FixedUpdate()
    {

        Vector3 finalPosition = playerTransform.position + viewOffset;
        
        Vector3 smoothPosition = Vector3.Lerp(transform.position, finalPosition, smoothTime);

        transform.position = smoothPosition;
    }
}
