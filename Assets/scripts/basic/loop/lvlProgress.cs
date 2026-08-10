using System.Collections;
using UnityEngine;
using TMPro;

public class lvlProgress : MonoBehaviour
{
    public TextMeshProUGUI lostText;
    public gameOrder resetter;
    public setCups cupSetter;
    public int [] cupsAtLevels;
    public int level = 0;
    public int winsToProgress;
    public string lossText;
    public string wonText;
    public fightManager bossFight;
    public lose loseManager;
    public won wonManager;
    public GameObject clickBlocker;

    private int currentWinsInLevel;
    public bool done;

    void Start()
    {
        clickBlocker = GameObject.Find("clickBlocker");
    }

    void Update()
    {
        if (done && Input.anyKeyDown)
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
        // Stop the OLD round's shuffle coroutine before any cup is destroyed.
        // This is what prevents Shuffle from continuing with destroyed Transforms.
        if (cupSetter != null &&
            cupSetter.cupLower != null &&
            cupSetter.cupLower.shuffler != null)
        {
            cupSetter.cupLower.shuffler.CancelActiveShuffles();
        }

        // The real ball must survive when the old cups are destroyed.
        if (cupSetter != null && cupSetter.ball != null)
            cupSetter.ball.SetParent(null, true);

        foreach (GameObject cup in GameObject.FindGameObjectsWithTag("cup"))
            Destroy(cup);

        if (cupSetter != null && cupSetter.cupLower != null)
            cupSetter.cupLower.cups = new Transform[0];

        // Restore the normal magician idle after a win/loss reaction.
        GameObject magicianGO = GameObject.Find("magician");

        if (magicianGO != null)
        {
            animSprite magician =
                magicianGO.GetComponent<animSprite>();

            if (magician != null)
                magician.ResetToDefault();
        }

        // Let Unity finish destroying the previous cups.
        yield return null;

        resetter.callGame();

        if (bossFight != null)
            bossFight.checkReady(level);
    }

    public void lost()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayLoseStinger();

        if (clickBlocker != null)
            clickBlocker.SetActive(true);

        loseManager.lost();
        lostText.text = lossText;
        roundOver();
    }

    public void won()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayWinStinger();

        loseManager.won();
        currentWinsInLevel++;

        bool advancing = currentWinsInLevel >= winsToProgress;

        wonManager.doWin(advancing);

        if (advancing)
            progressLevel();

        roundOver();
        lostText.text = wonText;
    }

    public void roundOver()
    {
        done = true;

        foreach (GameObject fakeBall
                 in GameObject.FindGameObjectsWithTag("fakeBall"))
        {
            StartCoroutine(
                fadeUp.MoveUpAndFade(fakeBall, 1f, .5f)
            );
        }
    }

    public void initialDone()
    {
        foreach (GameObject fakeBall
                 in GameObject.FindGameObjectsWithTag("fakeBall"))
        {
            fakeBall.transform.parent = null;
        }
    }

    public void progressLevel(bool gainLevel = true)
    {
        currentWinsInLevel = 0;

        if (gainLevel)
            level++;

        if (level < 0)
        {
            level = 0;
            return;
        }

        if (cupsAtLevels == null || cupsAtLevels.Length == 0)
            return;

        // Your scene has four normal cup levels (0-3), and bossFight starts
        // at level 4. Therefore level == cupsAtLevels.Length is intentional:
        // it means "normal levels finished; enter boss fight."
        if (level == cupsAtLevels.Length)
        {
            Debug.Log("Normal levels complete. Boss fight unlocked.");
            return;
        }

        // Do not allow progression beyond the boss threshold.
        if (level > cupsAtLevels.Length)
        {
            level = cupsAtLevels.Length;
            return;
        }

        cupSetter.cupsAmt = cupsAtLevels[level];
    }
}
