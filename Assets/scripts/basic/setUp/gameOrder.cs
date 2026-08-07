using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOrder : MonoBehaviour
{
    public setCups cupSetter;
    public LowerCup cupLowerer;
    // Start is called before the first frame update
    void Start()
    {
        callGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callGame()
    {
        cupSetter.MakeCups();//puts cups + ball in starting pos

        StartCoroutine(cupLowerer.StartRound());//move cups down, begin shuffle
    }

}
