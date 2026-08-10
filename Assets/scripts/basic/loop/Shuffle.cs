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

    void Start()
    {
        resetShuffleTracker();
    }

    public void resetShuffleTracker()
    {
        trackShufflesAmt = shufflesAmt;
    }

    public IEnumerator WaitForShuffles(LowerCup lowerCup)
    {
        if (lowerCup == null || lowerCup.cups == null || lowerCup.cups.Length == 0)
            yield break;

        if (bossFight != null)
            StartCoroutine(bossFight.preShuffle());

        doShuffle(lowerCup.cups);
        yield return new WaitForSeconds(duration * 1.2f);

        if (specialMoves != null)
        {
            float specialDuration = specialMoves.shuffleOver();
            if (specialDuration > 0f)
                yield return new WaitForSeconds(specialDuration);
        }

        trackShufflesAmt--;

        if (trackShufflesAmt > 0)
        {
            StartCoroutine(WaitForShuffles(lowerCup));
        }
        else if (clickBlocker != null)
        {
            clickBlocker.SetActive(false);
        }
    }

    // Compatibility overload for scripts that still pass a Transform array.
    public IEnumerator WaitForShuffles(Transform[] cups)
    {
        if (cups == null || cups.Length == 0)
            yield break;

        doShuffle(cups);
        yield return new WaitForSeconds(duration * 1.2f);

        if (specialMoves != null)
        {
            float specialDuration = specialMoves.shuffleOver();
            if (specialDuration > 0f)
                yield return new WaitForSeconds(specialDuration);
        }

        trackShufflesAmt--;

        if (trackShufflesAmt > 0)
        {
            StartCoroutine(WaitForShuffles(cups));
        }
        else if (clickBlocker != null)
        {
            clickBlocker.SetActive(false);
        }
    }

    public void doShuffle(Transform[] cups)
    {
        if (cups == null || cups.Length <= 1)
        {
            Debug.LogWarning("Not enough cups to shuffle.");
            return;
        }

        moveCoroutines.Clear();

        Vector3[] positions = new Vector3[cups.Length];

        for (int i = 0; i < cups.Length; i++)
            positions[i] = cups[i].position;

        bool sameOrder;

        do
        {
            for (int i = positions.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }

            sameOrder = true;

            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i] != cups[i].position)
                {
                    sameOrder = false;
                    break;
                }
            }
        }
        while (sameOrder);

        for (int i = 0; i < positions.Length; i++)
        {
            Coroutine movement = StartCoroutine(
                GoToPos.MoveCoroutine(cups[i], positions[i], duration)
            );

            moveCoroutines.Add(movement);
        }
    }
}
