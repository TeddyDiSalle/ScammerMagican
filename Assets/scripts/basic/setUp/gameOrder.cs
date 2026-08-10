using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameOrder : MonoBehaviour
{
    public setCups cupSetter;
    public LowerCup cupLowerer;
    public Shuffle shuffler;
    public bool showCutscene;
    public GameObject introScene;
    public float cutsceneLength;
    public lvlProgress progresserForStarting;
    public TextMeshProUGUI promptPressText;
    public dialogue talker;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(trueStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callGame()
    {
        cupSetter.MakeCups();//puts cups + ball in starting pos
        shuffler.resetShuffleTracker();//reset amount of shuffles

        

        StartCoroutine(cupLowerer.StartRound());//move cups down, begin shuffle
    }
    IEnumerator trueStart()
    {
        if (showCutscene)
        {
            introScene.SetActive(true);
            yield return new WaitForSeconds(cutsceneLength);
            introScene.SetActive(false);
        }

        progresserForStarting.done = true;
        talker.addLine("Let's begin shall we");
        promptPressText.text = "Press any button to play";
    }

}
