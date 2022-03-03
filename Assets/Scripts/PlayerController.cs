using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 4.5f;
    private float rotationSpeed = 90;
    private float jumpForce = 4.5f;
    private bool isOnGround;
    private Rigidbody playerRb;
    private bool isBackwardPressed;
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
        float inputX = Input.GetAxis("Horizontal");

        // walk anim
        if (inputY == 0){
            playerAnim.SetBool("walking_b", false);
        } else {
            playerAnim.SetBool("walking_b", true);
        }
        
        // manage 180° turn
        if (inputY < 0){
            if (!isBackwardPressed){
                isBackwardPressed = true;
                transform.Rotate(0,180,0);
            }   
        } else {
            isBackwardPressed = false;
        }

        // move forward
        Vector3 moveVector = transform.forward * Mathf.Abs(inputY) * speed;
        playerRb.velocity = new Vector3(moveVector.x, playerRb.velocity.y, moveVector.z);

        // rotate with inputX
        transform.Rotate(0,Time.deltaTime * rotationSpeed * inputX, 0);
    }

    void Update(){
        // jump
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && isOnGround){
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // anim
            playerAnim.SetTrigger("jump_t");
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Gem")){
            // GET GEM
            playerAudio.PlayOneShot(worldManager.getGemSound);

            GameObject particle = worldManager.gemParticle;
            particle.transform.position = other.transform.position;
            particle.GetComponent<ParticleSystem>().Play();

            other.gameObject.SetActive(false);
            GameManager.Instance.catchedGems.Add(other.gameObject);

            // win the game?
            if (GameObject.FindGameObjectWithTag("Gem") == null){
                GameObject winParticle = worldManager.winParticle;
                winParticle.transform.position = other.transform.position;
                winParticle.GetComponent<ParticleSystem>().Play();
            }

            worldManager.UpdateScoreText();

        }
    }
    void OnCollisionEnter(Collision collision){

        if (collision.gameObject.CompareTag("WalkPlane")){
            isOnGround = true;
            // land anim
            playerAnim.SetTrigger("land_t");
        }
    }
    void OnCollisionExit(Collision collision){
        if (collision.gameObject.CompareTag("WalkPlane")){
            isOnGround = false;
        }
    }
}
