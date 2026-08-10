using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerCup : MonoBehaviour
{
    public Transform [] cups;
    public float lowerAmt;
    public float duration;
    public Shuffle shuffler;
    public ParentBall reparenter;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(StartRound());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartRound()
    {
        yield return StartCoroutine(DoLower());

        reparenter.SetParent(cups);
        StartCoroutine(shuffler.WaitForShuffles(this));
    }

    public void lowerCup(Transform cup,bool raiseInstead = false)
    {
        GoToPos.MovePos(this,cup,cup.position+new Vector3(0f,lowerAmt*(raiseInstead?1f:-1f),0f),duration);
    }
    public IEnumerator DoLower(
    bool raiseInstead = false,
    Transform ignore = null)
    {
        foreach (Transform cup in cups)
        {
            if (cup == null)
                continue;

            // Remove ALL children, not just child 0.
            while (cup.childCount > 0)
            {
                cup.GetChild(0).SetParent(null, true);
            }

            if (cup != ignore)
            {
                lowerCup(cup, raiseInstead);
            }
        }

        yield return new WaitForSeconds(duration);
    }

    
}
