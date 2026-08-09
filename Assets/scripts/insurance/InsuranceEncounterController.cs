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

        if (signaturePad != null)
            signaturePad.Clear();

        if (contractHintText != null)
            contractHintText.text = "Sign your name below. Totally normal contract. Definitely read all of it.";
    }

    public void FinishSigning()
    {
        if (state != EncounterState.Contract)
            return;

        if (signaturePad != null && !signaturePad.HasSignature)
        {
            if (contractHintText != null)
                contractHintText.text = "Nice try. The dotted line requires at least SOME scribbling.";
            return;
        }

        EndEncounter();
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
