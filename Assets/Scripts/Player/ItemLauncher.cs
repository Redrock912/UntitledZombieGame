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

    // Start is called before the first frame update
    void Start()
    {

        Instance = this;
        cam = Camera.main;
        item.useGravity = false;
        item = GetComponent<Rigidbody>();
        launchEquation = GetComponentInParent<LaunchToDestination>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            CalculateDestination();
        }

        if (Input.GetMouseButtonUp(0))
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

        

        
        
        
        
        //launcherAngle = Quaternion.Euler(0, , 0) * launcherAngle;
    }

    void Launch()
    {
        item.velocity = launcherAngle*magnitude;

        Physics.gravity = Vector3.down * launchEquation.gravity;
        item.useGravity = true;
        
    }
}
