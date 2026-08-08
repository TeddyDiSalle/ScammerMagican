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
    private int currentWinsInLevel;//track wins until they reach winsToProgress
    private bool done;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (done&&Input.anyKeyDown)
            reset();
    }

    public void reset()
    {
        done = false;
        lostText.text = "";
        foreach (GameObject cup in GameObject.FindGameObjectsWithTag("cup"))
            Destroy(cup);
        resetter.callGame();
    }

    public void lost()
    {
        
        lostText.text = lossText;
        roundOver();
    }

    public void won()
    {

        currentWinsInLevel++;
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

    void progressLevel()
    {
        currentWinsInLevel = 0;
        level ++;
        if (level >= cupsAtLevels.Length)
        {
            Debug.LogWarning("Reached end of levels");
            return;
        }
        cupSetter.cupsAmt = cupsAtLevels[level];
    }
}
