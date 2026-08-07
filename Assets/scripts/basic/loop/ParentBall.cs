using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentBall : MonoBehaviour
{
    public Transform ball;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetParent(Transform [] cups)
    {
        foreach (Transform cup in cups)
            if (cup.position.x==ball.position.x)
                ball.SetParent(cup);
    }
}
