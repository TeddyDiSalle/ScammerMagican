using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerCup : MonoBehaviour
{
    public Transform[] cups;
    public float lowerAmt;
    public float duration;
    public Shuffle shuffler;
    public ParentBall reparenter;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartRound());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartRound()
    {
        // Normal gameplay music for the cover/shuffle portion.
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameplayMusic();
            AudioManager.Instance.PlayBallDrop();
        }

        // Keep the ball VISIBLE while the cups come down over it.
        // This preserves the reveal/cover animation at the start of the round.
        yield return StartCoroutine(DoLower());

        // Once the cups are fully down, attach the ball to the correct cup
        // so it follows that cup during the shuffle.
        if (reparenter != null)
            reparenter.SetParent(cups);

        // NOW hide the ball for the actual shuffle movement.
        if (reparenter != null && reparenter.ball != null)
        {
            SpriteRenderer ballRenderer =
                reparenter.ball.GetComponent<SpriteRenderer>();

            if (ballRenderer != null)
                ballRenderer.enabled = false;
        }

        // Start the looping shuffle SFX only during the actual shuffle.
        if (AudioManager.Instance != null)
            AudioManager.Instance.StartShuffleSfx();

        // Begin shuffling only after the ball is covered and hidden.
        if (shuffler != null)
            StartCoroutine(shuffler.WaitForShuffles(this));
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

            // Remove ALL children, not just child 0.
            // This keeps the real ball/fake balls from being destroyed with a cup
            // and lets the reveal show what was underneath each cup.
            while (cup.childCount > 0)
            {
                cup.GetChild(0).SetParent(null, true);
            }

            if (cup != ignore)
            {
                lowerCup(cup, raiseInstead);
            }
        }

        yield return new WaitForSeconds(duration);
    }
}
