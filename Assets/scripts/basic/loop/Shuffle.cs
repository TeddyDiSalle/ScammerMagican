using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public int shufflesAmt;
    private int trackShufflesAmt;
    public float duration;
    public specMoveManager specialMoves;
    public GameObject clickBlocker;
    public fightManager bossFight;
    public List<Coroutine> moveCoroutines;
    // Start is called before the first frame update
    void Start()
    {
        resetShuffleTracker();
    }

    public void resetShuffleTracker()
    {
        trackShufflesAmt = shufflesAmt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator WaitForShuffles(LowerCup lowerCup)
    {
        StartCoroutine(bossFight.preShuffle());
        //Debug.Log(trackShufflesAmt);
        doShuffle(lowerCup.cups);
        yield return new WaitForSeconds(duration*1.2f);//a little longer than shuffle time

        float specialDuration = specialMoves.shuffleOver();//eventually make this return a float for how long to wait vvvv
        yield return new WaitForSeconds(specialDuration);

        trackShufflesAmt --;//used a shuffle amount
        if (trackShufflesAmt>0)//still more shuffles to do
            StartCoroutine(WaitForShuffles(lowerCup));//run it bacl
        else
            clickBlocker.SetActive(false);//allow player to click
    }

    public void doShuffle(Transform[] cups)
    {
        if (cups.Length<=1)
        {
            //Debug.LogWarning("only 1 cup left, would break game with while loop");
            return;
        }
        moveCoroutines = new List<Coroutine>(0);
        Vector3[] positions = new Vector3[cups.Length];

        for (int i = 0; i < cups.Length; i++)
            positions[i] = cups[i].position;

        bool sameOrder;

        do
        {
            for (int i = positions.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }

            sameOrder = true;

            for (int i = 0; i < positions.Length; i++)
            {
                if (positions[i] != cups[i].position)
                {
                    sameOrder = false;
                    break;
                }
            }

        } while (sameOrder);

        for (int i = 0; i < positions.Length; i++)
        {
            moveCoroutines.Add(StartCoroutine(GoToPos.MoveCoroutine(cups[i], positions[i], duration)));
        }
    }
}
