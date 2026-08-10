using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class won : MonoBehaviour
{
    public string [] slightWinLines;
    public string [] fullWinLines;
    public dialogue chatManager;

    private animSprite magicianAnimator;
    private Sprite[] worriedSprites;

    public void doWin(bool fullWin)
    {
        SetMagicianWorried();

        if (fullWin)
            chatManager.addLine(fullWinLines[Random.Range(0,fullWinLines.Length)]);
        else
            chatManager.addLine(slightWinLines[Random.Range(0,slightWinLines.Length)]);
    }

    private void SetMagicianWorried()
    {
        if (magicianAnimator == null)
        {
            GameObject magician = GameObject.Find("magician");
            if (magician != null)
                magicianAnimator = magician.GetComponent<animSprite>();
        }

        if (worriedSprites == null || worriedSprites.Length == 0)
            worriedSprites = animSprite.LoadSpritesFromResources("Art/MagicianWorried");

        if (magicianAnimator != null && worriedSprites != null && worriedSprites.Length > 0)
            magicianAnimator.SetSprites(worriedSprites, true, false, 12f);
    }
}
