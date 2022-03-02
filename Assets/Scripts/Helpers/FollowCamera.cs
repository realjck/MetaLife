using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject player;
    [SerializeField] private Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null){
            transform.position = player.transform.TransformPoint(offset);
            transform.rotation = player.transform.rotation;
        }
    }
}
