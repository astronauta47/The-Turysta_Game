using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Test : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] string text;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("Send");
        if (stream.IsWriting)
        {
            stream.SendNext(text); Debug.Log("Send");
        }
        else
        {
            text = (string)stream.ReceiveNext(); Debug.Log("Send");
        }
    }
}
