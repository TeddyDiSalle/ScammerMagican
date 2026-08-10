using System.Collections;
using UnityEngine;

public class smokeScreen : MonoBehaviour
{
    public SpriteRenderer smoke;    // puff sprite over the cups, starts alpha 0, high sorting order
    public rollToOther roller;      // reuse the real cheat, hidden under cover
    public LowerCup cupArray;
    public GameObject clickBlocker;  // reuse lvlProgress's blocker so no clicking mid-smoke
    public float fadeDur;           // billow-in / clear-out time
    public float holdDur;           // fully opaque window - the sleight happens here
    private bool alreadyRunning;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public float callSmoke()
    {
        if (!alreadyRunning)
            StartCoroutine(smokeRun());

        return fadeDur * 2f + holdDur;//choose/reveal must wait this whole window
    }

    IEnumerator smokeRun()
    {
        alreadyRunning = true;
        if (clickBlocker!=null)
            clickBlocker.SetActive(true);

        yield return StartCoroutine(fade(0f, 1f, fadeDur));//billow in

        roller.doRoll(cupArray.cups);//cheat while hidden
        yield return new WaitForSeconds(holdDur);

        yield return StartCoroutine(fade(1f, 0f, fadeDur));//clear out

        if (clickBlocker!=null)
            clickBlocker.SetActive(false);
        alreadyRunning = false;
    }

    IEnumerator fade(float from, float to, float dur)
    {
        float t = 0f;
        Color c = smoke.color;
        while (t < dur)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / dur);
            smoke.color = c;
            yield return null;
        }
        c.a = to;
        smoke.color = c;
    }
}
