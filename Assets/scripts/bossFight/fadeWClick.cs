using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeWClick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        if (transform.childCount>0)
        {
            if (transform.GetChild(0).GetComponent<Collider2D>() == null && transform.GetChild(0).childCount > 0)
            {
                transform.GetChild(0).GetChild(0).GetComponent<Collider2D>().enabled = true;
                transform.GetChild(0).GetChild(0).parent = transform.parent;
                
                StartCoroutine(fadeUp.MoveUpAndFade(gameObject,2f,.5f));
                return;
            }
            
            if (transform.GetChild(0).GetComponent<Collider2D>() != null)
                transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
            transform.GetChild(0).parent = transform.parent;
            StartCoroutine(fadeUp.MoveUpAndFade(gameObject,2f,.5f));
        }
    }
}
