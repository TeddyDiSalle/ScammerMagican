using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class won : MonoBehaviour
{
    public string [] slightWinLines;
    public string [] fullWinLines;
    public dialogue chatManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void doWin(bool fullWin)
    {
        if (fullWin)
            chatManager.addLine(fullWinLines[Random.Range(0,fullWinLines.Length)]);
        else
            chatManager.addLine(slightWinLines[Random.Range(0,slightWinLines.Length)]);
    }
}
