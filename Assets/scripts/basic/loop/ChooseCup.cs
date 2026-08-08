using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseCup : MonoBehaviour
{
    public float revealDelay;
    public GameObject clickBlocker;
    private lvlProgress progress;
    private LowerCup cupRaiser;
    // Start is called before the first frame update
    void Start()
    {
        cupRaiser = FindObjectOfType<LowerCup>();
        progress = FindObjectOfType<lvlProgress>();
        clickBlocker = GameObject.Find("clickBlocker");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (!clickBlocker.activeSelf)
            StartCoroutine(DoReveal());
    }

    IEnumerator DoReveal()
    {
        clickBlocker.SetActive(true);//make sure player cant click
        bool won = false;
        //Debug.Log("Sprite clicked!"+gameObject);

        if (transform.childCount>0&&//has a child
        transform.GetChild(0).gameObject.name=="ball"){//its the ball!
            transform.GetChild(0).SetParent(null);
            won = true;
            
        }
        cupRaiser.lowerCup(transform,true);

        yield return new WaitForSeconds(cupRaiser.duration+revealDelay);
        progress.initialDone();

        yield return StartCoroutine(cupRaiser.DoLower(true,transform));

        if (won)
            progress.won();
        else
            progress.lost();
    }
}
