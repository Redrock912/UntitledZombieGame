using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaunchArcRenderer : MonoBehaviour
{

    LineRenderer lineRenderer;

    public float velocity;
    public float angle;
    public int resolution;

    float gravity;
    float radiantAngle;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        gravity = 9.8f; // Physics2D.gravity.y
    }
    // Start is called before the first frame update
    void Start()
    {
        RenderEstimation();
    }

    private void OnValidate()
    {
        // Check that lr is no null and that game is playing
        if(lineRenderer != null && Application.isPlaying)
        {
            RenderEstimation();
        }
    }


    void RenderEstimation()
    {
        lineRenderer.positionCount = resolution + 1;
        lineRenderer.SetPositions(CalculatePositions());
    }

    Vector3[] CalculatePositions()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radiantAngle = Mathf.Deg2Rad * angle;
        // d = v^2 * sintheta / g
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radiantAngle)) / gravity;

        for(int i = 0; i <= resolution; i++)
        {
            float u = (float)i / (float)resolution;

            arcArray[i] = CalculatePoint(u,maxDistance);
        }


        return arcArray;
    }


    // returns the point of height and distance
    Vector3 CalculatePoint(float u, float maxDistance)
    {
        float x = maxDistance * u;

        // y = tan(th) * x - g*x*x / 2*velocity^2 * cos(th)^2
        float y = x * Mathf.Tan(radiantAngle) - ((gravity * x * x) / (2*velocity*velocity*Mathf.Cos(radiantAngle)*Mathf.Cos(radiantAngle)));

        return new Vector3(x, y);
    }
}
