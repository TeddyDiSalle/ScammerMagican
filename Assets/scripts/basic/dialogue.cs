using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class dialogue : MonoBehaviour
{
    public TextMeshProUGUI text;
    public List<string> lines;
    public bool midLine;
    public float charsPerSecond;
    public float holdTextDuration;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!midLine&&lines.Count>0)
            StartCoroutine(typeLine(lines[0]));
    }

    public void addLine(string line)
    {
        lines.Add(line);
    }

    IEnumerator typeLine(string line)
    {
        lines.Remove(line);
        midLine = true;
        float chars = 0;

        while (chars<line.Length)
        {
            chars += charsPerSecond*Time.deltaTime;
            text.text = line.Substring(0,(int)chars);
            yield return null;
        }

        yield return new WaitForSeconds(holdTextDuration);

        while (chars>0)
        {
            chars -= charsPerSecond*Time.deltaTime*3f;
            text.text = line.Substring(0,(int)chars);
            yield return null;
        }

        midLine = false;
    }
}
