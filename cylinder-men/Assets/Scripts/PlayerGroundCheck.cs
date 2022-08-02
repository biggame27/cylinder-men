using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;

    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    //triggers when smth collides once
    void OnTriggerEnter(Collider other)
    {
        playerController.SetGroundedState(true);
    }
    //stops colliding with other
    void OnTriggerExit(Collider other)
    {
        //shouldnt matter if it is only colliding with the player
        if(other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false);
    }

    //always triggers as long as smth be colliding
    void OnTriggerStay(Collider other)
    {
        if(other.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }

    //contains information like the contact poitns and impact velocities instead of collider
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }

    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(false);
    }

    void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject == playerController.gameObject)
            return;
        playerController.SetGroundedState(true);
    }
}
