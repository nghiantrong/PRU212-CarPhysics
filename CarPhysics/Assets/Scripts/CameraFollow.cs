using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerPos;
    public float smoothness;
    public Vector3 velocity;
    

    void Update()
    {
        Vector3 target = new Vector3(playerPos.position.x, playerPos.position.y, -10f);
        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothness);
    }
}
