using System.Collections;
using UnityEngine;

public static class GoToPos
{
    public static void MovePos(MonoBehaviour runner, Transform target, Vector3 endPos, float duration)
    {
        runner.StartCoroutine(MoveCoroutine(target, endPos, duration));
    }

    public static IEnumerator MoveCoroutine(Transform target, Vector3 endPos, float duration)
    {
        Vector3 startPos = target.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            if (target!=null)
                target.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (target!=null)
            target.position = endPos;
    }
}