using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    [SerializeField] private float speed;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(0, Time.deltaTime * speed, 0);
    }
}
