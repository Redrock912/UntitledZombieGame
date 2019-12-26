using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public event System.Action OnItemLaunch;
    public event System.Action OnPlayerDeath;

    //public BallProjectile ballProjectilePrefab;
    public List<ItemLauncher> itemLauncherPrefabList;
    

    public Transform launchPosition;
    public float movementSpeed = 6.0f;
    public float impactMultiplier = 10.0f;
    public float minRotationDistance;
    Camera cam;
    Vector3 velocity;

    public float cameraLerpTime;
    Rigidbody rigidBody;

    BallProjectile ballProjectile;
    ItemLauncher item;

    public bool isAlive = true;

    ZombiePool zombiePool;

    [HideInInspector]
    public int currentSlotNumber;

    public List<ItemObject> nearItems;
    // 0 = rock, 1= fireworks
    public int[] currentItems;

    private void Awake()
    {
        cam = Camera.main;
        rigidBody = GetComponent<Rigidbody>();
        currentItems = new int[4];
       
        nearItems = new List<ItemObject>();
       
    }

    private void Start()
    {
        zombiePool = ZombiePool.Instance;
        rigidBody = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (isAlive)
        {
            rigidBody.MovePosition(rigidBody.position + velocity * Time.fixedDeltaTime);
        }
        
    }

    private void Update()
    {
        if (isAlive)
        {


            if (Input.GetMouseButtonDown(0) && currentItems[currentSlotNumber] > 0)
            {

                if (currentItems[currentSlotNumber] >= itemLauncherPrefabList[currentSlotNumber].requiredAmount)
                {
                    currentItems[currentSlotNumber] -= itemLauncherPrefabList[currentSlotNumber].requiredAmount;
                    item = Instantiate(itemLauncherPrefabList[currentSlotNumber], launchPosition.position, transform.rotation);

                    item.transform.parent = launchPosition;
                }
                
               
                //ballProjectile.launcher = launchPosition;

            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                PickUpItem();
            }

            velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * movementSpeed;


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            LayerMask groundMask = LayerMask.GetMask("MousePosition");
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 100, groundMask))
            {

                Vector3 groundPosition = new Vector3(hitInfo.point.x, 0, hitInfo.point.z);

                if (Vector3.Magnitude(groundPosition - transform.position) > minRotationDistance)
                {
                    transform.LookAt(groundPosition + Vector3.up * transform.position.y);
                }

            }
        }
    }



    public void AddNearItem(ItemObject item)
    {
        nearItems.Add(item);
    }

    public void RemoveNearItem(ItemObject item)
    {
        if (nearItems.Contains(item))
        {
            nearItems.Remove(item);
        }
       
    }

    public void PickUpItem()
    {

        if (nearItems.Count > 0)
        {

            currentItems[(int)nearItems[0].itemType]++;

            nearItems[0].PickedUpItem();
            nearItems.RemoveAt(0);

        }
    }


    // 1 hit
    public void TakeDamage(Vector3 forceDirection = new Vector3())
    {
        isAlive = false;

        rigidBody.constraints = RigidbodyConstraints.None;
        if (!isAlive)
        {
            rigidBody.AddForce(forceDirection * impactMultiplier, ForceMode.Impulse);

            if (OnPlayerDeath != null)
            {
                OnPlayerDeath();              
            }
        }
    }
}
