using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] characters;
    public GameObject[] worlds;
    public int selectedCharacterIndex;
    public int selectedSkyIndex;
    public int selectedWorldIndex;
    public bool isPlaying;
    public List<GameObject> catchedGems;
    void Awake()
    {
        if (Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
