using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Material[] skyMaterials;
    [SerializeField] private Vector3[] lightPositions;
    [SerializeField] FollowCamera followCameraScript;
    [SerializeField] private GameObject lightObject;
    private int selectedSkyIndex;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] public AudioClip getGemSound;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null){
            
            // get sky
            selectedSkyIndex = GameManager.Instance.selectedSkyIndex;
            ApplyCurrentSky();

            // spawn player
            GameObject player = Instantiate(GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex]);
            player.AddComponent<PlayerController>();

            // assign camera
            followCameraScript.player = player;

            // disable catched gems
            foreach(string gemName in GameManager.Instance.catchedGems){
                GameObject.Find(gemName).SetActive(false);
            }

            // update score text
            UpdateScoreText();
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

    public void UpdateScoreText(){
        int remainingGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        int catchedGems = GameManager.Instance.catchedGems.Count;
        scoreText.text = catchedGems + "/" + (remainingGems + catchedGems);
    }
}
