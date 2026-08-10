using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentBall : MonoBehaviour
{
    public Transform ball;

    public void SetParent(Transform[] cups)
    {
        if (ball == null || cups == null)
            return;

        foreach (Transform cup in cups)
        {
            // The previous round may have destroyed this cup.
            if (cup == null)
                continue;

            if (Mathf.Approximately(
                cup.position.x,
                ball.position.x))
            {
                ball.SetParent(cup);
                return;
            }
        }
    }
}