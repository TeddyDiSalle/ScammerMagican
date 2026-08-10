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

    void Start()
    {
        SetupPortalVisual();
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

    private void SetupPortalVisual()
    {
        if (portal == null)
            return;

        Sprite[] portalFrames = animSprite.LoadSpritesFromResources("Art/PortalAnim");
        if (portalFrames == null || portalFrames.Length == 0)
            return;

        foreach (SpriteRenderer sr in portal.GetComponentsInChildren<SpriteRenderer>(true))
            sr.enabled = false;

        Transform visual = portal.Find("AnimatedPortal");
        if (visual == null)
        {
            GameObject visualGO = new GameObject("AnimatedPortal");
            visualGO.transform.SetParent(portal, false);
            visualGO.transform.localPosition = Vector3.zero;
            visualGO.transform.localScale = Vector3.one;
            visual = visualGO.transform;
        }

        SpriteRenderer renderer = visual.GetComponent<SpriteRenderer>();
        if (renderer == null)
            renderer = visual.gameObject.AddComponent<SpriteRenderer>();

        renderer.enabled = true;
        renderer.sortingOrder = 1;
        renderer.sprite = portalFrames[0];

        animSprite animator = visual.GetComponent<animSprite>();
        if (animator == null)
            animator = visual.gameObject.AddComponent<animSprite>();

        animator.sprites = portalFrames;
        animator.frameRate = 10f;
        animator.startAt0 = true;
        animator.holdLastFrame = false;
        animator.useUnscaledTime = true;
        animator.RestartAnimation();
    }
}
