using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    private AudioSource audioSource;
    public AudioClip jump;
    public AudioClip land;
    public AudioClip getGem;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip steps;
    public AudioClip rez;
    [Space]
    [SerializeField] private AudioClip click;
    void Awake()
    {
        if (Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start(){
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySound(string sound){
        AudioClip clip = (AudioClip)this.GetType().GetField(sound).GetValue(this);
        audioSource.PlayOneShot(clip);
    }
    public void PlayLoopSound(string sound){
        AudioClip clip = (AudioClip)this.GetType().GetField(sound).GetValue(this);
        audioSource.clip = clip;
        audioSource.Play();
    }
    public void StopSound(){
        audioSource.Stop();
    }
    public void ClickUISound(){
        audioSource.PlayOneShot(click);
    }
}
