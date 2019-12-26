using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ItemLauncher : MonoBehaviour
{

    public Rigidbody item;

    private Camera cam;
    public static ItemLauncher Instance { get; private set; }

    LaunchToDestination launchEquation;
    float magnitude;
    float angle;
    Vector3 launcherAngle;
    protected bool isLaunched =false;
    protected Collider[] colliderArray;
    public int requiredAmount = 1;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        cam = Camera.main;
        item.useGravity = false;
        item = GetComponent<Rigidbody>();
        launchEquation = GetComponentInParent<Player>().GetComponentInChildren<LaunchToDestination>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isLaunched == false)
        {
            FollowLaunchPosition();
        }
        
        if (Input.GetMouseButton(0))
        {
            CalculateDestination();
            
        }

        if (Input.GetMouseButtonUp(0) && isLaunched == false)
        {
        
            Launch();
        }
    }



    void CalculateDestination()
    {
        magnitude = launchEquation.velocity;
        angle = launchEquation.angle;

        float radiantAngle = Mathf.Deg2Rad * angle;
        
        Vector3 vectorXZ = launchEquation.dir.normalized;
        Vector3 vectorY = Vector3.up;

        launcherAngle = (vectorXZ + vectorY).normalized;

    }

    void Launch()
    {
        isLaunched = true;
        transform.parent = null;
        item.velocity = launcherAngle*magnitude;

        Physics.gravity = Vector3.down * launchEquation.gravity;
        item.useGravity = true;
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (isLaunched)
        {
            item.Sleep();

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
            Destroy(gameObject, 0.5f);
        }

    }

  

    void FollowLaunchPosition()
    {
        transform.position = transform.parent.position;
    }
}
