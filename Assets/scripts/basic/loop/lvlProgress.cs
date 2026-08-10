using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lvlProgress : MonoBehaviour
{
    public TextMeshProUGUI lostText;
    public gameOrder resetter;
    public setCups cupSetter;
    public int [] cupsAtLevels;//how many cups at each level
    public int level = 0;//levels of complexity
    public int winsToProgress;
    public string lossText;
    public string wonText;
    public fightManager bossFight;
    public lose loseManager;
    public won wonManager;
    public GameObject clickBlocker;
    private int currentWinsInLevel;//track wins until they reach winsToProgress
    public bool done;
    // Start is called before the first frame update
    void Start()
    {
        clickBlocker = GameObject.Find("clickBlocker");
    }

    // Update is called once per frame
    void Update()
    {
        if (done&&Input.anyKeyDown)
            reset();
    }

    public void reset()
{
    if (!done)
        return;

    done = false;
    lostText.text = "";

    StartCoroutine(ResetRoutine());
}

    private IEnumerator ResetRoutine()
    {
        // VERY IMPORTANT:
        // The real ball must survive when the old cups are destroyed.
        if (cupSetter != null && cupSetter.ball != null)
        {
            cupSetter.ball.SetParent(null, true);
        }

        // Destroy all old cups.
        foreach (GameObject cup in GameObject.FindGameObjectsWithTag("cup"))
        {
            Destroy(cup);
        }

        // Clear old cup references.
        if (cupSetter != null &&
            cupSetter.cupLower != null)
        {
            cupSetter.cupLower.cups = new Transform[0];
        }

        // Give Unity one frame to actually destroy the old cups.
        yield return null;

        resetter.callGame();

        if (bossFight != null)
            bossFight.checkReady(level);
    }

    public void lost()
    {
        clickBlocker.SetActive(true);
        loseManager.lost();
        lostText.text = lossText;
        roundOver();
    }

    public void won()
    {
        loseManager.won();
        currentWinsInLevel++;

        wonManager.doWin(currentWinsInLevel>=winsToProgress);
        if (currentWinsInLevel>=winsToProgress)
            progressLevel();

        roundOver();
        
        lostText.text = wonText;
    }

    public void roundOver()
    {
        done = true;

        foreach (GameObject fakeBall in GameObject.FindGameObjectsWithTag("fakeBall"))
            StartCoroutine(fadeUp.MoveUpAndFade(fakeBall,1f,.5f));
    }

    public void initialDone()//called before the cups rise
    {
        foreach (GameObject fakeBall in GameObject.FindGameObjectsWithTag("fakeBall"))
            fakeBall.transform.parent = null;
    }

    public void progressLevel(bool gainLevel = true)
{
    currentWinsInLevel = 0;

    if (gainLevel)
        level++;

    // Protect against invalid negative levels.
    if (level < 0)
    {
        Debug.LogWarning("Level went below 0.");
        return;
    }

    // Protect against going past the final level.
    if (level >= cupsAtLevels.Length)
    {
        Debug.LogWarning("Reached end of levels.");
        return;
    }

    cupSetter.cupsAmt = cupsAtLevels[level];
}
}
