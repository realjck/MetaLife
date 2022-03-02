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
    [SerializeField] private GameObject gemSet;
    [SerializeField] public AudioClip getGemSound;
    [SerializeField] private Button avatarButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject stopButton;
    [SerializeField] private TextMeshProUGUI timerText;
    private IEnumerator timer;
    private int elapsedTime;
    // Start is called before the first frame update
    void Start()
    {

        timer = CountTime();

        if (GameManager.Instance != null){
            
            // get sky
            selectedSkyIndex = GameManager.Instance.selectedSkyIndex;
            ApplyCurrentSky();

            // spawn player
            GameObject player = Instantiate(GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex]);
            player.AddComponent<PlayerController>();

            // assign camera
            followCameraScript.player = player;

            gemSet.SetActive(false);
            scoreText.gameObject.SetActive(false);
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

        StartCoroutine(timer);
    }
    IEnumerator CountTime(){
        while (true){
            
            string str = Mathf.Floor(elapsedTime/60).ToString();
            str += ":";
            float remtim = elapsedTime % 60;
            if (remtim<10){
                str += "0";
            }
            str += remtim;
            timerText.text = str;

            yield return new WaitForSeconds(1);
            elapsedTime++;
        }
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
        elapsedTime = 0;
    }
    public void UpdateScoreText(){
        int remainingGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        int catchedGems = GameManager.Instance.catchedGems.Count;
        scoreText.text = catchedGems + "/" + (remainingGems + catchedGems);
    }
}
