using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallProjectile : MonoBehaviour
{

    private event System.Action OnBallDestroy;

    public static BallProjectile instance;
    
    public Rigidbody ball;
    Collider[] colliderArray;

    public float h = 25;
    public float gravity = -18;

    public float velocity;

    public Vector3 destinationPoint;
    //public Transform target;
    public Vector3 targetVector;
    RaycastHit hitInfo;

    public bool debugPath;

    private Camera cam;
    LineDrawer[] lineDrawer;

    public Transform launcher;
    private bool isLaunched = false;
    
    // Start is called before the first frame update
    private void Awake()
    {

        
        if(instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
        
    }


    void Start()
    {
        cam = Camera.main;
        lineDrawer = new LineDrawer[30];
        foreach(var i in lineDrawer)
        {
            OnBallDestroy += i.Destroy;
        }
        ball.useGravity = false;


    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DrawPath();
        }


        if (Input.GetMouseButtonUp(0))
        {
            Launch();
        }

        if (ball.transform.position.y < -1)
        {
            foreach(var i in lineDrawer)
            {
                
                i.Destroy();
            }
            Destroy(gameObject, .5f);
        }
    }

    //private void FixedUpdate()
    //{
    //    if(isLaunched == false)
    //    {
    //        FollowLaunchPosition();
    //    }
        
    //}

    //void FollowLaunchPosition()
    //{
    //    transform.position = launcher.position;
    //}

    public void Launch()
    {
        isLaunched = true;
        Physics.gravity = Vector3.up * gravity;
        ball.useGravity = true;
        print(CalculateDestination().initialVelocity);
        ball.velocity = CalculateDestination().initialVelocity;
        foreach (var i in lineDrawer)
        {
            i.Destroy();
        }

    }


    LaunchData CalculateDestination()
    {
        
        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
        LayerMask hitMask =  LayerMask.GetMask("MousePosition");
       
        if (Physics.Raycast(camRay, out hitInfo, 100, hitMask))
        {
            //target.position = hitInfo.point;
            targetVector = hitInfo.point;
            
        }


        //float destinationY = target.position.y - ball.position.y;
        float destinationY = targetVector.y - ball.position.y;


        Vector3 destinationXZ = new Vector3(targetVector.x - ball.position.x, 0, targetVector.z - ball.position.z);

        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(-2 * Mathf.Abs(destinationY - h) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
        Vector3 velocityXZ = destinationXZ / time;

        Vector3 result = velocityXZ + velocityY;



        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);

    }

    void DrawPath()
    {

        LaunchData launchData = CalculateDestination();
        Vector3 previousDrawPoint = ball.position;

        int resolution = 30;

        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution * launchData.timeToTarget;
            Vector3 distance = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = ball.position + distance;
            //Debug.DrawLine(previousDrawPoint, drawPoint,Color.green);
            lineDrawer[i - 1].DrawLineInGameView(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;


        }
    }

    
    private void OnCollisionEnter(Collision collision)
    {


        if (isLaunched)
        {
            ball.Sleep();

            colliderArray = Physics.OverlapSphere(transform.position, 10f);

            Zombie zombie;
            foreach (var collider in colliderArray)
            {
                zombie = collider.transform.GetComponentInParent<Zombie>();


                if (zombie != null)
                {

                    zombie.SetTargetPosition(transform.position);
                }
            }
            if(OnBallDestroy != null)
            {
                OnBallDestroy();
            }
            Destroy(gameObject, 0.5f);
        }
        
        
    }


    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }
    }

    public struct LineDrawer
    {
        private LineRenderer lineRenderer;
        private float lineSize;

        public LineDrawer(float lineSize = 0.2f)
        {
            GameObject lineObj = new GameObject("LineObj");
            
            lineRenderer = lineObj.AddComponent<LineRenderer>();
            //Particles/Additive
            lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

            this.lineSize = lineSize;
        }

        private void init(float lineSize = 0.2f)
        {
            if (lineRenderer == null)
            {
                GameObject lineObj = new GameObject("LineObj");
                lineRenderer = lineObj.AddComponent<LineRenderer>();
                //Particles/Additive
                lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));

                this.lineSize = lineSize;
            }
        }

        //Draws lines through the provided vertices
        public void DrawLineInGameView(Vector3 start, Vector3 end, Color color)
        {
            if (lineRenderer == null)
            {
                init(0.2f);
            }

            //Set color
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            //Set width
            lineRenderer.startWidth = lineSize;
            lineRenderer.endWidth = lineSize;

            //Set line count which is 2
            lineRenderer.positionCount = 2;

            //Set the postion of both two lines
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, end);
        }

        public void Destroy()
        {
            if (lineRenderer != null)
            {
                Object.Destroy(lineRenderer.gameObject);

            }
        }

    }
}
