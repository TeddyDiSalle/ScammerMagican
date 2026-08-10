using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI text;
    public List<string> lines = new List<string>();

    public bool midLine;
    public float charsPerSecond = 30f;
    public float holdTextDuration = 1f;

    void Update()
    {
        if (!midLine && lines.Count > 0)
        {
            StartCoroutine(typeLine(lines[0]));
        }
    }

    public void addLine(string line)
    {
        if (string.IsNullOrEmpty(line))
            return;

        lines.Add(line);
    }

    IEnumerator typeLine(string line)
    {
        if (string.IsNullOrEmpty(line))
            yield break;

        // Since this coroutine was started using lines[0],
        // remove exactly the first queued line.
        if (lines.Count > 0)
            lines.RemoveAt(0);

        midLine = true;

        float chars = 0f;

        // TYPE THE TEXT IN
        while (chars < line.Length)
        {
            chars += charsPerSecond * Time.deltaTime;

            // Never allow Substring to go beyond line.Length.
            int characterCount = Mathf.Clamp(
                (int)chars,
                0,
                line.Length
            );

            text.text = line.Substring(
                0,
                characterCount
            );

            yield return null;
        }

        // Make sure the full line is visible.
        text.text = line;

        yield return new WaitForSeconds(holdTextDuration);

        // ERASE THE TEXT
        chars = line.Length;

        while (chars > 0f)
        {
            chars -= charsPerSecond * Time.deltaTime * 3f;

            // Never allow Substring to receive a negative length.
            int characterCount = Mathf.Clamp(
                (int)chars,
                0,
                line.Length
            );

            text.text = line.Substring(
                0,
                characterCount
            );

            yield return null;
        }

        text.text = "";
        midLine = false;
    }
}