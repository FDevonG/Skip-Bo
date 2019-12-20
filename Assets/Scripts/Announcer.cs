using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Announcer : MonoBehaviour
{
    public static Announcer Instance { get; private set; }
    [SerializeField] AudioSource announcerVoice;
    [SerializeField] AudioClip bye;
    public AudioClip[] hellos = new AudioClip[6];
    public AudioClip[] playerTurns = new AudioClip[4];
    public AudioClip[] compliments = new AudioClip[12];
    public AudioClip[] youWon = new AudioClip[6];
    public AudioClip[] cant = new AudioClip[4];
    [SerializeField] AudioClip[] errors = new AudioClip[3];
    [SerializeField] AudioClip betterLuck;

    public bool welcomePlayed = false;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void Welcome() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = hellos[Random.Range(0, hellos.Length)];
            announcerVoice.Play();
        }
    }

    public void AnnouncePlayerTurn(int turnIndex) {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = playerTurns[turnIndex];
            announcerVoice.Play();
        }
    }

    public void PayCompliment() {
        if (Sounds.IsSoundEffectsActive()) {
            if (!announcerVoice.isPlaying) {
                announcerVoice.clip = compliments[Random.Range(0, compliments.Length)];
                announcerVoice.Play();
            }
        }
    }

    public void Cant() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = cant[Random.Range(0, cant.Length)];
            announcerVoice.Play();
        }
    }

    public void AnnouncerAnError() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = errors[Random.Range(0, errors.Length)];
            announcerVoice.Play();
        }
    }

    public void YouWon() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = youWon[Random.Range(0, youWon.Length)];
            announcerVoice.Play();
        }
    }

    public void YouLost() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = betterLuck;
            announcerVoice.Play();
        }
    }

    public void GoodBye() {
        if (Sounds.IsSoundEffectsActive()) {
            announcerVoice.clip = bye;
            announcerVoice.Play();
        }
    }
}
