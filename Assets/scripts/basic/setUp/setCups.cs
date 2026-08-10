using System.Collections.Generic;
using UnityEngine;

public class setCups : MonoBehaviour
{
    public GameObject cupPrefab;
    public float border;
    public float cupSpriteSize;
    public int cupsAmt;
    public float spacingAspectRatio; // how big is a space compared to the cup
    public float cupScaleAspectRatio;
    public Transform ball;
    public float setY;
    public LowerCup cupLower;

    private List<Transform> cups;

    public void MakeCups()
    {
        cups = new List<Transform>();

        float totalWidth = 2 * border;
        float cupDistance = totalWidth / cupsAmt;
        float cupWidth =
            totalWidth / (cupsAmt + (cupsAmt - 1) * spacingAspectRatio);
        float cupHeight = cupWidth / cupScaleAspectRatio;

        float ballYPos = -1.8f - cupHeight * 0.25f;
        ball.localScale = Vector2.one * cupWidth / 2f;

        int middle = Mathf.FloorToInt((float)cupsAmt / 2f);

        for (int i = 0; i < cupsAmt; i++)
        {
            GameObject cup = Instantiate(cupPrefab);
            cups.Add(cup.transform);

            cup.transform.localScale =
                new Vector2(cupWidth, cupWidth) / cupSpriteSize;

            cup.transform.position =
                new Vector2(
                    -border + cupDistance / 2f + cupDistance * i,
                    setY
                );

            cup.name = "cup" + i;

            if (i == middle)
                ball.position = new Vector2(cup.transform.position.x, ballYPos);
        }

        cupLower.cups = cups.ToArray();
    }

    public List<Vector2> getCupPositions()
    {
        List<Vector2> positions = new List<Vector2>();

        float totalWidth = 2 * border;
        float cupDistance = totalWidth / cupsAmt;

        for (int i = 0; i < cupsAmt; i++)
        {
            positions.Add(
                new Vector2(
                    -border + cupDistance / 2f + cupDistance * i,
                    setY
                )
            );
        }

        return positions;
    }
}
