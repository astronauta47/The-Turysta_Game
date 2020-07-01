using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comapss : MonoBehaviour
{
    private Vector3 northDirection;
    private Vector3 busolDirection;
    [SerializeField] Transform player;

    [SerializeField] RectTransform northLayer;
    [SerializeField] RectTransform busolLayer;

    float move;

    void Update()
    {
        ChangeDirection();
        Busola();
    }

    void ChangeDirection()
    {
        northDirection.z = player.eulerAngles.y + 180;
        northLayer.localEulerAngles = northDirection;

        busolDirection.z = player.eulerAngles.y + move + 180;
        busolLayer.localEulerAngles = busolDirection;

    }

    void Busola()
    {
        if(Input.GetMouseButtonDown(1))
        {
            move = -player.eulerAngles.y - 90;
        }

    }
}
