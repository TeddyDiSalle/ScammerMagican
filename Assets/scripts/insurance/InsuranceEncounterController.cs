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
    [SerializeField] private float interruptAfterSeconds = 30f;
    [SerializeField] private float minimumRepeatDelay = 30f;
    [SerializeField] private float maximumRepeatDelay = 90f;

    [Header("Contract salesman bounce")]
    [SerializeField] private float salesmanBounceSpeed = 700f;
    [SerializeField] private float stretchinDelay = 3f;
    [SerializeField] private float stretchinMessageDuration = 2f;

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
    private bool insurancePurchased;

    private Coroutine encounterTimer;
    private Coroutine salesmanBounceCoroutine;
    private Coroutine stretchinCoroutine;

    private Vector3 salesmanOriginalPosition;
    private bool salesmanOriginalPositionSaved;

    // We remember exactly which cup colliders were enabled so we can restore them safely.
    private readonly Dictionary<Collider2D, bool> cupColliderStates =
        new Dictionary<Collider2D, bool>();

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
        encounterTimer = StartCoroutine(FirstEncounterTimer());
    }

    private IEnumerator FirstEncounterTimer()
    {
        yield return new WaitForSeconds(interruptAfterSeconds);

        encounterTimer = null;

        if (!insurancePurchased && state == EncounterState.Waiting)
            BeginEncounter();
    }

    private IEnumerator RandomEncounterTimer()
    {
        float minDelay = Mathf.Min(minimumRepeatDelay, maximumRepeatDelay);
        float maxDelay = Mathf.Max(minimumRepeatDelay, maximumRepeatDelay);
        float randomDelay = Random.Range(minDelay, maxDelay);

        Debug.Log("Insurance Guy will return in " + randomDelay.ToString("F1") + " seconds.");

        yield return new WaitForSeconds(randomDelay);

        encounterTimer = null;

        if (!insurancePurchased && state == EncounterState.Waiting)
            BeginEncounter();
    }

    public void BeginEncounter()
    {
        if (state != EncounterState.Waiting || insurancePurchased)
            return;

        CancelEncounterTimer();

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        BlockCupClicks();

        SetPanel(salesmanVisual, true);
        SetPanel(dialoguePanel, true);
        SetPanel(decisionPanel, false);
        SetPanel(contractPanel, false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

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

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

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
        {
            contractHintText.text =
                "Sign your name below. Totally normal contract. Definitely read all of it.";
        }

        // Keep the contract above the salesman in the UI draw order.
        if (contractPanel != null)
            contractPanel.transform.SetAsLastSibling();

        StopSalesmanBounce();
        salesmanBounceCoroutine = StartCoroutine(BounceSalesmanAroundScreen());

        if (stretchinCoroutine != null)
            StopCoroutine(stretchinCoroutine);

        stretchinCoroutine = StartCoroutine(StretchinDialogue());
    }

    private IEnumerator BounceSalesmanAroundScreen()
    {
        if (salesmanVisual == null)
            yield break;

        RectTransform salesmanRect = salesmanVisual.GetComponent<RectTransform>();

        if (salesmanRect == null)
            yield break;

        RectTransform parentRect = salesmanRect.parent as RectTransform;

        if (parentRect == null)
            yield break;

        salesmanOriginalPosition = salesmanRect.localPosition;
        salesmanOriginalPositionSaved = true;

        // A diagonal DVD-screensaver style direction.
        Vector2 direction = new Vector2(1f, 0.72f).normalized;

        while (state == EncounterState.Contract)
        {
            float delta = Time.unscaledDeltaTime;
            Vector3 position = salesmanRect.localPosition;

            position.x += direction.x * salesmanBounceSpeed * delta;
            position.y += direction.y * salesmanBounceSpeed * delta;

            float halfWidth =
                salesmanRect.rect.width * Mathf.Abs(salesmanRect.localScale.x) * 0.5f;

            float halfHeight =
                salesmanRect.rect.height * Mathf.Abs(salesmanRect.localScale.y) * 0.5f;

            float leftEdge = parentRect.rect.xMin + halfWidth;
            float rightEdge = parentRect.rect.xMax - halfWidth;
            float bottomEdge = parentRect.rect.yMin + halfHeight;
            float topEdge = parentRect.rect.yMax - halfHeight;

            // If the image is larger than the available area on an axis,
            // keep that axis centered rather than letting the math flip out.
            if (leftEdge >= rightEdge)
            {
                position.x = parentRect.rect.center.x;
            }
            else if (position.x <= leftEdge)
            {
                position.x = leftEdge;
                direction.x = Mathf.Abs(direction.x);
            }
            else if (position.x >= rightEdge)
            {
                position.x = rightEdge;
                direction.x = -Mathf.Abs(direction.x);
            }

            if (bottomEdge >= topEdge)
            {
                position.y = parentRect.rect.center.y;
            }
            else if (position.y <= bottomEdge)
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

    private IEnumerator StretchinDialogue()
    {
        yield return new WaitForSecondsRealtime(stretchinDelay);

        if (state != EncounterState.Contract)
        {
            stretchinCoroutine = null;
            yield break;
        }

        if (speakerText != null)
            speakerText.text = "INSURANCE GUY";

        if (dialogueText != null)
            dialogueText.text = "Just stretchin'.";

        // Show the dialogue above both the salesman and the contract.
        SetPanel(dialoguePanel, true);

        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (dialoguePanel != null)
            dialoguePanel.transform.SetAsLastSibling();

        // IMPORTANT: the salesman keeps bouncing during this message.
        yield return new WaitForSecondsRealtime(stretchinMessageDuration);

        if (state == EncounterState.Contract)
            SetPanel(dialoguePanel, false);

        stretchinCoroutine = null;
    }

    public void FinishSigning()
    {
        if (state != EncounterState.Contract)
            return;

        if (signaturePad != null && !signaturePad.HasSignature)
        {
            if (contractHintText != null)
            {
                contractHintText.text =
                    "Nice try. The dotted line requires at least SOME scribbling.";
            }

            // He deliberately keeps bouncing until the player actually signs.
            return;
        }

        insurancePurchased = true;
        EndEncounter();
    }

    private void EndEncounter()
    {
        if (state == EncounterState.Finished)
            return;

        StopSalesmanBounce();

        if (stretchinCoroutine != null)
        {
            StopCoroutine(stretchinCoroutine);
            stretchinCoroutine = null;
        }

        SetPanel(dialoguePanel, false);
        SetPanel(decisionPanel, false);
        SetPanel(contractPanel, false);
        SetPanel(salesmanVisual, false);

        if (continueButton != null)
            continueButton.gameObject.SetActive(true);

        RestoreCupClicks();
        Time.timeScale = previousTimeScale;

        if (insurancePurchased)
        {
            // They signed. The salesman is finally gone for this play session.
            state = EncounterState.Finished;
        }
        else
        {
            // They rejected him. He can return after another random delay.
            state = EncounterState.Waiting;
            CancelEncounterTimer();
            encounterTimer = StartCoroutine(RandomEncounterTimer());
        }
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
            RectTransform salesmanRect = salesmanVisual.GetComponent<RectTransform>();

            if (salesmanRect != null)
                salesmanRect.localPosition = salesmanOriginalPosition;
        }
    }

    private void CancelEncounterTimer()
    {
        if (encounterTimer == null)
            return;

        StopCoroutine(encounterTimer);
        encounterTimer = null;
    }

    private void BlockCupClicks()
    {
        cupColliderStates.Clear();

        // The project creates cups at runtime and gives each one a ChooseCup component.
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
        CancelEncounterTimer();
        StopSalesmanBounce();

        if (state != EncounterState.Waiting && state != EncounterState.Finished)
        {
            RestoreCupClicks();
            Time.timeScale = previousTimeScale;
        }
    }
}
