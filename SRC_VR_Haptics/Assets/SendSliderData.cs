using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]

public class SendSliderData : MonoBehaviour
{
    private InteractionBehaviour _intObj;
    //private InteractionBehaviour _intObj_finger;
    [HideInInspector]
    public float sendDataSlider = 0;
    [HideInInspector]
    public bool isEngaged = false;
    [HideInInspector]
    public int Finger;

    // Start is called before the first frame update
    void Start()
    {
        //_intObj = GetComponent<InteractionBehaviour>();
        
        _intObj = GetComponent<InteractionBehaviour>();
        Finger = 99;
        isEngaged = false;

        _intObj.OnContactBegin -= OnContactBegin;
        _intObj.OnContactBegin += OnContactBegin;
        _intObj.OnContactEnd -= OnContactEnd;
        _intObj.OnContactEnd += OnContactEnd;

    }

    //// Update is called once per frame
    void Update()
    {
        if ((_intObj as InteractionSlider).isPressed || (_intObj as InteractionSlider).isGrasped)
        {
            isEngaged = true;
            sendDataSlider = (_intObj as InteractionSlider).normalizedHorizontalValue;
        }
        else
        {
            isEngaged = false;
            sendDataSlider = 0;
        }
    }


    private void OnContactBegin()
    {
        if (!(_intObj.primaryHoveringFinger == null))
        {
            Finger = (int)_intObj.primaryHoveringFinger.Type;
        }
        //Debug.Log($"Finger {Finger} starts touching");
    }

    private void OnContactEnd()
    {
        Finger = 99;
        //Debug.Log("Not hovering anymore");
    }
}
