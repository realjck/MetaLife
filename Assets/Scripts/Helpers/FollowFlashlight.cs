using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFlashlight : MonoBehaviour
{
    public GameObject player;

    // Update is called once per frame
    void Update()
    {
        if (player != null){
            transform.position = player.transform.position;
            transform.rotation = player.transform.rotation;
        }        
    }
}
