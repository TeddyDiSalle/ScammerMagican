using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public int shufflesAmt;
    private int trackShufflesAmt;
    public float duration;
    public specMoveManager specialMoves;
    public GameObject clickBlocker;
    public fightManager bossFight;
    public List<Coroutine> moveCoroutines = new List<Coroutine>();

    // Every new shuffle run gets an ID. If a round resets/destroys cups,
    // CancelActiveShuffles invalidates the old coroutine so it cannot continue
    // trying to shuffle destroyed Transform references.
    private int shuffleRunId = 0;

    void Start()
    {
        resetShuffleTracker();
    }

    public void resetShuffleTracker()
    {
        trackShufflesAmt = shufflesAmt;
    }

    public void CancelActiveShuffles()
    {
        shuffleRunId++;

        foreach (Coroutine movement in moveCoroutines)
        {
            if (movement != null)
                StopCoroutine(movement);
        }

        moveCoroutines.Clear();

        if (AudioManager.Instance != null)
            AudioManager.Instance.StopShuffleSfx();
    }

    public IEnumerator WaitForShuffles(LowerCup lowerCup)
    {
        int myRunId = ++shuffleRunId;

        while (trackShufflesAmt > 0)
        {
            if (myRunId != shuffleRunId)
                yield break;

            if (lowerCup == null || lowerCup.cups == null)
            {
                StopShuffleAudioOnly();
                yield break;
            }

            // Remove any cups that were already destroyed before this shuffle.
            lowerCup.cups = GetValidCups(lowerCup.cups);

            if (lowerCup.cups.Length <= 1)
            {
                StopShuffleAudioOnly();
                yield break;
            }

            if (bossFight != null)
                StartCoroutine(bossFight.preShuffle());

            doShuffle(lowerCup.cups);

            yield return new WaitForSeconds(duration * 1.2f);

            if (myRunId != shuffleRunId)
                yield break;

            // A cup can disappear during the boss fight or a reset.
            lowerCup.cups = GetValidCups(lowerCup.cups);

            if (lowerCup.cups.Length <= 1)
            {
                StopShuffleAudioOnly();
                yield break;
            }

            if (specialMoves != null)
            {
                float specialDuration = specialMoves.shuffleOver();

                if (specialDuration > 0f)
                    yield return new WaitForSeconds(specialDuration);
            }

            if (myRunId != shuffleRunId)
                yield break;

            trackShufflesAmt--;
        }

        if (myRunId == shuffleRunId)
        {
            EndShuffleAudio();

            if (clickBlocker != null)
                clickBlocker.SetActive(false);
        }
    }

    // Compatibility overload for scripts that still pass a Transform array.
    public IEnumerator WaitForShuffles(Transform[] cups)
    {
        int myRunId = ++shuffleRunId;

        Transform[] workingCups = GetValidCups(cups);

        while (trackShufflesAmt > 0)
        {
            if (myRunId != shuffleRunId)
                yield break;

            workingCups = GetValidCups(workingCups);

            if (workingCups.Length <= 1)
            {
                StopShuffleAudioOnly();
                yield break;
            }

            doShuffle(workingCups);

            yield return new WaitForSeconds(duration * 1.2f);

            if (myRunId != shuffleRunId)
                yield break;

            workingCups = GetValidCups(workingCups);

            if (workingCups.Length <= 1)
            {
                StopShuffleAudioOnly();
                yield break;
            }

            if (specialMoves != null)
            {
                float specialDuration = specialMoves.shuffleOver();

                if (specialDuration > 0f)
                    yield return new WaitForSeconds(specialDuration);
            }

            if (myRunId != shuffleRunId)
                yield break;

            trackShufflesAmt--;
        }

        if (myRunId == shuffleRunId)
        {
            EndShuffleAudio();

            if (clickBlocker != null)
                clickBlocker.SetActive(false);
        }
    }

    private Transform[] GetValidCups(Transform[] cups)
    {
        if (cups == null)
            return new Transform[0];

        List<Transform> valid = new List<Transform>();

        foreach (Transform cup in cups)
        {
            if (cup != null)
                valid.Add(cup);
        }

        return valid.ToArray();
    }

    private void StopShuffleAudioOnly()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopShuffleSfx();
    }

    private void EndShuffleAudio()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopShuffleSfx();
        AudioManager.Instance.PlaySelectionMusic();
    }

    public void doShuffle(Transform[] cups)
    {
        Transform[] validCups = GetValidCups(cups);

        if (validCups.Length <= 1)
            return;

        // Stop only the movement coroutines from the previous shuffle step.
        foreach (Coroutine movement in moveCoroutines)
        {
            if (movement != null)
                StopCoroutine(movement);
        }

        moveCoroutines.Clear();

        Vector3[] positions = new Vector3[validCups.Length];

        for (int i = 0; i < validCups.Length; i++)
        {
            if (validCups[i] == null)
                return;

            positions[i] = validCups[i].position;
        }

        bool sameOrder;

        do
        {
            for (int i = positions.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (positions[i], positions[j]) =
                    (positions[j], positions[i]);
            }

            sameOrder = true;

            for (int i = 0; i < positions.Length; i++)
            {
                if (validCups[i] == null)
                    return;

                if (positions[i] != validCups[i].position)
                {
                    sameOrder = false;
                    break;
                }
            }
        }
        while (sameOrder);

        for (int i = 0; i < positions.Length; i++)
        {
            if (validCups[i] == null)
                continue;

            Coroutine movement = StartCoroutine(
                GoToPos.MoveCoroutine(
                    validCups[i],
                    positions[i],
                    duration
                )
            );

            moveCoroutines.Add(movement);
        }
    }
}
