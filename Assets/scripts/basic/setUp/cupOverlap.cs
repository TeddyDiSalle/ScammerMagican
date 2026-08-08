using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cupOverlap : MonoBehaviour
{
    public int inBack;
    public int furthestBack;
    public SpriteRenderer spriteRenderer;
    public Color darkest;
    public float speed;
    List<cupOverlap> cupsInColliding = new List<cupOverlap>(0);
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sortingOrder = 0 - inBack;
    }

    // Update is called once per frame
    void Update()
    {
        int maxBack = -1;
        foreach (cupOverlap cup in cupsInColliding){
            if (cup.inBack>maxBack)
                maxBack = cup.inBack;
            while (inBack==cup.inBack)
            {
                if (Random.Range(1,51)==1){
                    inBack++;
                    setInBack();
                }
            }
        }
        if (inBack-1>maxBack)
        {
            inBack --;
            setInBack();
        }
    }

    IEnumerator setToColor()
    {
        while (true)
        {
            Color goalColor = Color.Lerp(Color.white, darkest, inBack / (float)furthestBack);

            
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                goalColor,
                speed * Time.deltaTime
            );

            if (Vector4.Distance(spriteRenderer.color, goalColor) < 0.01f)
            {
                spriteRenderer.color = goalColor;
                yield break;
            }

            yield return null;
        }
        
    }

    void setInBack()
    {
        spriteRenderer.sortingOrder = 0 - inBack;
        //spriteRenderer.color = Color.Lerp(Color.white, darkest, inBack/(float)furthestBack);
        StartCoroutine(setToColor());
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        cupsInColliding.Add(other.GetComponent<cupOverlap>());


        //Debug.Log(cupsInColliding.Count);
        //if (cupsInColliding.Count>1)
            //Debug.LogWarning(cupsInColliding.Count);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        cupsInColliding.Remove(other.GetComponent<cupOverlap>());
    }
}
