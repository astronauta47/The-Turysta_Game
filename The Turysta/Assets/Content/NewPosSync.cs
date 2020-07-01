using Photon.Pun;
using UnityEngine;

public class NewPosSync : MonoBehaviour, IPunObservable
{
    public Vector3 position;
    Quaternion rotation;

    PhotonView photonView;

    public void StartPos()
    {
        position = Vector3.zero;
        rotation = Quaternion.Euler(0, 0, 0);
    }

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        StartPos();
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 8f);
        }

    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if (photonView.IsMine)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }


    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new System.NotImplementedException();
    }
}
