using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveableObject : MonoBehaviourPunCallbacks, Photon.Pun.IPunObservable
{
    private Vector3 syncPos = Vector3.zero;
    private Quaternion syncRot = Quaternion.identity;
    private Rigidbody myBody;
    private PhotonView PV;
    private bool pickable;

    void Start()
    {
        myBody = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
    }
    
    void FixedUpdate()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            transform.position = Vector3.Lerp(transform.position, syncPos, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, syncRot, 0f);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            syncPos = (Vector3) stream.ReceiveNext();
            syncRot = (Quaternion) stream.ReceiveNext();
            
        }
    }
    public void ChangeGravity(bool gravity)
    {
        PV.RPC("RPC_ChangeGravity", RpcTarget.All, gravity);
    }

    public void ChangeVelocity(Vector3 vel)
    {
        PV.RPC("RPC_ChangeVelocity", RpcTarget.All, vel);
    }

    [PunRPC]
    void RPC_ChangeGravity(bool gravity)
    {
        myBody.useGravity = gravity;
    }

    [PunRPC]
    void RPC_ChangeVelocity(Vector3 vel)
    {
        myBody.velocity = vel;
    }

    [PunRPC]
    void RPC_SetPickable(bool temp)
    {

    }

}  
