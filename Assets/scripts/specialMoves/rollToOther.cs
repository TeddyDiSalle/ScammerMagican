using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class rollToOther : MonoBehaviour
{
    public Transform ball;
    public float duration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float doRoll(Transform [] cups)
    {
        if (ball.parent==null){
            Debug.LogWarning("ball not in cup");
            return 0f;
        }

        Transform cup = ball.parent;
        cups = cups
            .OrderBy(t => t.position.x)
            .ToArray();

        for (int i = 0; i < cups.Length; i++)
        {
            if ((i > 0 && cups[i - 1] == cup) && (i < cups.Length - 1 && cups[i + 1] == cup))//ball has cups on left and right
            {
                if (Random.Range(0,2)==0)//50/50
                    StartCoroutine(animRoll(cups[i],cups[i-1]));//ball goes right
                else
                    StartCoroutine(animRoll(cups[i],cups[i+1]));//ball goes left
                return duration;
            }
            else if (i>0&&cups[i-1]==cup){//cup to the left has ball
                StartCoroutine(animRoll(cups[i],cups[i-1]));//anim to this cup (cup to the right of ball)
                return duration;
            }else if (i<cups.Length-1&&cups[i+1]==cup){//cup to the right has ball
                StartCoroutine(animRoll(cups[i],cups[i+1]));//anim to this cup (cup to the left of ball)
                return duration;
            }
        }

        return 0f;
    }

    IEnumerator animRoll(Transform nextCup,Transform otherCup)
    {
        //Debug.Log(nextCup);
        ball.parent = null;

        StartCoroutine(GoToPos.MoveCoroutine(nextCup,new Vector2(nextCup.position.x,nextCup.position.y+2f),duration*.25f));
        yield return StartCoroutine(GoToPos.MoveCoroutine(otherCup,new Vector2(otherCup.position.x,otherCup.position.y+2f),duration*.25f));
        
        yield return StartCoroutine(GoToPos.MoveCoroutine(ball,new Vector2(nextCup.position.x,ball.position.y),duration*.5f));

        StartCoroutine(GoToPos.MoveCoroutine(nextCup,new Vector2(nextCup.position.x,nextCup.position.y-2f),duration*.25f));
        yield return StartCoroutine(GoToPos.MoveCoroutine(otherCup,new Vector2(otherCup.position.x,otherCup.position.y-2f),duration*.25f));
        ball.parent = nextCup;
    }


}
