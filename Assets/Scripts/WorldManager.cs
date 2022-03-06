using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class WorldManager : MonoBehaviour
{
    [SerializeField] FollowCamera followCameraScript;
    [SerializeField] private GameObject lightObject;
    [SerializeField] private GameObject flashlightObject;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] public GameObject gemParticle;
    [SerializeField] public GameObject winParticle;
    [SerializeField] private Button avatarButton;
    [SerializeField] private Button worldButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject difficultyButtons;
    private bool isDifficultyButtonsShowing;
    [SerializeField] private GameObject stopButton;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private int[] difficultyNbGems; // where 0 means all
    [SerializeField] private int[] difficultyPlayTimes; // in seconds
    private List<GameObject> disabledGems = new List<GameObject>();
    private IEnumerator timer;
    [SerializeField] private float gravity = 20;
    public GameObject player;
    private GameObject gemSet;
    private int remainingTime;
    // Start is called before the first frame update
    void Start()
    {

        timer = CountTime();

        if (GameManager.Instance != null){

            // instantiate world
            GameObject world = GameManager.Instance.worlds[GameManager.Instance.selectedWorldIndex];
            Instantiate(world, Vector3.zero, world.transform.rotation);
            Physics.gravity = new Vector3(0, -gravity, 0);
            
            // apply sky
            ApplyCurrentSky();

            // get gemsSet
            gemSet = GameObject.Find("Gems");
            gemSet.SetActive(false);

            // spawn player
            GameObject player = Instantiate(GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex]);
            player.AddComponent<PlayerController>();

            // assign camera
            followCameraScript.player = player;

            // assign player to flashlight
            flashlightObject.GetComponent<FollowFlashlight>().player = player;

            scoreText.gameObject.SetActive(false);

            // rez sound
            if (AudioManager.Instance != null){
                AudioManager.Instance.PlaySound("rez");
            }

            GameManager.Instance.isWorldRezzed = true;
        }
    }

    public void ClickUISound(){
        if (AudioManager.Instance != null){
            AudioManager.Instance.ClickUISound();
        }
    }
    public void ClickChangeSky(){

        difficultyButtons.SetActive(false);
        isDifficultyButtonsShowing = false;

        GameManager.Instance.selectedSkyIndex++;
        if (GameManager.Instance.selectedSkyIndex > GameManager.Instance.skyMaterials.Length -1){
            GameManager.Instance.selectedSkyIndex = 0;
        }
        ApplyCurrentSky();
    }

    void ApplyCurrentSky(){
        RenderSettings.skybox = GameManager.Instance.skyMaterials[GameManager.Instance.selectedSkyIndex];
        DynamicGI.UpdateEnvironment();
        Vector3 lightPos = GameManager.Instance.lightPositions[GameManager.Instance.selectedSkyIndex];
        if (lightPos == Vector3.zero){
            // apply night mode
            lightObject.SetActive(false);
            flashlightObject.SetActive(true);
        } else {
            // disable night mode
            lightObject.SetActive(true);
            flashlightObject.SetActive(false);
            lightObject.transform.rotation = Quaternion.Euler(lightPos);
        }
    }

    public void ClickAvatar(){
        SceneManager.LoadScene(0);
    }
    public void ClickWorld(){
        SceneManager.LoadScene(1);
    }
    public void ClickPlay(){
        if (isDifficultyButtonsShowing){
            difficultyButtons.SetActive(false);
            isDifficultyButtonsShowing = false;
        } else {
            difficultyButtons.SetActive(true);
            isDifficultyButtonsShowing = true;
        }
    }
    
    // GAME
    public void StartPlay(int level){

        int playingTime = difficultyPlayTimes[level];
        int nbgems = difficultyNbGems[level];

        difficultyButtons.SetActive(false);
        isDifficultyButtonsShowing = false;
        
        GameManager.Instance.isPlaying = true;
        gemSet.SetActive(true);

        // if nbgems, disable all farest others
        if (nbgems != 0){
            Dictionary<GameObject, float> gemsDict = new Dictionary<GameObject, float>();
            GameObject[] allGems = GameObject.FindGameObjectsWithTag("Gem");
            for (int i=0; i < allGems.Length; i++){
                float dist = (player.transform.position - allGems[i].transform.position).magnitude;
                gemsDict.Add(allGems[i], dist);
            }
            List<KeyValuePair<GameObject, float>> gemsList = new List<KeyValuePair<GameObject, float>>(gemsDict);
            gemsList.Sort(
                delegate(KeyValuePair<GameObject, float> firstPair, KeyValuePair<GameObject, float> nextPair){
                    return firstPair.Value.CompareTo(nextPair.Value);
                }
            );
            List<KeyValuePair<GameObject, float>> disabledGemsList = gemsList.GetRange(nbgems, gemsList.Count - nbgems);
            foreach (KeyValuePair<GameObject, float> element in disabledGemsList){
                disabledGems.Add(element.Key);
                element.Key.SetActive(false);
            }
        }

        UpdateScoreText();

        playButton.SetActive(false);
        scoreText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        stopButton.SetActive(true);
        avatarButton.interactable = false;
        worldButton.interactable = false;

        remainingTime = playingTime;
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
                AudioManager.Instance.PlaySound2("lose");
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
        
        // replace catched and disabled gems
        foreach(GameObject gem in GameManager.Instance.catchedGems){
            gem.SetActive(true);
        }
        GameManager.Instance.catchedGems.Clear();
        foreach(GameObject gem in disabledGems){
            gem.SetActive(true);
        }
        disabledGems.Clear();

        gemSet.SetActive(false);

        playButton.SetActive(true);
        scoreText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        stopButton.SetActive(false);
        avatarButton.interactable = true;
        worldButton.interactable = true;

        StopCoroutine(timer);
    }
    public void UpdateScoreText(){
        int remainingGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        int catchedGems = GameManager.Instance.catchedGems.Count;
        scoreText.text = catchedGems + "/" + (remainingGems + catchedGems);

        if (remainingGems == 0){
            // WIN GAME
            AudioManager.Instance.PlaySound2("win");

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
