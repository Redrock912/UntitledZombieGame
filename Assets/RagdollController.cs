using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public List<Rigidbody> ragdollParts;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        

       foreach(var ragdoll in ragdollParts)
        {
            ragdoll.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.enabled = false;

            foreach (var ragdoll in ragdollParts)
            {
                ragdoll.isKinematic = false;
            }

        }
    }
}
