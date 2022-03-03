using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 4.5f;
    private float rotationSpeed = 90;
    private Rigidbody playerRb;
    private Animator playerAnim;
    private AudioSource playerAudio;
    private WorldManager worldManager;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawnPosition;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputY = Input.GetAxis("Vertical");
        if (inputY == 0){
            playerAnim.SetBool("walking_b", false);
        } else {
            playerAnim.SetBool("walking_b", true);
        }
        Vector3 moveVector = transform.forward * inputY * speed;
        playerRb.velocity = new Vector3(moveVector.x, playerRb.velocity.y, moveVector.z);

        float directionX = Input.GetAxis("Horizontal");
        transform.Rotate(0,Time.deltaTime * rotationSpeed * directionX, 0);
    }

    void OnCollisionEnter(Collision collision){
        
        if (collision.gameObject.CompareTag("Gem")){
            playerAudio.PlayOneShot(worldManager.getGemSound);

            GameObject particle = worldManager.gemParticle;
            particle.transform.position = collision.transform.position;
            particle.GetComponent<ParticleSystem>().Play();

            collision.gameObject.SetActive(false);
            GameManager.Instance.catchedGems.Add(collision.gameObject);

            if (GameObject.FindGameObjectWithTag("Gem") == null){
                GameObject winParticle = worldManager.winParticle;
                winParticle.transform.position = collision.transform.position;
                winParticle.GetComponent<ParticleSystem>().Play();
            }

            worldManager.UpdateScoreText();
        }
    }
}
