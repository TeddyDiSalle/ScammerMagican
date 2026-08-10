using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class rollToOther : MonoBehaviour
{
    public Transform ball;
    public float duration;
    public teleport teleporter;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float doRoll(Transform [] cups,bool onlyNext = true,Transform ball = null)
    {
        if (ball==null)//no fake ball set
            ball = this.ball;//use real ball

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
            if (onlyNext){//go to an cup next to ball
                if ((i > 0 && cups[i - 1] == cup) && (i < cups.Length - 1 && cups[i + 1] == cup))//ball has cups on left and right
                {
                    if (Random.Range(0,2)==0)//50/50
                        StartCoroutine(animRoll(ball,cups[i],cups[i-1]));//ball goes right
                    else
                        StartCoroutine(animRoll(ball,cups[i],cups[i+1]));//ball goes left
                    return duration;
                }
                else if (i>0&&cups[i-1]==cup){//cup to the left has ball
                    StartCoroutine(animRoll(ball,cups[i],cups[i-1]));//anim to this cup (cup to the right of ball)
                    return duration;
                }else if (i<cups.Length-1&&cups[i+1]==cup){//cup to the right has ball
                    StartCoroutine(animRoll(ball,cups[i],cups[i+1]));//anim to this cup (cup to the left of ball)
                    return duration;
                }
            }else//go to any cup
            {
                int randomCup = -1;
                if (cups[i]==cup)
                {
                    while (randomCup==-1||randomCup==i)
                        randomCup = Random.Range(0,cups.Length);
                }
                StartCoroutine(animRoll(ball,cups[i],cups[randomCup]));
                return duration;
            }
        }

        return 0f;
    }

    IEnumerator animRoll(Transform ball,Transform nextCup,Transform otherCup)
    {
        ball.parent = null;

        if (duration!=0){
            StartCoroutine(GoToPos.MoveCoroutine(nextCup,new Vector2(nextCup.position.x,nextCup.position.y+2f),duration*.25f));
            yield return StartCoroutine(GoToPos.MoveCoroutine(otherCup,new Vector2(otherCup.position.x,otherCup.position.y+2f),duration*.25f));
        }
        
        yield return StartCoroutine(GoToPos.MoveCoroutine(ball,new Vector2(nextCup.position.x,ball.position.y),duration*.5f));

        if (duration!=0){
            StartCoroutine(GoToPos.MoveCoroutine(nextCup,new Vector2(nextCup.position.x,nextCup.position.y-2f),duration*.25f));
            yield return StartCoroutine(GoToPos.MoveCoroutine(otherCup,new Vector2(otherCup.position.x,otherCup.position.y-2f),duration*.25f));
        }
        ball.parent = nextCup;

        handleTeleport(nextCup,otherCup);//one instance of this script is for teleporting not rolling
    }

    void handleTeleport(Transform nextCup,Transform otherCup)
    {
        if (teleporter==null)//if no teleporter set assume this instance is only for rolling
            return;//ignore this funct
        teleporter.makePortal(nextCup);
        teleporter.makePortal(otherCup);
    }


}
