using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldMenuManager : MonoBehaviour
{
    [SerializeField] private Vector3 worldPosition;
    [SerializeField] private TextMeshProUGUI worldTitleText;
    private GameObject world;
    // Start is called before the first frame update
    void Start()
    {
        ShowCurrentWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            ClickLeft();
        } else if (Input.GetKeyDown(KeyCode.RightArrow)){
            ClickRight();
        } else if (Input.GetKeyDown(KeyCode.Space)){
            ClickRez();
        }
    }

    void ShowCurrentWorld(){
        if (world != null){
            Destroy(world);
        }
        world = Instantiate(GameManager.Instance.worlds[GameManager.Instance.selectedWorldIndex]);
        world.transform.position = worldPosition;
        GameObject.Find("Gems").SetActive(false);

        worldTitleText.text = GameManager.Instance.worldTitles[GameManager.Instance.selectedWorldIndex];
    }

    public void ClickLeft(){
        AudioManager.Instance.ClickUISound();
        GameManager.Instance.selectedWorldIndex--;
        if (GameManager.Instance.selectedWorldIndex < 0){
            GameManager.Instance.selectedWorldIndex = GameManager.Instance.worlds.Length -1;
        }
        ShowCurrentWorld();
    }
    public void ClickRight(){
        AudioManager.Instance.ClickUISound();
        GameManager.Instance.selectedWorldIndex++;
        if (GameManager.Instance.selectedWorldIndex > GameManager.Instance.worlds.Length -1){
            GameManager.Instance.selectedWorldIndex = 0;
        }
        ShowCurrentWorld();
    }

    public void ClickRez(){
        AudioManager.Instance.ClickUISound();
        SceneManager.LoadScene(2);
    }
}
