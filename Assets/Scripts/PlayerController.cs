using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 spawnPosition = new Vector3(0, 0.5f, 0);
    [SerializeField] private float speed = 4.5f;
    [SerializeField] private float runSpeed = 7.0f;
    [SerializeField] private float rotationSpeed = 80f;
    [SerializeField] private float rotationRunSpeed = 150f;
    [SerializeField] private float jumpForce = 7f;
    private int groundCollisionsCounter;
    private Rigidbody playerRb;
    private bool isBackwardPressed;
    private Animator playerAnim;
    private WorldManager worldManager;
    private bool isWalking;
    private bool hasWalked;
    private bool isStartingRunning;
    private bool isRunning;
    private bool isKeyUpReleased;
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

        worldManager.player = this.gameObject;
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        float inputForward = inputY;
        float inputSide = inputX;

        // walk anim & sound
        if (inputForward == 0 && !isStartingRunning){
            playerAnim.SetBool("walking_b", false);
            if (isWalking){
                isWalking = false;
            }
            if (hasWalked){
                StopSound();
                hasWalked = false;
            }
        } else {
            if (!isWalking){
                playerAnim.SetBool("walking_b", true);
                isWalking = true;
                PlaySound("steps");
                hasWalked = true;
            }
        }
        
        // manage 180° turns
        if (inputForward < 0){
            if (!isBackwardPressed){
                isBackwardPressed = true;
                cameraFollower.direction = 2;
                transform.Rotate(0,180,0);
            }
        } else if (inputForward >= 0) {
            isBackwardPressed = false;
            cameraFollower.direction = 0;
        }

        // move forward
        float speedToApply;
        float rotationSpeedToApply;
        if (isRunning){
            speedToApply = runSpeed;
            rotationSpeedToApply = rotationRunSpeed;
        } else {
            speedToApply = speed;
            rotationSpeedToApply = rotationSpeed;
        }
        Vector3 moveVector = transform.forward * Mathf.Abs(inputForward) * speedToApply;
        playerRb.velocity = new Vector3(moveVector.x, playerRb.velocity.y, moveVector.z);

        // rotate
        if (isBackwardPressed){
            inputSide *= -1;
        }
        float currentSpeed = playerRb.velocity.magnitude;
        float currentRotationSpeed = rotationSpeedToApply + speed*15 - currentSpeed*15;
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0,currentRotationSpeed * inputSide,0) * Time.fixedDeltaTime);
        playerRb.MoveRotation(playerRb.rotation * deltaRotation);
        
    }

    IEnumerator WaitKeyUpPressed(){
        isKeyUpReleased = true;
        yield return new WaitForSeconds(0.25f);
        isKeyUpReleased = false;
    }
    IEnumerator StartRunning(){
        isStartingRunning = true;
        yield return new WaitForSeconds(0.25f);
        isStartingRunning = false;
    }

    void Update(){

        // manage double tap up arrow to run
        if (Input.GetKeyUp(KeyCode.UpArrow)){
            if (!isRunning){
                StopAllCoroutines();
                StartCoroutine(WaitKeyUpPressed());
            } else {
                if (!isStartingRunning){
                    StopSound();
                    PlaySound("steps");
                    playerAnim.SetBool("running_b", false);
                    isRunning = false;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            if (isKeyUpReleased){
                playerAnim.SetBool("running_b", true);
                isRunning = true;
                PlaySound("runsteps");
                StartCoroutine(StartRunning());
            } else {
                if (!isStartingRunning){
                    playerAnim.SetBool("running_b", false);
                    isRunning = false;
                }
            }
        }

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
                    if (isWalking && !isRunning){
                        PlaySound("steps");
                    } else if (isWalking && isRunning){
                        PlaySound("runsteps");
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
            if (sound == "steps" || sound == "runsteps"){
                if (!gameObject.CompareTag("Silent")){
                    AudioManager.Instance.PlayLoopSound(sound);
                }
            } else {
                AudioManager.Instance.PlaySound(sound);
            }
        }
    }
    private void StopSound(){
        if (AudioManager.Instance != null){
            AudioManager.Instance.StopSound();
        }
    }
}
