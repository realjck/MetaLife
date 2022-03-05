using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0, 0);
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float rotationSpeed = 80f;
    private float jumpForce = 4.5f;
    private int groundCollisionsCounter;
    private Rigidbody playerRb;
    private bool isBackwardPressed;
    private Animator playerAnim;
    private WorldManager worldManager;
    private bool isWalking;
    private bool isJumping;
    private FollowCamera cameraFollower;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = spawnPosition;
        playerRb = GetComponent<Rigidbody>();
        playerAnim = GetComponent<Animator>();

        worldManager = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        cameraFollower = GameObject.Find("Main Camera").GetComponent<FollowCamera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputForward = inputY;
        float inputSide = inputX;

        // walk anim & sound
        if (inputForward == 0){
            playerAnim.SetBool("walking_b", false);

            if (isWalking){
                isWalking = false;
                StopSound();
            }
        } else {
            playerAnim.SetBool("walking_b", true);

            if (!isWalking){
                isWalking = true;
                PlaySound("steps");
            }
        }
        
        // manage 180° turns
        if (inputForward < 0){
            if (!isBackwardPressed){
                isBackwardPressed = true;
                cameraFollower.direction = 2;
                transform.Rotate(0,180,0);
            }
        } else if (inputForward > 0) {
            if (isBackwardPressed){
                isBackwardPressed = false;
                if (inputSide == 0){
                    transform.Rotate(0,180,0);
                }
            }
            cameraFollower.direction = 0;
        }

        // move forward
        Vector3 moveVector = transform.forward * Mathf.Abs(inputForward) * speed;
        playerRb.velocity = new Vector3(moveVector.x, playerRb.velocity.y, moveVector.z);

        // rotate
        if (isBackwardPressed){
            inputSide *= -1;
        }
        float currentSpeed = playerRb.velocity.magnitude;
        float currentRotationSpeed = rotationSpeed + speed*15 - currentSpeed*15;
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0,currentRotationSpeed * inputSide,0) * Time.fixedDeltaTime);
        playerRb.MoveRotation(playerRb.rotation * deltaRotation);
        
    }

    void Update(){

        // jump
        if ((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && (groundCollisionsCounter != 0)){
            isJumping = true;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // sound
            StopSound();
            PlaySound("jump");
            // anim
            playerAnim.SetTrigger("jump_t");
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.CompareTag("Gem")){
            // GET GEM
            PlaySound("getGem");

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
                    PlaySound("land");
                    isJumping = false;
                    if (isWalking){
                        PlaySound("steps");
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

    // for debug (check if audiomanager is here)
    private void PlaySound(string sound){
        if (AudioManager.Instance != null){
            switch(sound){
                case "steps":
                    AudioManager.Instance.PlayLoopSound(sound);
                    break;
                default:
                    AudioManager.Instance.PlaySound(sound);
                    break;
            }
        }
    }
    private void StopSound(){
        if (AudioManager.Instance != null){
            AudioManager.Instance.StopSound();
        }
    }
}
