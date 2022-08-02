using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GravityGun : Gun
{
    //what layer to pickup
    [SerializeField] private LayerMask pickupMask;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform pickupTarget;
    [SerializeField] private float pickupRange;
    private Rigidbody currentObject;

    public override void Use()
    {
        if(currentObject)
        {
            currentObject.gameObject.GetComponent<MoveableObject>().ChangeGravity(true);
            currentObject = null;
            //thing.transform.SetParent(null);
        
        }
        else
        {
            RaycastHit hit;
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if(Physics.Raycast(ray, out hit, pickupRange, pickupMask))
            {
                currentObject = hit.rigidbody;
                currentObject.useGravity = false;
                hit.collider.gameObject.GetComponent<MoveableObject>().ChangeGravity(false);
            }
        }
    }

    void FixedUpdate()
    {
        if(currentObject)
        {
            Vector3 directionToPoint = pickupTarget.position - currentObject.position;
            float distanceToPoint = directionToPoint.magnitude;
            //alter velocity to direction of thingy
            currentObject.gameObject.GetComponent<MoveableObject>().ChangeVelocity(directionToPoint * 12f * distanceToPoint);
        }
    }

    

}
