using System.Collections;
using UnityEngine;

public class fadeWClick : MonoBehaviour
{
    private bool transitioning;

    void OnMouseDown()
    {
        if (transitioning)
            return;

        finale finaleController =
            GetComponent<finale>();

        if (finaleController != null &&
            !finaleController.IsPrepared())
        {
            return;
        }

        Transform nextCup = FindNextNestedCup();

        if (nextCup != null)
        {
            RevealNextCup(nextCup);
            return;
        }

        // No nested cup remains. The only child may be the ball.
        // This is the actual end of the game.
        FinishGame();
    }

    private Transform FindNextNestedCup()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.GetComponent<fadeWClick>() != null)
                return child;
        }

        return null;
    }

    private void RevealNextCup(Transform nextCup)
    {
        transitioning = true;

        Collider2D currentCollider =
            GetComponent<Collider2D>();

        if (currentCollider != null)
            currentCollider.enabled = false;

        Collider2D nextCollider =
            nextCup.GetComponent<Collider2D>();

        if (nextCollider != null)
            nextCollider.enabled = true;

        // Preserve the nested cup's current world size and exact position
        // as it becomes the new clickable top-level cup.
        nextCup.SetParent(transform.parent, true);

        StartCoroutine(
            fadeUp.MoveUpAndFade(
                gameObject,
                2f,
                0.5f
            )
        );
    }

    private void FinishGame()
    {
        transitioning = true;

        Collider2D currentCollider =
            GetComponent<Collider2D>();

        if (currentCollider != null)
            currentCollider.enabled = false;

        // Lift/fade the very last cup so the ball gets the final reveal.
        StartCoroutine(
            fadeUp.MoveUpAndFade(
                gameObject,
                2f,
                0.5f
            )
        );

        GameWinOverlay.Show();
    }
}
