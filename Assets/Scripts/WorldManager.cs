using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Material[] skyMaterials;
    [SerializeField] private Vector3[] lightPositions;
    [SerializeField] FollowCamera followCameraScript;
    [SerializeField] private GameObject lightObject;
    private int selectedSkyIndex;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] public GameObject gemParticle;
    [SerializeField] public GameObject winParticle;
    [SerializeField] private Button avatarButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject stopButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    private IEnumerator timer;
    [SerializeField] private int playTimeDuration = 180;
    [SerializeField] private GameObject gemSet;
    private int remainingTime;
    // Start is called before the first frame update
    void Start()
    {

        timer = CountTime();

        if (GameManager.Instance != null){

            // instantiate world
            GameObject world = GameManager.Instance.worlds[GameManager.Instance.selectedWorldIndex];
            Instantiate(world, Vector3.zero, world.transform.rotation);
            
            // apply sky
            selectedSkyIndex = GameManager.Instance.selectedSkyIndex;
            ApplyCurrentSky();

            // get gemsSet
            gemSet = GameObject.Find("Gems");
            gemSet.SetActive(false);

            // spawn player
            GameObject player = Instantiate(GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex]);
            player.AddComponent<PlayerController>();

            // assign camera
            followCameraScript.player = player;

            scoreText.gameObject.SetActive(false);
        }
    }
    
    public void ClickUISound(){
        if (AudioManager.Instance != null){
            AudioManager.Instance.ClickUISound();
        }
    }
    public void ClickChangeSky(){
        selectedSkyIndex++;
        if (selectedSkyIndex > skyMaterials.Length -1){
            selectedSkyIndex = 0;
        }
        if (GameManager.Instance != null){
            GameManager.Instance.selectedSkyIndex = selectedSkyIndex;
        }
        ApplyCurrentSky();
    }

    void ApplyCurrentSky(){
        RenderSettings.skybox = skyMaterials[selectedSkyIndex];
        DynamicGI.UpdateEnvironment();
        lightObject.transform.rotation = Quaternion.Euler(lightPositions[selectedSkyIndex]);
    }

    public void ExitToMenu(){
        SceneManager.LoadScene(0);
    }
    
    // GAME
    public void StartPlay(){
        GameManager.Instance.isPlaying = true;

        gemSet.SetActive(true);
        UpdateScoreText();

        playButton.SetActive(false);
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        stopButton.SetActive(true);
        avatarButton.interactable = false;

        remainingTime = playTimeDuration;
        StartCoroutine(timer);
        StartCoroutine(ShowStartPlayMessage());
    }
    IEnumerator ShowStartPlayMessage(){
        gameOverText.text = "GET ALL THE GEMS!";
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3.5f);
        gameOverText.gameObject.SetActive(false);
    }
    IEnumerator CountTime(){
        while (true){
            
            string str = Mathf.Floor(remainingTime/60).ToString();
            str += ":";
            float remtim = remainingTime % 60;
            if (remtim<10){
                str += "0";
            }
            str += remtim;
            timerText.text = str;

            if (remainingTime == 0){
                // GAME OVER
                AudioManager.Instance.PlaySound("lose");
                gameOverText.text = "GAME OVER";
                StartCoroutine(ShowGameOver());
                StopPlay();
            }

            yield return new WaitForSeconds(1);
            remainingTime--;
        }
    }

    IEnumerator ShowGameOver(){
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.5f);
        gameOverText.gameObject.SetActive(false);
    }

    public void StopPlay(){
        GameManager.Instance.isPlaying = false;
        foreach(GameObject gemName in GameManager.Instance.catchedGems){
            gemName.SetActive(true);
        }
        GameManager.Instance.catchedGems.Clear();
        gemSet.SetActive(false);

        playButton.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        stopButton.SetActive(false);
        avatarButton.interactable = true;

        StopCoroutine(timer);
    }
    public void UpdateScoreText(){
        int remainingGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        int catchedGems = GameManager.Instance.catchedGems.Count;
        scoreText.text = catchedGems + "/" + (remainingGems + catchedGems);

        if (remainingGems == 0){
            // WIN GAME
            AudioManager.Instance.PlaySound("win");

            string rank;
            if (remainingTime > 60){
                rank = "A+";
            } else if (remainingTime > 50){
                rank = "A";
            } else if (remainingTime > 35){
                rank = "B+";
            } else if (remainingTime > 25){
                rank = "B";
            } else if (remainingTime > 15){
                rank = "C+";
            } else {
                rank = "C";
            }

            gameOverText.text = "CONGRATULATIONS\nYOUR RANK: "+rank;
            StartCoroutine(ShowGameOver());
            StopPlay();
        }

    }
}
