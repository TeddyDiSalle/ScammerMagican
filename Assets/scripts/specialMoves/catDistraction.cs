using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class catDistraction : MonoBehaviour
{
    public Transform cat;
    public float distance;
    public float duration;
    private bool alreadyRunning;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float callCat()
    {
        if (!alreadyRunning)
            StartCoroutine(catRun());

        return 0f;
    }

    IEnumerator catRun()
    {
        alreadyRunning = true;
        Vector2 goal = cat.transform.position;
        if (cat.transform.position.x<0){
            goal += new Vector2(distance,0f);
            cat.localScale = new Vector2(1f,1f);
        }else{
            goal -= new Vector2(distance,0f);
            cat.localScale = new Vector2(-1f,1f);
        }

        yield return StartCoroutine(GoToPos.MoveCoroutine(cat,goal,duration));
        alreadyRunning = false;
    }
}
