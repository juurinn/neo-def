using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays random music from a playlist.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {

    static public MusicPlayer instance;

    [Tooltip("All the music clips go here.")]
    public AudioClip[] musicClips;

    [Tooltip("How long is the delay between switching to a new clip, in seconds.")]
    public float delayBetweenClips = 4f;

    [Tooltip("Disallow music player to play same clip back to back.")]
    public bool noBackToBackClips = true;

    /// <summary>
    /// Current audio clip that is playing.
    /// </summary>
    public AudioClip CurrentClip {
        get { return m_AudioSource.clip; }
    }

    private AudioSource m_AudioSource;
    private int m_LastIndexPlayed;
    private int m_RandomIndex;
    private int m_Iterations;
    private float m_Timer;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.loop = false;
        m_LastIndexPlayed = -1;
        m_RandomIndex = -1;
        m_Timer = 0f;
    }

    private void Update() {
        if (!m_AudioSource.isPlaying) {
            m_Timer += Time.unscaledDeltaTime;
            if (m_Timer >= delayBetweenClips) {
                m_AudioSource.clip = GetRandomClip();
                m_AudioSource.Play();
                m_Timer = 0f;
            }
        }
    }

    /// <summary>
    /// Get a random music clip that is not the same as the last played clip.
    /// </summary>
    /// <returns> Random audio clip. </returns>
    private AudioClip GetRandomClip() {
        if (!noBackToBackClips) return musicClips[Random.Range(0, musicClips.Length)];
        m_Iterations = 0;
        while (m_RandomIndex == m_LastIndexPlayed) {
            m_RandomIndex = Random.Range(0, musicClips.Length);
            m_Iterations++;
            if (m_Iterations > 1000) break;
        }
        m_LastIndexPlayed = m_RandomIndex;
        return musicClips[m_RandomIndex];
    }

}
