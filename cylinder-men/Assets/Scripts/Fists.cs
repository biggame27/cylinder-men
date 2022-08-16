using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fists : Item
{
    public override void Use()
    {
    //     Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
    //     ray.origin = cam.transform.position;
    //     if(Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         if(hit.transform.CompareTag("Enemy"))
    //         {
    //             Health health = hit.transform.GetComponent<Health>();

    //             if(health == null)
    //             {
    //                 throw new System.Exception("Cannot find Health Component On Enemy");
    //             }
    //             else
    //             {
    //                 health.TakeDamage(((GunInfo)itemInfo).damage);
    //             }
    //         }
    //         hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
    //         PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
    //     }
    }
}
