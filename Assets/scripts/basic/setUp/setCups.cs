using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class setCups : MonoBehaviour
{
    public GameObject cupPrefab;
    public float border;
    public int cupsAmt;
    public float spacingAspectRatio;//how big is a space compared to the cup
    public float cupScaleAspectRatio;
    public Transform ball;
    public float setY;
    public LowerCup cupLower;
    private List<Transform> cups;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MakeCups()
    {
        cups = new List<Transform>(0);

        float totalWidth = 2*border;
        float cupDistance = totalWidth/cupsAmt;
        float cupWidth = totalWidth/(cupsAmt + (cupsAmt-1)*spacingAspectRatio);

        for (int i = 0; i < cupsAmt; i++){
            GameObject cup = Instantiate(cupPrefab);
            cups.Add(cup.transform);
            cup.transform.localScale = new Vector2(cupWidth,cupWidth/cupScaleAspectRatio);
            cup.transform.position = new Vector2(-border+cupDistance/2f+cupDistance*i,setY);
            cup.name = "cup"+i.ToString();

            if (i==0)//first one
                ball.transform.position = new Vector2(cup.transform.position.x,ball.transform.position.y);
        }

        cupLower.cups = cups.ToArray();
    }
}
