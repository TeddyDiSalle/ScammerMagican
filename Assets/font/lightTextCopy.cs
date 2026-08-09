using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class lightTextCopy : MonoBehaviour
{
    public TextMeshProUGUI original;
    public TextMeshProUGUI copy;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        copy.text = original.text;
        copy.enabled = original.enabled;
    }
}
