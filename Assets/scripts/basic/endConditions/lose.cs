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

    private animSprite magicianAnimator;
    private Sprite[] happySprites;

    public void won()
    {
        lossesInARow = 0;
    }

    public void lost()
    {
        SetMagicianHappy();
        lossesInARow++;

        if (lossesInARow >= lossesToDemotion && !demoted)
        {
            demoted = true;

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
        SetMagicianHappy();
        progressManager.level = 0;
        progressManager.progressLevel(false);
        StartCoroutine(waitForDone());
        text.text = "Press any button to restart";
        chatManager.addLine(fullLossLines[Random.Range(0,fullLossLines.Length)]);
    }

    IEnumerator waitForDone()
    {
        yield return new WaitForSeconds(1f);
        progressManager.done = true;
    }

    private void SetMagicianHappy()
    {
        if (magicianAnimator == null)
        {
            GameObject magician = GameObject.Find("magician");
            if (magician != null)
                magicianAnimator = magician.GetComponent<animSprite>();
        }

        if (happySprites == null || happySprites.Length == 0)
            happySprites = animSprite.LoadSpritesFromResources("Art/MagicianHappy");

        if (magicianAnimator != null && happySprites != null && happySprites.Length > 0)
            magicianAnimator.SetSprites(happySprites, true, false, 12f);
    }
}
