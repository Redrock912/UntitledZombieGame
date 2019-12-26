using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{

    public float interactDistance;

    SphereCollider itemCollider;

    Player playerRef;

    public Renderer rd;
    Material outlineMaterial;

    // required amount to instantiate
    public int requiredAmount=1;

    public ItemType itemType;

    

    Shader sd;
    public enum ItemType
    {
        Rock=0, Scrap=1
    }

    


    // Start is called before the first frame update
    void Start()
    {
        itemCollider = GetComponent<SphereCollider>();
        itemCollider.radius = interactDistance;
        
        sd = rd.material.shader;
        print(sd);

        outlineMaterial = rd.material;


        
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, interactDistance);
    }


    // add to playerNearItem
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            
            playerRef = other.GetComponent<Player>();
            playerRef.AddNearItem(this);
            outlineMaterial.SetFloat("Vector1_C7AB5698", 1f);

        }

    }

    // remove from playerNearItem
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            playerRef.RemoveNearItem(this);
            playerRef = null;
            outlineMaterial.SetFloat("Vector1_C7AB5698", 0f);
        }
            
    }

    
    public void PickedUpItem()
    {
        Destroy(gameObject);
    }
}
