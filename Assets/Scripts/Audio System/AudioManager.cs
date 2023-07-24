using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Pool;
using Pelumi.ObjectPool;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioBank musicBank;
    [SerializeField] AudioBank soundEffectBank;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] AudioSource soundEffectPlayer;
    [SerializeField] Audio3DPlayer audio3DPlayerPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        AdvanceButton.OnAnyButtonClicked += PlayButtonSoundEffect;
    }

    private void PlayButtonSoundEffect()
    {
        PlaySoundEffect("Button Click");
    }

    private void ChangeMusic(AudioClip audioClip, bool loop)
    {
        musicPlayer.clip = audioClip;
        musicPlayer.loop = loop;
        musicPlayer.Play();
    }

    private IEnumerator PlayMusicFade(AudioClip audioClip, bool loop = true, float fadeDuration = 1.0f)
    {
        // Fade out the music player
        float elapsedTime = 0f;
        float startVolume = 1f;
        float targetVolume = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        musicPlayer.volume = 0f; // Ensure the volume is set to the target value
        musicPlayer.Stop();

        // Start the new audio clip
        musicPlayer.clip = audioClip;
        musicPlayer.loop = loop;
        musicPlayer.Play();

        // Fade in the music player
        elapsedTime = 0f;
        startVolume = 0f;
        targetVolume = 1f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicPlayer.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        musicPlayer.volume = 1f; // Ensure the volume is set to the target value
    }

    private IEnumerator StopMusicFade()
    {
        float speed = 0.05f;

        while (musicPlayer.volume >= speed)
        {
            musicPlayer.volume -= speed;
            yield return new WaitForSecondsRealtime(0.1f);
        }

        musicPlayer.Stop();
    }

    private IEnumerator BlendTwoMusicRoutine(AudioClip intro, AudioClip loopMusic, bool loop  = true)
    {
        yield return PlayMusicFade(intro, false);
        yield return new WaitForSecondsRealtime(musicPlayer.clip.length - 0.5f);
        yield return PlayMusicFade(loopMusic, loop);
    }
    public static AudioClip GetMusicClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return  Instance.musicBank.GetAsset(audioID);
    } 
    
    public static AudioClip GetSoundEffectClip(string audioID)
    {
        if (!InstanceExists()) return null;
        return Instance.soundEffectBank.GetAsset(audioID);
    }

    public AudioSource GetSfxAudioSource() => soundEffectPlayer;

    public static void PlaySoundEffect(string audioID, bool randomPitch = false)
    {
        if (Instance == null) return;
        Instance.soundEffectPlayer.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1;
        Instance.soundEffectPlayer.PlayOneShot(GetSoundEffectClip(audioID));
    }

    public static void PlayMusic(string ID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.PlayMusicFade(GetMusicClip(ID), loop));
    }

    public static void PauseMusic()
    {
        if (!InstanceExists()) return;
        Instance.musicPlayer.Pause();
    }

    public static void ResumeMusic()
    {
        if (!InstanceExists()) return;
        Instance.musicPlayer.UnPause();
    }

    public static void StopMusic()
    {
        if (!InstanceExists()) return;
        Instance.StartCoroutine(Instance.StopMusicFade());
    }

    public static void BlendTwoMusic(string startAudioID, string nextAudioID, bool loop = true)
    {
        if (!InstanceExists()) return;
        Instance.StopAllCoroutines();
        Instance.StartCoroutine(Instance.BlendTwoMusicRoutine(GetMusicClip(startAudioID), GetMusicClip(nextAudioID), loop));
    }

    public static Audio3DPlayer Play3DSoundEffect(string audioID, Vector3 position, float _dopplerLevel = 1, float _spread = 0, AudioRolloffMode _audioRolloffMode = AudioRolloffMode.Linear, float _minDistance = 1, float _maxDistance = 500)
    {
        if (Instance == null) return null;
        Audio3DPlayer audio3DPlayer = ObjectPoolManager.SpawnObject(Instance.audio3DPlayerPrefab);
        audio3DPlayer.transform.position = position;
        audio3DPlayer.PlaySoundEffect(GetSoundEffectClip(audioID), _dopplerLevel, _spread, _audioRolloffMode, _minDistance, _maxDistance);
        return audio3DPlayer;
    }

    private static bool InstanceExists()
    {
        if (Instance == null) Debug.LogError("No Audio Manager in the scene");
        return Instance != null;
    }
}