using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    private float speed = 2;
    private float rotationSpeed = 70;
    private Animator playerAnim;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawnPosition;
        playerAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float directionY = Input.GetAxis("Vertical");
        transform.Translate(Vector3.forward * Time.deltaTime * speed * directionY);
        if (directionY == 0){
            playerAnim.SetBool("walking_b", false);
        } else {
            playerAnim.SetBool("walking_b", true);
        }

        float directionX = Input.GetAxis("Horizontal");
        transform.Rotate(0,Time.deltaTime * rotationSpeed * directionX, 0);
    }
}
