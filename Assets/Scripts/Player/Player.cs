using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public BallProjectile ballProjectilePrefab;
    public Transform launchPosition;
    public float movementSpeed = 6.0f;
    public float minRotationDistance;
    Camera cam;
    Vector3 velocity;

    public float cameraLerpTime;
    Rigidbody rigidBody;

    BallProjectile ballProjectile;

    private void Awake()
    {
        cam = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
    }

    //void FacePlayer(Transform target)
    //{
    //    Vector3 dir = target.position - transform.position;

    //    Quaternion lookRotation = Quaternion.LookRotation(dir);
    //    Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * cameraLerpTime).eulerAngles;
    //    transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    //}

    


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ballProjectile = Instantiate(ballProjectilePrefab, launchPosition.position, Quaternion.identity);
            ballProjectile.launcher = launchPosition;
            
        }
        else
        {
            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * movementSpeed;
        }

    

        if (Input.GetAxisRaw("Horizontal") > 0)
        {

        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {


            Vector3 groundPosition = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);
            
            if (Vector3.Magnitude(groundPosition - transform.position) > minRotationDistance)
            {
                transform.LookAt(groundPosition + Vector3.up * transform.position.y);
            }

        }


        
        //Vector3 mousePosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));

        //transform.LookAt(mousePosition);

        //print(mousePosition + "asdf");


        
        

        
    }
    
    private void FixedUpdate()
    {
        rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
    }
}
