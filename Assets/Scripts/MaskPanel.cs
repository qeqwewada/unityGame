using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskPanel : BasePanel
{
    [SerializeField] private Text _text;
    public void ShowMessage(string msg)
    {
        _text.text = msg;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
