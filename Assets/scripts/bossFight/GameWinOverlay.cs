using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameWinOverlay : MonoBehaviour
{
    private static bool showing;

    private CanvasGroup canvasGroup;
    private float previousTimeScale;
    private bool canRestart;

    public static void Show()
    {
        if (showing)
            return;

        showing = true;

        GameObject host =
            new GameObject("GameWinOverlay");

        DontDestroyOnLoad(host);

        host.AddComponent<GameWinOverlay>();
    }

    void Start()
    {
        previousTimeScale = Time.timeScale;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopShuffleSfx();
            AudioManager.Instance.PlayWinStinger();
        }

        StartCoroutine(ShowAfterFinalReveal());
    }

    private IEnumerator ShowAfterFinalReveal()
    {
        // Give the final cup time to lift/fade and expose the ball.
        yield return new WaitForSecondsRealtime(0.75f);

        Time.timeScale = 0f;

        BuildOverlay();

        // Prevent the click that revealed the last cup from also
        // immediately restarting the game.
        yield return new WaitForSecondsRealtime(0.45f);
        canRestart = true;
    }

    void Update()
    {
        if (!canRestart)
            return;

        if (Input.anyKeyDown)
            RestartGame();
    }

    private void BuildOverlay()
    {
        Canvas canvas =
            gameObject.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder = 32760;

        CanvasScaler scaler =
            gameObject.AddComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight = 0.5f;

        gameObject.AddComponent<GraphicRaycaster>();

        canvasGroup =
            gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = true;

        GameObject shade =
            new GameObject(
                "Shade",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(Image)
            );

        shade.transform.SetParent(transform, false);

        RectTransform shadeRect =
            shade.GetComponent<RectTransform>();

        shadeRect.anchorMin = Vector2.zero;
        shadeRect.anchorMax = Vector2.one;
        shadeRect.offsetMin = Vector2.zero;
        shadeRect.offsetMax = Vector2.zero;

        Image shadeImage =
            shade.GetComponent<Image>();

        shadeImage.color =
            new Color(0f, 0f, 0f, 0.68f);

        GameObject titleObject =
            new GameObject(
                "WinText",
                typeof(RectTransform),
                typeof(CanvasRenderer),
                typeof(TextMeshProUGUI)
            );

        titleObject.transform.SetParent(transform, false);

        RectTransform titleRect =
            titleObject.GetComponent<RectTransform>();

        titleRect.anchorMin =
            new Vector2(0.1f, 0.25f);

        titleRect.anchorMax =
            new Vector2(0.9f, 0.75f);

        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;

        TextMeshProUGUI title =
            titleObject.GetComponent<TextMeshProUGUI>();

        title.text =
            "YOU WIN!\n\n<size=42>Press any key to play again</size>";

        title.alignment =
            TextAlignmentOptions.Center;

        title.fontSize = 92f;
        title.enableAutoSizing = true;
        title.fontSizeMin = 34f;
        title.fontSizeMax = 92f;
        title.color = Color.white;

        if (TMP_Settings.defaultFontAsset != null)
            title.font = TMP_Settings.defaultFontAsset;

        StartCoroutine(FadeOverlayIn());
    }

    private IEnumerator FadeOverlayIn()
    {
        float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            if (canvasGroup != null)
            {
                canvasGroup.alpha =
                    Mathf.Clamp01(elapsed / duration);
            }

            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    private void RestartGame()
    {
        canRestart = false;

        Time.timeScale = previousTimeScale;

        // Allow this overlay to be shown again if the player wins
        // another run after restarting.
        showing = false;

        int sceneIndex =
            SceneManager.GetActiveScene().buildIndex;

        Destroy(gameObject);
        SceneManager.LoadScene(sceneIndex);
    }

    void OnDestroy()
    {
        if (Time.timeScale == 0f)
            Time.timeScale = previousTimeScale;
    }
}
