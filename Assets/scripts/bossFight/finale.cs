using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finale : MonoBehaviour
{
    public Transform finaleCupProgression;
    public Transform ball;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(GetComponent<ChooseCup>());
        Destroy(GetComponent<cupOverlap>());

        finaleCupProgression = GameObject.Find("finalCupProgression").transform;
        finaleCupProgression.parent =  transform;
        finaleCupProgression.localScale = Vector3.one;
        finaleCupProgression.localPosition = Vector3.zero;
        GetComponent<SpriteRenderer>().sortingOrder = 0;

        gameObject.AddComponent<fadeWClick>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ball.parent = transform){
            ball.parent = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
            Debug.Log(transform.GetChild(0).GetChild(0).GetChild(0));
            Debug.Log(transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0));
            Debug.Log(ball.parent);
            ball.localScale = Vector2.one*12f;
            Destroy(this);
        }
    }


    
}
