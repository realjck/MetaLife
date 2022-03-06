using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] characters;
    public GameObject[] worlds;
    public string[] worldTitles;
    public int selectedCharacterIndex;
    public Material[] skyMaterials;
    public Vector3[] lightPositions;
    public int selectedSkyIndex;
    public int selectedWorldIndex;
    public bool isWorldRezzed;
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
