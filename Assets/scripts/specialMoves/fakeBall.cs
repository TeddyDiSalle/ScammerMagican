using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class fakeBall : MonoBehaviour
{
    public GameObject ball;
    public LowerCup cupArray;
    public Color color;
    public rollToOther roller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float makeBall()
    {
        List<Transform> emptyCups = new List<Transform>();
        foreach (Transform cup in cupArray.cups)
        {
            if (cup.childCount==0)
                emptyCups.Add(cup);
        }

        if (emptyCups.Count==0)
            return 0f;

        Transform hostCup = emptyCups[Random.Range(0, emptyCups.Count)];

        Transform fakeBall = Instantiate(ball).transform;
        fakeBall.SetParent(hostCup);
        fakeBall.localScale = ball.transform.localScale;
        fakeBall.localPosition = ball.transform.localPosition;
        fakeBall.GetComponent<SpriteRenderer>().color = color;
        fakeBall.tag = "fakeBall";

        roller.doRoll(cupArray.cups, true, fakeBall);
        return roller.duration;
    }
}
