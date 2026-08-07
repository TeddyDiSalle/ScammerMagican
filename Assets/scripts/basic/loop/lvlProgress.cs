using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lvlProgress : MonoBehaviour
{
    public TextMeshProUGUI lostText;
    public gameOrder resetter;
    private bool done;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (done&&Input.anyKeyDown)
            reset();
    }

    public void reset()
    {
        done = false;
        lostText.enabled = false;
        foreach (GameObject cup in GameObject.FindGameObjectsWithTag("cup"))
            Destroy(cup);
        resetter.callGame();
    }

    public void lost()
    {
        done = true;
        lostText.enabled = true;
    }

    public void won()
    {
        done = true;
        reset();
    }
}
