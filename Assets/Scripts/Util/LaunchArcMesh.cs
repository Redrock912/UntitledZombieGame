using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchArcMesh : MonoBehaviour
{

    Mesh mesh;
    public float meshWidth;

    public float velocity;
    public float angle;
    public int resolution;
    

    float gravity;
    float radiantAngle;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        gravity = 9.8f; // Physics2D.gravity.y
    }
    // Start is called before the first frame update
    void Start()
    {
        RenderEstimation(CalculatePositions());
    }

    private void OnValidate()
    {
        // Check that mesh is not null and that game is playing
        if (mesh != null && Application.isPlaying)
        {
            RenderEstimation(CalculatePositions());
        }
    }


    void RenderEstimation(Vector3[] arcVertices)
    {
        // clear out information 
        mesh.Clear();

        // 3d information
        Vector3[] vertices = new Vector3[(resolution + 1)*2];
        int[] triangles = new int[resolution *3*2*2]; // every quad is 2 triangles => 3*2 , double-sided => *2

        for(int i = 0; i <= resolution; i++)
        {
            // x is width of arc, y is height, z is the forward
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVertices[i].y, arcVertices[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVertices[i].y, arcVertices[i].x);

            // no triangles on the last point
            if(i != resolution)
            {

                // first side clockwise
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i* 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i+ 1)*2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                // the other side counterclockwise
                triangles[i * 12 + 6] = i * 2; // first vertice
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2; // second vertice
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1; // first vertice of the next segment
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;  // second vertice of the next segment


            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
        }

    }

    Vector3[] CalculatePositions()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radiantAngle = Mathf.Deg2Rad * angle;
        // d = v^2 * sintheta / g
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radiantAngle)) / gravity;
        
        for (int i = 0; i <= resolution; i++)
        {
            float u = (float)i / (float)resolution;

            arcArray[i] = CalculatePoint(u, maxDistance);
        }


        return arcArray;
    }


    // returns the point of height and distance
    Vector3 CalculatePoint(float u, float maxDistance)
    {
        float x = maxDistance * u;

        // y = tan(th) * x - g*x*x / 2*velocity^2 * cos(th)^2
        float y = x * Mathf.Tan(radiantAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radiantAngle) * Mathf.Cos(radiantAngle)));

        return new Vector3(x, y);
    }
}
