using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer; 
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Initial Volume Settings (0.0001 ~ 1.0)")]
    // 💡 데시벨 수식은 유지하되, 게임 시작 시 적용할 기본 볼륨 수치만 낮춥니다 (0.2f -> 0.05f)
    [Range(0.0001f, 1f)] [SerializeField] private float defaultMasterVolume = 0.05f; 
    [Range(0.0001f, 1f)] [SerializeField] private float defaultBgmVolume = 0.05f;
    [Range(0.0001f, 1f)] [SerializeField] private float defaultSfxVolume = 0.05f;

    private AudioSource audioBgm;
    private AudioSource audioSfx;
    private AudioSource audioSfxLoop;

    private Dictionary<string, AudioClip> bgmContainer = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxContainer = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitSoundChannels(); 
            LoadAllSounds(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 1. 소리가 재생되기 전에 볼륨부터 '즉시' 설정 (대기시간 제거)
        InitVolumes();

        // 2. 볼륨 세팅 완료 후 BGM 재생
        PlayBGM("Menu");
    }

    private void InitVolumes()
    {
        SetMasterVolume(defaultMasterVolume);
        SetBGMVolume(defaultBgmVolume);
        SetSFXVolume(defaultSfxVolume);
        
        Debug.Log("[SoundManager] 오디오 시스템 초기 볼륨 설정 완료");
    }

    private void InitSoundChannels()
    {
        if (audioBgm == null) audioBgm = gameObject.AddComponent<AudioSource>();
        if (audioSfx == null) audioSfx = gameObject.AddComponent<AudioSource>();
        if (audioSfxLoop == null) audioSfxLoop = gameObject.AddComponent<AudioSource>();

        audioBgm.loop = true; 
        audioBgm.spatialBlend = 0f;
        audioSfx.spatialBlend = 0f;
        
        audioSfxLoop.loop = true;
        audioSfxLoop.spatialBlend = 0f;

        if (bgmGroup != null) audioBgm.outputAudioMixerGroup = bgmGroup;
        if (sfxGroup != null) audioSfx.outputAudioMixerGroup = sfxGroup;
        if (sfxGroup != null) audioSfxLoop.outputAudioMixerGroup = sfxGroup;
    }

    private void LoadAllSounds()
    {
        AudioClip[] bgmClips = Resources.LoadAll<AudioClip>("Sounds/BGM");
        foreach (AudioClip clip in bgmClips)
        {
            bgmContainer[clip.name] = clip;
        }

        AudioClip[] sfxClips = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (AudioClip clip in sfxClips)
        {
            sfxContainer[clip.name] = clip;
        }
    }

    public void PlayBGM(string clipName)
    {
        if (!bgmContainer.ContainsKey(clipName)) return;

        if (audioBgm.isPlaying && audioBgm.clip == bgmContainer[clipName]) return;

        audioBgm.clip = bgmContainer[clipName];
        audioBgm.Play();
    }

    public void StopBGM()
    {
        audioBgm.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if (!sfxContainer.ContainsKey(clipName)) return;
        audioSfx.PlayOneShot(sfxContainer[clipName]);
    }

    public void PlayLoopSFX(string clipName)
    {
        if (!sfxContainer.ContainsKey(clipName)) return;

        if (audioSfxLoop.isPlaying && audioSfxLoop.clip == sfxContainer[clipName]) return;

        audioSfxLoop.clip = sfxContainer[clipName];
        audioSfxLoop.Play();
    }

    public void StopLoopSFX()
    {
        audioSfxLoop.Stop();
        audioSfxLoop.clip = null;
    }

    #region 볼륨 제어 로직

    // 💡 기존의 데시벨 변환 로직(+6f 포함)을 100% 원본 그대로 유지합니다.
    private float LinearToDecibel(float value)
    {
        if (value <= 0.0001f) return -80f; 
        return (Mathf.Log10(value) * 20f) + 6f; 
    }

    public void SetMasterVolume(float volume)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat("MasterVol", LinearToDecibel(volume));
    }

    public void SetBGMVolume(float volume)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat("BGMVol", LinearToDecibel(volume));
    }

    public void SetSFXVolume(float volume)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat("SFXVol", LinearToDecibel(volume));
    }

    public void MenuBGM()
    {
        PlayBGM("Menu");
    }
    #endregion
}