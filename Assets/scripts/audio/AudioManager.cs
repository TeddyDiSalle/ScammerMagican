using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource shuffleSource;
    private AudioSource stingerSource;

    private AudioClip ballDropSfx;
    private AudioClip cupShuffleSfx;
    private AudioClip uiChangeSfx;
    private AudioClip uiSelectSfx;

    private AudioClip gameplayMusic;
    private AudioClip selectionMusic;
    private AudioClip menuMusic;
    private AudioClip winStinger;
    private AudioClip loseStinger;

    // Easy game-jam volume controls.
    private const float MusicVolume = 0.40f;
    private const float SfxVolume = 0.85f;
    private const float UiVolume = 0.75f;
    private const float StingerVolume = 0.95f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null)
            return;

        GameObject audioObject = new GameObject("AudioManager");
        audioObject.AddComponent<AudioManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAudioSources();
        LoadAudioClips();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene());
        StartCoroutine(AttachButtonSoundsNextFrame());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance = null;
        }
    }

    private void CreateAudioSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = MusicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
        sfxSource.volume = SfxVolume;

        shuffleSource = gameObject.AddComponent<AudioSource>();
        shuffleSource.playOnAwake = false;
        shuffleSource.loop = true;
        shuffleSource.volume = SfxVolume;

        stingerSource = gameObject.AddComponent<AudioSource>();
        stingerSource.playOnAwake = false;
        stingerSource.loop = false;
        stingerSource.volume = StingerVolume;
    }

    private void LoadAudioClips()
    {
        ballDropSfx = Resources.Load<AudioClip>("Audio/sfx_ball-drop");
        cupShuffleSfx = Resources.Load<AudioClip>("Audio/sfx_cup-shuffling");
        uiChangeSfx = Resources.Load<AudioClip>("Audio/sfx_ui_change-selection");
        uiSelectSfx = Resources.Load<AudioClip>("Audio/sfx_ui_select");

        selectionMusic = Resources.Load<AudioClip>("Audio/music_selection-loop");
        menuMusic = Resources.Load<AudioClip>("Audio/music_menu-loop");
        gameplayMusic = Resources.Load<AudioClip>("Audio/music_ball-cup-shuffle");

        winStinger = Resources.Load<AudioClip>("Audio/music-demo_win-stinger");
        loseStinger = Resources.Load<AudioClip>("Audio/music-demo_lose-stinger");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene);
        StartCoroutine(AttachButtonSoundsNextFrame());
    }

    private void PlayMusicForScene(Scene scene)
    {
        string sceneName = scene.name.ToLowerInvariant();

        if (sceneName.Contains("menu"))
            PlayMenuMusic();
        else
            PlayGameplayMusic();
    }

    private IEnumerator AttachButtonSoundsNextFrame()
    {
        // Let the scene finish creating its UI first.
        yield return null;

        Button[] buttons = FindObjectsOfType<Button>(true);

        foreach (Button button in buttons)
        {
            if (button != null && button.GetComponent<UIButtonSFX>() == null)
                button.gameObject.AddComponent<UIButtonSFX>();
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = MusicVolume;
        musicSource.Play();
    }

    public void PlayGameplayMusic()
    {
        PlayMusic(gameplayMusic);
    }

    public void PlaySelectionMusic()
    {
        PlayMusic(selectionMusic);
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayBallDrop()
    {
        PlayOneShot(ballDropSfx, SfxVolume);
    }

    public void StartShuffleSfx()
    {
        if (cupShuffleSfx == null || shuffleSource == null)
            return;

        if (shuffleSource.isPlaying && shuffleSource.clip == cupShuffleSfx)
            return;

        shuffleSource.Stop();
        shuffleSource.clip = cupShuffleSfx;
        shuffleSource.loop = true;
        shuffleSource.volume = SfxVolume;
        shuffleSource.Play();
    }

    public void StopShuffleSfx()
    {
        if (shuffleSource != null)
            shuffleSource.Stop();
    }

    public void PlayUIChange()
    {
        PlayOneShot(uiChangeSfx, UiVolume);
    }

    public void PlayUISelect()
    {
        PlayOneShot(uiSelectSfx, UiVolume);
    }

    public void PlayWinStinger()
    {
        PlayStinger(winStinger);
    }

    public void PlayLoseStinger()
    {
        PlayStinger(loseStinger);
    }

    private void PlayStinger(AudioClip clip)
    {
        if (clip == null || stingerSource == null)
            return;

        stingerSource.Stop();
        stingerSource.clip = clip;
        stingerSource.loop = false;
        stingerSource.volume = StingerVolume;
        stingerSource.Play();
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null || sfxSource == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }
}
