using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class fadeUp
{
    public static IEnumerator MoveUpAndFade(GameObject obj, float amount, float duration)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Vector3 startPos = obj.transform.position;
        Color startColor = sr.color;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            if (obj)
                obj.transform.position = startPos + Vector3.up * (amount * progress);
            if (sr)
                sr.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(startColor.a, 0f, progress));

            yield return null;
        }

        //obj.transform.position = startPos + Vector3.up * amount;
        //sr.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        UnityEngine.Object.Destroy(obj);
    }
}
