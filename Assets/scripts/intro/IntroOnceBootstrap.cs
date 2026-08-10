using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroOnceBootstrap : MonoBehaviour
{
    private static bool shownThisRun;

    public static bool IsPlaying { get; private set; }

    private const string FramePath = "Intro/IntroScene";

    // 24-frame supplied GIF with doubled timing: about 6 seconds total.
    private readonly float[] frameDurations = new float[]
    {
        0.250f, 0.250f, 0.250f, 0.250f, 0.250f, 0.250f,
        0.250f, 0.250f, 0.250f, 0.250f, 0.250f, 0.250f,
        0.250f, 0.250f, 0.250f, 0.250f, 0.250f, 0.250f,
        0.250f, 0.250f, 0.250f, 0.250f, 0.250f, 0.250f
    };

    private Sprite[] frames;
    private Image introImage;
    private CanvasGroup canvasGroup;

    private float previousTimeScale;

    private GameObject insuranceEncounter;
    private bool insuranceWasActive;

    private Coroutine introRoutine;
    private bool finishing;
    private bool introCanBeSkipped;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        shownThisRun = false;
        IsPlaying = false;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ShowAtGameStart()
    {
        if (shownThisRun)
            return;

        shownThisRun = true;

        GameObject host = new GameObject("IntroOnce");
        DontDestroyOnLoad(host);
        host.AddComponent<IntroOnceBootstrap>();
    }

    private void Awake()
    {
        IsPlaying = true;

        frames = Resources.LoadAll<Sprite>(FramePath);

        if (frames == null || frames.Length == 0)
        {
            Debug.LogError("IntroOnce: no frames found at Resources/" + FramePath);
            IsPlaying = false;
            Destroy(gameObject);
            return;
        }

        Array.Sort(frames, (a, b) => string.CompareOrdinal(a.name, b.name));

        previousTimeScale = Time.timeScale;

        // Freeze GAMEPLAY, but DO NOT pause the AudioListener.
        // This allows the background music to play during the intro.
        Time.timeScale = 0f;

        insuranceEncounter = GameObject.Find("InsuranceEncounter");
        if (insuranceEncounter != null)
        {
            insuranceWasActive = insuranceEncounter.activeSelf;
            insuranceEncounter.SetActive(false);
        }

        BuildOverlay();
        introRoutine = StartCoroutine(PlayIntro());
        StartCoroutine(StartIntroMusic());
    }

    private IEnumerator StartIntroMusic()
    {
        // AudioManager may initialize during the same startup frame.
        // Give it a few realtime frames to appear.
        float timeout = 1f;
        float elapsed = 0f;

        while (AudioManager.Instance == null && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayGameplayMusic();
    }

    private void Update()
    {
        if (!finishing && introCanBeSkipped && Input.anyKeyDown)
            SkipIntro();
    }

    private void BuildOverlay()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 32767;

        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        GameObject imageObject = new GameObject(
            "IntroAnimation",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(AspectRatioFitter)
        );

        imageObject.transform.SetParent(transform, false);

        RectTransform rect = imageObject.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        introImage = imageObject.GetComponent<Image>();
        introImage.sprite = frames[0];
        introImage.color = Color.white;
        introImage.raycastTarget = true;

        AspectRatioFitter fitter = imageObject.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 16f / 9f;
    }

    private IEnumerator PlayIntro()
    {
        // Avoid accidental instant skip from the input/focus event that launched play.
        yield return new WaitForSecondsRealtime(0.15f);
        introCanBeSkipped = true;

        for (int i = 0; i < frames.Length; i++)
        {
            introImage.sprite = frames[i];

            float hold =
                i < frameDurations.Length
                    ? frameDurations[i]
                    : 0.25f;

            yield return new WaitForSecondsRealtime(hold);
        }

        yield return new WaitForSecondsRealtime(0.20f);
        yield return StartCoroutine(FadeOutAndFinish());
    }

    private void SkipIntro()
    {
        if (finishing)
            return;

        if (introRoutine != null)
            StopCoroutine(introRoutine);

        StartCoroutine(FadeOutAndFinish());
    }

    private IEnumerator FadeOutAndFinish()
    {
        if (finishing)
            yield break;

        finishing = true;
        introCanBeSkipped = false;

        float fadeDuration = 0.20f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (canvasGroup != null)
                canvasGroup.alpha =
                    1f - Mathf.Clamp01(elapsed / fadeDuration);

            yield return null;
        }

        FinishIntro();
    }

    private void FinishIntro()
    {
        if (insuranceEncounter != null && insuranceWasActive)
            insuranceEncounter.SetActive(true);

        Time.timeScale = previousTimeScale;
        IsPlaying = false;

        Debug.Log("Intro finished. Gameplay starting.");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        IsPlaying = false;

        if (Time.timeScale == 0f)
            Time.timeScale = previousTimeScale;
    }
}
