using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(0f, 1f, -12.5f);
    }
}
