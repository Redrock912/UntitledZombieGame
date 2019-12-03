using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class LaunchToDestination : MonoBehaviour
{

    Mesh mesh;
    public float meshWidth;

    public Vector3 dir;
    public float maxDistance;
    public int resolution;

    // fixed
    [HideInInspector]
    public float angle = 45;

    public float velocity;
    public float gravity;
    float radiantAngle;
    public float estimatedTime;

    Camera cam;
    RaycastHit hitInfo;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        gravity = 9.8f; // Physics2D.gravity.y
    }
    // Start is called before the first frame update
    void Start()
    {
        //  RenderEstimation(CalculatePositions(maxDistance));

        cam = Camera.main;

        

    }

    private void OnValidate()
    {
        // Check that mesh is not null and that game is playing
        if (mesh != null && Application.isPlaying)
        {
          //  RenderEstimation(CalculatePositions(maxDistance));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {

            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
            LayerMask hitMask = LayerMask.GetMask("MousePosition");

            if (Physics.Raycast(camRay, out hitInfo, 100, hitMask))
            {

                dir = new Vector3(hitInfo.point.x - transform.position.x, 0, hitInfo.point.z - transform.position.z);

                maxDistance = dir.magnitude;

                RenderEstimation(CalculatePositions(maxDistance,dir.normalized));

                // turn the launcher
                transform.LookAt(dir);
            }

            
        }

        

        
    }


    void RenderEstimation(Vector3[] arcVertices)
    {
        // clear out information 
        mesh.Clear();

        // 3d information
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 3 * 2 * 2]; // every quad is 2 triangles => 3*2 , double-sided => *2

        for (int i = 0; i <= resolution; i++)
        {
            
            // x is width of arc, y is height, z is the forward
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVertices[i].y, arcVertices[i].x);
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVertices[i].y, arcVertices[i].x);

            // no triangles on the last point
            if (i != resolution)
            {

                // first side clockwise
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
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

    Vector3[] CalculatePositions(float maxDistance,Vector3 dir)
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radiantAngle = Mathf.Deg2Rad * angle;
        // d = v^2 * sintheta / g
        //float maxDistance = (velocity * velocity * Mathf.Sin(2 * radiantAngle)) / gravity;

        velocity = Mathf.Sqrt(gravity * maxDistance / Mathf.Sin(2 * radiantAngle));
        estimatedTime = maxDistance / velocity;


        for (int i = 0; i <= resolution; i++)
        {
            float u = (float)i / (float)resolution;

            arcArray[i] = CalculatePoint(u, maxDistance,dir);
        }


        return arcArray;
    }


    // returns the point of height and distance
    Vector3 CalculatePoint(float u, float maxDistance,Vector3 dir)
    {
        float x = maxDistance * u;

        // y =y0 + tan(th) * x - g*x*x / 2*velocity^2 * cos(th)^2
        float y =  x * Mathf.Tan(radiantAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radiantAngle) * Mathf.Cos(radiantAngle)));

        Vector3 equationVector = new Vector3(x,y);
        return equationVector;
    }
}
