using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public float delay;
    public float duration;
    public rollToOther roller;
    public Transform portal;
    public Transform ball;
    public float downAmount;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float doTP(Transform [] cups)
    {
        roller.doRoll(cups);

        

        return delay;
    }

    public void makePortal(Transform cup)
    {
        Transform tempPortal = Instantiate(portal.gameObject,portal.position,portal.rotation).transform;
        tempPortal.position = new Vector2(cup.position.x,tempPortal.position.y);

        tempPortal.localScale = ball.lossyScale/2f;

        StartCoroutine(popDown(tempPortal,downAmount,ball.lossyScale,duration));
    }

    public IEnumerator popDown(Transform obj, float downAmount, Vector3 targetScale, float duration)
    {
        Vector3 startPos = obj.position;
        Vector3 startScale = obj.localScale;
        float t = 0f;

        float semiDuration = duration/3f;
        while (t < semiDuration)
        {
            t += Time.deltaTime;
            float p = t / semiDuration;

            obj.position = Vector3.Lerp(startPos, startPos + Vector3.down * downAmount, p);
            obj.localScale = Vector3.Lerp(startScale, targetScale, p);

            yield return null;
        }

        yield return new WaitForSeconds(semiDuration);

        t = 0f;
        while (t < semiDuration)
        {
            t += Time.deltaTime;
            float p = t / semiDuration;

            obj.position = Vector3.Lerp(startPos + Vector3.down * downAmount, startPos, p);
            obj.localScale = Vector3.Lerp(targetScale, startScale, p);

            yield return null;
        }

        Destroy(obj.gameObject);
    }
}
