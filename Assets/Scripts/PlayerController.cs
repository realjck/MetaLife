using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 4.5f;
    private float rotationSpeed = 90;
    private float jumpForce = 4.5f;
    private int groundCollisionsCounter;
    private Rigidbody playerRb;
    private bool isBackwardPressed;
    private Animator playerAnim;
    private WorldManager worldManager;
    private bool isWalking;
    private bool isJumping;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawnPosition;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();

        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputY = Input.GetAxis("Vertical");
        float inputX = Input.GetAxis("Horizontal");

        // walk anim & sound
        if (inputY == 0){
            playerAnim.SetBool("walking_b", false);

            if (isWalking){
                isWalking = false;
                AudioManager.Instance.StopSound();
            }
        } else {
            playerAnim.SetBool("walking_b", true);

            if (!isWalking){
                isWalking = true;
                AudioManager.Instance.PlayLoopSound("steps");
            }
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
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && (groundCollisionsCounter != 0)){
            isJumping = true;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // sound
            AudioManager.Instance.StopSound();
            AudioManager.Instance.PlaySound("jump");
            // anim
            playerAnim.SetTrigger("jump_t");
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Gem")){
            // GET GEM
            AudioManager.Instance.PlaySound("getGem");

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

            // land sound
            if (isJumping){
                if (groundCollisionsCounter == 0){
                    AudioManager.Instance.PlaySound("land");
                    isJumping = false;
                    if (isWalking){
                        AudioManager.Instance.PlayLoopSound("steps");
                    }
                }
            }

            groundCollisionsCounter++;

            // land anim
            playerAnim.SetTrigger("land_t");  
        }
    }
    void OnCollisionExit(Collision collision){
        if (collision.gameObject.CompareTag("WalkPlane")){
            groundCollisionsCounter--;
        }
    }
}
