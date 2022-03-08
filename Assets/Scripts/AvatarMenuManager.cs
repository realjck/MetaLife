using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class AvatarMenuManager : MonoBehaviour
{
    [SerializeField] private Vector3 characterPosition;
    [SerializeField] private TextMeshProUGUI versionText;
    private GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        ShowCurrentCharacter();
        if (!GameManager.Instance.isWorldRezzed){
            versionText.text = "v" + Application.version;
        } else {
            versionText.gameObject.SetActive(false);
        }
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

    void ShowCurrentCharacter(){
        if (character != null){
            Destroy(character);
        }
        character = Instantiate(GameManager.Instance.characters[GameManager.Instance.selectedCharacterIndex]);
        character.transform.position = characterPosition;
        character.GetComponent<Rigidbody>().useGravity = false;
    }

    public void ClickLeft(){
        AudioManager.Instance.ClickUISound();
        GameManager.Instance.selectedCharacterIndex--;
        if (GameManager.Instance.selectedCharacterIndex < 0){
            GameManager.Instance.selectedCharacterIndex = GameManager.Instance.characters.Length -1;
        }
        ShowCurrentCharacter();
    }
    public void ClickRight(){
        AudioManager.Instance.ClickUISound();
        GameManager.Instance.selectedCharacterIndex++;
        if (GameManager.Instance.selectedCharacterIndex > GameManager.Instance.characters.Length -1){
            GameManager.Instance.selectedCharacterIndex = 0;
        }
        ShowCurrentCharacter();
    }

    public void ClickRez(){
        AudioManager.Instance.ClickUISound();
        if (GameManager.Instance.isWorldRezzed){
            SceneManager.LoadScene(2);
        } else {
            SceneManager.LoadScene(1);
        }
    }
}
