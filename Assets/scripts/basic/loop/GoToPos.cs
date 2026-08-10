using System.Collections;
using UnityEngine;

public static class GoToPos
{
    public static void MovePos(
        MonoBehaviour runner,
        Transform target,
        Vector3 endPos,
        float duration)
    {
        if (runner == null || target == null)
            return;

        runner.StartCoroutine(
            MoveCoroutine(target, endPos, duration)
        );
    }

    public static IEnumerator MoveCoroutine(
        Transform target,
        Vector3 endPos,
        float duration)
    {
        // Object may have already been destroyed.
        if (target == null)
            yield break;

        Vector3 startPos = target.position;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            if (target != null)
                target.position = endPos;

            yield break;
        }

        while (elapsed < duration)
        {
            // Cup may have been destroyed during the coroutine.
            if (target == null)
                yield break;

            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            target.position = Vector3.Lerp(
                startPos,
                endPos,
                t
            );

            yield return null;
        }

        // Check one final time.
        if (target != null)
            target.position = endPos;
    }
}