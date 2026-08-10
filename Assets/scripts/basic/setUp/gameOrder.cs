using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOrder : MonoBehaviour
{
    public setCups cupSetter;
    public LowerCup cupLowerer;

    void Start()
    {
        callGame();
    }

    public void callGame()
    {
        cupSetter.MakeCups();
        StartCoroutine(cupLowerer.StartRound());
    }
}
