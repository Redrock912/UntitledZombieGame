using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public BallProjectile ballProjectilePrefab;
    public Transform launchPosition;


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
          BallProjectile ballProjectile =   Instantiate(ballProjectilePrefab, launchPosition.position, Quaternion.identity);
            ballProjectile.transform.parent = transform;
        }
    }
}
