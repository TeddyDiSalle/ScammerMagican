using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class lose : MonoBehaviour
{
    public int lossesInARow;
    public int lossesToDemotion;
    public bool demoted;
    public int lossesToLose;
    public lvlProgress progressManager;
    public dialogue chatManager;
    public TextMeshProUGUI text;
    public string [] slightLossLines;
    public string [] fullLossLines;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void won()
    {
        lossesInARow = 0;
    }

    public void lost()
    {
        lossesInARow++;

        if (lossesInARow >= lossesToDemotion && !demoted)
        {
            demoted = true;

            // If we're already at the bottom level,
            // this loss means game over instead of level -1.
            if (progressManager.level <= 0 ||
                lossesInARow >= lossesToLose)
            {
                demoted = false;
                gameOver();
                return;
            }

            progressManager.level--;
            progressManager.progressLevel(false);
        }

        if (slightLossLines != null &&
            slightLossLines.Length > 0)
        {
            chatManager.addLine(
                slightLossLines[
                    Random.Range(0, slightLossLines.Length)
                ]
            );
        }
    }

    public void gameOver()
    {
        progressManager.level = 0;
        progressManager.progressLevel(false);
        StartCoroutine(waitForDone());
        text.text = "Press any button to restart";
        chatManager.addLine(fullLossLines[Random.Range(0,fullLossLines.Length)]);
        //fighter.fightReady = false;
        //Debug.Log("gameOver");
    }

    IEnumerator waitForDone()
    {
        yield return new WaitForSeconds(1f);
        progressManager.done = true;
    }
}
