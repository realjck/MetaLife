using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGem : MonoBehaviour
{
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * speed, 0);
    }
}
