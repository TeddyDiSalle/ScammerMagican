using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class healthBar : MonoBehaviour
{
    public RectTransform bar;
    public float totalWidth;
    public float currentPercent = 1f;
    public float goalPercent = 1f;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        currentPercent = 1f;
        goalPercent = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        currentPercent = (Mathf.MoveTowards(currentPercent, goalPercent, speed * Time.deltaTime));
        setBar(currentPercent);
    }

    public void setHealth(float percent)
    {
        goalPercent = percent;
    }

    public void setBar(float percent)
    {
        bar.sizeDelta = new Vector2(totalWidth*percent,bar.sizeDelta.y);
        bar.anchoredPosition = new Vector2((totalWidth-totalWidth*percent)/-2f,bar.anchoredPosition.y);
    }
}
