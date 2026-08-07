using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    public int shufflesAmt;
    private int trackShufflesAmt;
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        trackShufflesAmt = shufflesAmt;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator WaitForShuffles(Transform [] cups)
    {
        Debug.Log(trackShufflesAmt);
        yield return new WaitForSeconds(duration*1.2f);//a little longer than shuffle time
        doShuffle(cups);

        trackShufflesAmt --;//used a shuffle amount
        if (trackShufflesAmt>0)//still more shuffles to do
            StartCoroutine(WaitForShuffles(cups));//run it bacl
    }

    public void doShuffle(Transform [] cups)
    {
        Vector3[] positions = new Vector3[cups.Length];

        for (int i = 0; i < cups.Length; i++)
            positions[i] = cups[i].position;// Copy positions

        
        for (int i = positions.Length - 1; i > 0; i--)// Shuffle randomly
        {
            int j = Random.Range(0, i + 1);
            (positions[i], positions[j]) = (positions[j], positions[i]);
        }

        for (int i = 0; i < positions.Length; i++)
        {
            GoToPos.MovePos(this,cups[i],positions[i],duration);
        }
    }
}
