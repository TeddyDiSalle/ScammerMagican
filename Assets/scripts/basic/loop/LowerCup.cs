using System.Collections;
using UnityEngine;

public class LowerCup : MonoBehaviour
{
    public Transform[] cups;
    public float lowerAmt;
    public float duration;
    public Shuffle shuffler;
    public ParentBall reparenter;

    void Start()
    {
        //StartCoroutine(StartRound());
    }

    void Update()
    {
    }

    public IEnumerator StartRound()
    {
        // Reset the shuffle count for EVERY round.
        if (shuffler != null)
            shuffler.resetShuffleTracker();

        if (AudioManager.Instance != null)
        {
            // Start/keep the main gameplay music going.
            AudioManager.Instance.PlayGameplayMusic();

            // During the opening intro we want MUSIC, but not the gameplay
            // ball-drop SFX playing invisibly behind the cutscene.
            if (!IntroOnceBootstrap.IsPlaying)
                AudioManager.Instance.PlayBallDrop();
        }

        // Keep the ball visible while the cups come down over it.
        yield return StartCoroutine(DoLower());

        // Once covered, attach the ball to the correct cup.
        if (reparenter != null)
            reparenter.SetParent(cups);

        // Hide the real ball only during the actual shuffle.
        if (reparenter != null && reparenter.ball != null)
        {
            SpriteRenderer ballRenderer =
                reparenter.ball.GetComponent<SpriteRenderer>();

            if (ballRenderer != null)
                ballRenderer.enabled = false;
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.StartShuffleSfx();

        if (shuffler != null)
        {
            StartCoroutine(shuffler.WaitForShuffles(this));
        }
        else
        {
            Debug.LogError(
                "LowerCup has no Shuffle reference assigned."
            );
        }
    }

    public void lowerCup(Transform cup, bool raiseInstead = false)
    {
        if (cup == null)
            return;

        GoToPos.MovePos(
            this,
            cup,
            cup.position + new Vector3(
                0f,
                lowerAmt * (raiseInstead ? 1f : -1f),
                0f
            ),
            duration
        );
    }

    public IEnumerator DoLower(
        bool raiseInstead = false,
        Transform ignore = null)
    {
        if (cups == null)
            yield break;

        foreach (Transform cup in cups)
        {
            if (cup == null)
                continue;

            while (cup.childCount > 0)
                cup.GetChild(0).SetParent(null, true);

            if (cup != ignore)
                lowerCup(cup, raiseInstead);
        }

        yield return new WaitForSeconds(duration);
    }
}
