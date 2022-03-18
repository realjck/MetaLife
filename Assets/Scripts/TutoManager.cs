using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutoManager : MonoBehaviour
{
    public void ClickRez(){
        AudioManager.Instance.ClickUISound();
        SceneManager.LoadScene(2);
    }
}
