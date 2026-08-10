using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class fightManager : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject clickBlocker;
    public LowerCup lowerCup;
    public Shuffle shuffler;
    public float hitTime;
    public int level;
    private Transform resetCup;
    public setCups gameSetter;
    public Transform ball;
    public rollToOther roller;
    public float health = 100f;
    public float damage;
    public healthBar bar;
    public lose loseManager;

    private bool fightReady = false;
    private float hitTimeTracker = -1f;
    private bool doingHit;

    // Trigger the normal cat movement once when the boss starts.
    private bool bossCatTriggered = false;

    void Start()
    {
    }

    void Update()
    {
        bar.transform.parent.gameObject.SetActive(fightReady);

        if (!fightReady)
            return;

        if (!clickBlocker.activeSelf && !doingHit)
        {
            shuffler.resetShuffleTracker();
            StartCoroutine(shuffler.WaitForShuffles(lowerCup));
            doingHit = true;
            hitTimeTracker = hitTime;
        }

        text.enabled = hitTimeTracker > 0;

        if (hitTimeTracker > 0)
        {
            hitTimeTracker -= Time.deltaTime;
            text.text = $"Choose the Cup NOW! {hitTimeTracker.ToString("F2")}s";
        }
        else if (doingHit)
        {
            clickBlocker.SetActive(true);
            doingHit = false;
        }
    }

    public void checkReady(int currentLevel)
    {
        bool wasFightReady = fightReady;
        fightReady = currentLevel >= this.level;

        // Use the original cat object and its original movement.
        if (fightReady && !wasFightReady && !bossCatTriggered)
        {
            bossCatTriggered = true;
            StartCoroutine(TriggerBossCat());
        }

        if (!fightReady)
            bossCatTriggered = false;
    }

    private IEnumerator TriggerBossCat()
    {
        yield return new WaitForSeconds(0.75f);

        specMoveManager specials = FindObjectOfType<specMoveManager>();

        if (specials != null && specials.cat != null)
        {
            Debug.Log("Boss fight: original cat movement triggered.");
            specials.cat.callCat();
        }
        else
        {
            Debug.LogWarning(
                "Boss fight could not trigger the cat: " +
                "specMoveManager or catDistraction reference is missing."
            );
        }
    }

    public bool haltCup(Transform cup)
    {
        if (!fightReady)
            return false;

        for (int i = 0; i < lowerCup.cups.Length; i++)
        {
            if (lowerCup.cups[i] == cup)
            {
                StopCoroutine(shuffler.moveCoroutines[i]);
                lowerCup.cups = lowerCup.cups.Where(x => x != cup).ToArray();

                bool gotRightCup = false;

                foreach (Transform child in cup)
                {
                    if (child == ball)
                    {
                        gotRightCup = true;
                        StartCoroutine(fadeUp.MoveUpAndFade(cup.gameObject, 2f, .5f));
                        child.parent = null;
                        goNearestCup(ball.position, lowerCup.cups.Length == 1);

                        if (lowerCup.cups.Length == 1)
                        {
                            finale final = lowerCup.cups[0].gameObject.AddComponent<finale>();
                            final.ball = ball;
                            fightReady = false;
                            text.text = "";
                        }
                    }
                    else
                    {
                        Destroy(child.gameObject);
                    }
                }

                if (!gotRightCup)
                {
                    StartCoroutine(raiseThenLower(cup));
                    health -= damage;
                    bar.setHealth(health / 100f);

                    if (health <= 0)
                    {
                        text.text = "";
                        ball.parent = null;

                        foreach (GameObject cup1 in GameObject.FindGameObjectsWithTag("cup"))
                            Destroy(cup1);

                        fightReady = false;
                        clickBlocker.SetActive(true);
                        loseManager.gameOver();
                    }
                }

                hitTimeTracker = 0f;
                return true;
            }
        }

        return false;
    }

    void goNearestCup(Vector2 position, bool final = false)
    {
        float closestDist = 999f;
        Transform closestCup = null;

        foreach (Transform cup in lowerCup.cups)
        {
            float dist = Mathf.Abs(cup.position.x - position.x);

            if (dist <= closestDist)
            {
                closestDist = dist;
                closestCup = cup;
            }
        }

        if (closestCup)
            StartCoroutine(moveLocalPos0(closestCup, final));
    }

    IEnumerator moveLocalPos0(Transform closestCup, bool final = false)
    {
        yield return new WaitForSeconds(1f);

        if (!final)
            ball.parent = closestCup;

        float speed = final ? 100f : 5f;
        float acceleration = speed;

        while ((!final && Mathf.Abs(ball.localPosition.x) > 0.1f) ||
               (final && Vector2.Distance(ball.localPosition, Vector2.zero) > 0.1f))
        {
            speed += acceleration * Time.deltaTime;

            if (!final)
            {
                ball.localPosition = new Vector2(
                    Mathf.MoveTowards(ball.localPosition.x, 0f, speed * Time.deltaTime),
                    ball.localPosition.y
                );
            }
            else
            {
                ball.localPosition = Vector2.MoveTowards(
                    ball.localPosition,
                    Vector2.zero,
                    speed * Time.deltaTime
                );
            }

            yield return null;
        }

        ball.localPosition = new Vector2(0f, ball.localPosition.y);
    }

    public IEnumerator preShuffle()
    {
        if (!resetCup)
            yield break;

        List<Vector2> positions = gameSetter.getCupPositions();

        foreach (Vector2 position in positions)
        {
            bool spotFilled = false;

            foreach (Transform thisCup in lowerCup.cups)
            {
                if (Mathf.Abs(thisCup.position.x - position.x) <= .5f)
                {
                    spotFilled = true;
                    break;
                }
            }

            if (spotFilled)
                continue;

            yield return StartCoroutine(
                GoToPos.MoveCoroutine(
                    resetCup,
                    new Vector2(position.x, resetCup.position.y - lowerCup.lowerAmt),
                    lowerCup.duration
                )
            );

            lowerCup.cups = lowerCup.cups.Append(resetCup).ToArray();
            resetCup = null;
            yield break;
        }

        Debug.LogWarning("if this is called its probably a problem");
    }

    IEnumerator raiseThenLower(Transform cup)
    {
        yield return StartCoroutine(
            GoToPos.MoveCoroutine(
                cup,
                cup.position + new Vector3(0f, lowerCup.lowerAmt * 1f, 0f),
                lowerCup.duration
            )
        );

        resetCup = cup;
    }
}
