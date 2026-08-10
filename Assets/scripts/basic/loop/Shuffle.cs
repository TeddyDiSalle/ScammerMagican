using System.Collections;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public int shufflesAmt;
    private int trackShufflesAmt;
    public float duration;

    // fightManager needs the currently-running movement coroutine for each cup
    // so it can stop a selected cup during the boss fight.
    public Coroutine[] moveCoroutines = new Coroutine[0];

    void Start()
    {
        resetShuffleTracker();
    }

    public void resetShuffleTracker()
    {
        trackShufflesAmt = shufflesAmt;
    }

    public IEnumerator WaitForShuffles(Transform[] cups)
    {
        if (cups == null || cups.Length == 0)
            yield break;

        Debug.Log(trackShufflesAmt);

        yield return new WaitForSeconds(duration * 1.2f);

        doShuffle(cups);

        trackShufflesAmt--;

        if (trackShufflesAmt > 0)
            StartCoroutine(WaitForShuffles(cups));
    }

    // Compatibility overload used by fightManager after the merge.
    public IEnumerator WaitForShuffles(LowerCup lowerCup)
    {
        if (lowerCup == null)
            yield break;

        yield return StartCoroutine(WaitForShuffles(lowerCup.cups));
    }

    public void doShuffle(Transform[] cups)
    {
        if (cups == null || cups.Length == 0)
            return;

        Vector3[] positions = new Vector3[cups.Length];

        for (int i = 0; i < cups.Length; i++)
            positions[i] = cups[i].position;

        for (int i = positions.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        moveCoroutines = new Coroutine[cups.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            moveCoroutines[i] = StartCoroutine(
                GoToPos.MoveCoroutine(cups[i], positions[i], duration)
            );
        }
    }
}
