using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviour
{
    public float value = 100f;

    PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public void TakeDamage(float damage)
    {
        value -= damage;

        PV.RPC("RPCSyncHealth", RpcTarget.All, value);
        
        if(value < 0)
            value = 0;
    }

    [PunRPC]
    void RPCSyncHealth(float newValue)
    {
        value = newValue;
    }
}
