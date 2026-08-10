using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InsuranceEncounterController : MonoBehaviour
{
    private enum EncounterState
    {
        Waiting,
        Intro,
        Decision,
        DeclineOutro,
        Contract,
        Finished
    }

    [Header("When the interruption happens")]
    [SerializeField] private float interruptAfterSeconds = 4f;

    [Header("Panels")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject decisionPanel;
    [SerializeField] private GameObject contractPanel;

    [Header("Dialogue")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button continueButton;

    [Header("Decision")]
    [SerializeField] private Button declineButton;
    [SerializeField] private Button acceptButton;

    [Header("Contract")]
    [SerializeField] private SignaturePad signaturePad;
    [SerializeField] private Button finishSigningButton;
    [SerializeField] private TMP_Text contractHintText;

    [Header("Optional salesman art / character")]
    [SerializeField] private GameObject salesmanVisual;

    private readonly string[] introLines =
    {
        "Whoa, whoa, whoa! Before you touch another cup, can I ask you one extremely important financial question?",
        "How insured are you right now? And please don't say 'not at all,' because I have a pamphlet specifically for that answer.",
        "Most people think cup-related incidents won't happen to them. That's exactly what people think right before a cup-related incident happens to them.",
        "We've got collision. We've got theft. We've got mysterious ball disappearance. We've even got what we call 'aggressive shuffling coverage.'",
        "And today only, I am legally allowed to describe the premium as 'pretty reasonable.'",
        "Picture this: you pick the wrong cup. Emotionally devastating. Financially? Potentially ruinous. Unless you're insured.",
        "You also get access to our 24-hour claims hotline, which is me. It's just my phone. I answer most of the time.",
        "Anyway, I don't want to pressure you. I just want to stand here and explain insurance until you make a decision.",
        "So. What do you say?"
    };

    private readonly string[] declineLines =
    {
        "Haha! Totally fair. Absolutely no problem. I respect a person who knows what they want.",
        "Just so you know, saying no today does mean surrendering our complimentary introductory accidental-cup-loss protection.",
        "Not trying to change your mind. I would never do that. I am simply giving you several new reasons to change your mind.",
        "And if the ball vanishes, the cup tips over, or an uninsured shuffle occurs, remember this exact conversation.",
        "Alright. I'll get out of your way. But if you reconsider, yell 'INSURANCE' very loudly and I may or may not hear you. Goodbye!"
    };

    private EncounterState state = EncounterState.Waiting;
    private int lineIndex;
    private float previousTimeScale = 1f;
    private Coroutine salesmanBounceCoroutine;
    private Vector3 salesmanOriginalPosition;
    private bool salesmanOriginalPositionSaved = false;

[SerializeField] private float salesmanBounceSpeed = 700f;

    // We remember exactly which cup colliders were enabled so we can restore them safely.
    private readonly Dictionary<Collider2D, bool> cupColliderStates = new Dictionary<Collider2D, bool>();

    private void Awake()
    {
        SetPanel(dialoguePanel, false);
        SetPanel(decisionPanel, false);
        SetPanel(contractPanel, false);
        SetPanel(salesmanVisual, false);

        if (continueButton != null)
            continueButton.onClick.AddListener(AdvanceDialogue);
        if (declineButton != null)
            declineButton.onClick.AddListener(DeclineInsurance);
        if (acceptButton != null)
            acceptButton.onClick.AddListener(AcceptInsurance);
        if (finishSigningButton != null)
            finishSigningButton.onClick.AddListener(FinishSigning);
    }

    private void Start()
    {
        StartCoroutine(BeginAfterDelay());
    }

    private IEnumerator BeginAfterDelay()
    {
        yield return new WaitForSeconds(interruptAfterSeconds);
        BeginEncounter();
    }

    public void BeginEncounter()
    {
        if (state != EncounterState.Waiting)
            return;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        BlockCupClicks();

        SetPanel(salesmanVisual, true);
        SetPanel(dialoguePanel, true);
        SetPanel(decisionPanel, false);
        SetPanel(contractPanel, false);

        if (speakerText != null)
            speakerText.text = "INSURANCE GUY";

        state = EncounterState.Intro;
        lineIndex = 0;
        ShowCurrentLine();
    }

    public void AdvanceDialogue()
    {
        if (state != EncounterState.Intro && state != EncounterState.DeclineOutro)
            return;

        lineIndex++;

        if (state == EncounterState.Intro && lineIndex >= introLines.Length)
        {
            ShowDecision();
            return;
        }

        if (state == EncounterState.DeclineOutro && lineIndex >= declineLines.Length)
        {
            EndEncounter();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        if (dialogueText == null)
            return;

        if (state == EncounterState.Intro)
            dialogueText.text = introLines[lineIndex];
        else if (state == EncounterState.DeclineOutro)
            dialogueText.text = declineLines[lineIndex];
    }

    private void ShowDecision()
    {
        state = EncounterState.Decision;
        SetPanel(dialoguePanel, false);
        SetPanel(decisionPanel, true);
    }

    public void DeclineInsurance()
    {
        if (state != EncounterState.Decision)
            return;

        state = EncounterState.DeclineOutro;
        lineIndex = 0;

        SetPanel(decisionPanel, false);
        SetPanel(dialoguePanel, true);
        ShowCurrentLine();
    }

    public void AcceptInsurance()
    {
        if (state != EncounterState.Decision)
            return;

        state = EncounterState.Contract;

        SetPanel(decisionPanel, false);
        SetPanel(dialoguePanel, false);
        SetPanel(contractPanel, true);
        SetPanel(salesmanVisual, true);

        if (signaturePad != null)
            signaturePad.Clear();

        if (contractHintText != null)
            contractHintText.text =
                "Sign your name below. Totally normal contract. Definitely read all of it.";

        // Keep salesman BEHIND the contract
        if (salesmanVisual != null)
            salesmanVisual.transform.SetAsFirstSibling();

        // Keep contract ABOVE salesman
        if (contractPanel != null)
            contractPanel.transform.SetAsLastSibling();

        salesmanBounceCoroutine = StartCoroutine(BounceSalesmanAroundScreen());
        StartCoroutine(StretchinDialogue());
    }

    private IEnumerator StretchinDialogue()
    {
        // Game is paused, so this MUST use realtime.
        yield return new WaitForSecondsRealtime(3f);

        if (state != EncounterState.Contract)
            yield break;

        if (speakerText != null)
            speakerText.text = "INSURANCE GUY";

        if (dialogueText != null)
            dialogueText.text = "Just stretchin'.";

        SetPanel(dialoguePanel, true);

        // Put dialogue above the bouncing salesman.
        if (dialoguePanel != null)
            dialoguePanel.transform.SetAsLastSibling();

        // He NEVER stops bouncing here.
        yield return new WaitForSecondsRealtime(2f);

        if (state == EncounterState.Contract)
            SetPanel(dialoguePanel, false);
    }

private IEnumerator BounceSalesmanAroundScreen()
{
    if (salesmanVisual == null)
        yield break;

    RectTransform salesmanRect =
        salesmanVisual.GetComponent<RectTransform>();

    RectTransform parentRect =
        salesmanRect.parent as RectTransform;

    if (salesmanRect == null || parentRect == null)
        yield break;

    // Remember exactly where he originally stood.
    salesmanOriginalPosition = salesmanRect.localPosition;
    salesmanOriginalPositionSaved = true;

    // Start diagonally.
    Vector2 direction = new Vector2(1f, 1f).normalized;

    while (state == EncounterState.Contract)
    {
        float delta = Time.unscaledDeltaTime;

        Vector3 position = salesmanRect.localPosition;

        position.x += direction.x * salesmanBounceSpeed * delta;
        position.y += direction.y * salesmanBounceSpeed * delta;

        // Size of the salesman.
        float halfWidth =
            salesmanRect.rect.width * Mathf.Abs(salesmanRect.localScale.x) / 2f;

        float halfHeight =
            salesmanRect.rect.height * Mathf.Abs(salesmanRect.localScale.y) / 2f;

        // Edges of the entire parent Canvas.
        float leftEdge = parentRect.rect.xMin + halfWidth;
        float rightEdge = parentRect.rect.xMax - halfWidth;

        float bottomEdge = parentRect.rect.yMin + halfHeight;
        float topEdge = parentRect.rect.yMax - halfHeight;

        // Bounce off left/right.
        if (position.x <= leftEdge)
        {
            position.x = leftEdge;
            direction.x = Mathf.Abs(direction.x);
        }
        else if (position.x >= rightEdge)
        {
            position.x = rightEdge;
            direction.x = -Mathf.Abs(direction.x);
        }

        // Bounce off top/bottom.
        if (position.y <= bottomEdge)
        {
            position.y = bottomEdge;
            direction.y = Mathf.Abs(direction.y);
        }
        else if (position.y >= topEdge)
        {
            position.y = topEdge;
            direction.y = -Mathf.Abs(direction.y);
        }

        salesmanRect.localPosition = position;

        yield return null;
    }
}

    private IEnumerator ContractStretchGag()
    {
        if (salesmanVisual == null)
            yield break;

        RectTransform salesmanRect = salesmanVisual.GetComponent<RectTransform>();

        if (salesmanRect == null)
            yield break;

        Vector2 startingPosition = salesmanRect.anchoredPosition;

        float duration = 3f;
        float bounceHeight = 45f;
        float bounceSpeed = 10f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float bounce =
                Mathf.Abs(Mathf.Sin(elapsed * bounceSpeed)) * bounceHeight;

            salesmanRect.anchoredPosition =
                startingPosition + Vector2.up * bounce;

            yield return null;
        }

        // Put him back exactly where he started.
        salesmanRect.anchoredPosition = startingPosition;

        // His extremely important explanation.
        if (speakerText != null)
            speakerText.text = "INSURANCE GUY";

        if (dialogueText != null)
            dialogueText.text = "Just stretchin'.";

        SetPanel(dialoguePanel, true);

        // IMPORTANT: Realtime because Time.timeScale is currently zero.
        yield return new WaitForSecondsRealtime(2f);

        // Get the dialogue box back out of the way so they can sign.
        if (state == EncounterState.Contract)
            SetPanel(dialoguePanel, false);
    }

    public void FinishSigning()
    {
        if (state != EncounterState.Contract)
            return;

        if (signaturePad != null && !signaturePad.HasSignature)
        {
            if (contractHintText != null)
                contractHintText.text =
                    "Nice try. The dotted line requires at least SOME scribbling.";

            // IMPORTANT:
            // He keeps bouncing if they haven't actually signed.
            return;
        }

        StopSalesmanBounce();

        EndEncounter();
    }

    private void StopSalesmanBounce()
    {
        if (salesmanBounceCoroutine != null)
        {
            StopCoroutine(salesmanBounceCoroutine);
            salesmanBounceCoroutine = null;
        }

        if (salesmanVisual != null && salesmanOriginalPositionSaved)
        {
            RectTransform salesmanRect =
                salesmanVisual.GetComponent<RectTransform>();

            if (salesmanRect != null)
                salesmanRect.localPosition = salesmanOriginalPosition;
        }
    }

    private void EndEncounter()
    {
        if (state == EncounterState.Finished)
            return;

        state = EncounterState.Finished;

        SetPanel(dialoguePanel, false);
        SetPanel(decisionPanel, false);
        SetPanel(contractPanel, false);
        SetPanel(salesmanVisual, false);

        RestoreCupClicks();
        Time.timeScale = previousTimeScale;
    }

    private void BlockCupClicks()
    {
        cupColliderStates.Clear();

        // Your project creates cups at runtime and gives each one a ChooseCup component.
        ChooseCup[] cups = FindObjectsOfType<ChooseCup>();
        foreach (ChooseCup cup in cups)
        {
            Collider2D[] colliders = cup.GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                if (col == null || cupColliderStates.ContainsKey(col))
                    continue;

                cupColliderStates.Add(col, col.enabled);
                col.enabled = false;
            }
        }
    }

    private void RestoreCupClicks()
    {
        foreach (KeyValuePair<Collider2D, bool> pair in cupColliderStates)
        {
            if (pair.Key != null)
                pair.Key.enabled = pair.Value;
        }

        cupColliderStates.Clear();
    }

    private static void SetPanel(GameObject obj, bool active)
    {
        if (obj != null)
            obj.SetActive(active);
    }

    private void OnDestroy()
    {
        // Prevent the editor/game from accidentally staying paused if this object is removed mid-encounter.
        if (state != EncounterState.Waiting && state != EncounterState.Finished)
        {
            RestoreCupClicks();
            Time.timeScale = previousTimeScale;
        }
    }
}
