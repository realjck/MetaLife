using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWaves : MonoBehaviour
{
    [SerializeField] private float repeat;
    [SerializeField] private float speed;
    private float startX;
    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        if (transform.position.x > startX + repeat){
            transform.position = new Vector3(startX, transform.position.y, transform.position.z);
        }
    }
}
