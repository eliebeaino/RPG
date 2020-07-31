using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }
        UpdateAnimator();
    }

    // move the player to the cursor when left click 
    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit)
        {
            GetComponent<NavMeshAgent>().destination = hit.point;
        }
    }

    // update the animator depending on speed using blend tree
    private void UpdateAnimator()
    {
        Vector3 velocity = GetComponent<NavMeshAgent>().velocity;                   // getting velocity from the nav mesh agent
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);      // converting the velocity to local space  
        float speed = localVelocity.z;                                              // storing speed value from z axis (only anxis that interests us for animation, moving forward or not)
        GetComponent<Animator>().SetFloat("Forward Speed", speed);                  // setting the blend speed value
    }
}
