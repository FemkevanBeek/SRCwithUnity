using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]

public class Send2DSliderData : MonoBehaviour
{
    private InteractionBehaviour _intObj;
    [HideInInspector]
    public float sendData2DSliderHorizontal = 0;
    [HideInInspector]
    public float sendData2DSliderVertical = 0;
    [HideInInspector]
    public bool isEngaged = false;
    [HideInInspector]
    public int Finger;

    // Start is called before the first frame update
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        Finger = 99;

        _intObj.OnContactBegin -= OnContactBegin;
        _intObj.OnContactBegin += OnContactBegin;

        _intObj.OnContactEnd -= OnContactEnd;
        _intObj.OnContactEnd += OnContactEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_intObj as InteractionSlider).isPressed || (_intObj as InteractionSlider).isGrasped)
        {
            isEngaged = true;
            sendData2DSliderHorizontal = (_intObj as InteractionSlider).normalizedHorizontalValue;
            //Debug.Log($"Sending 2D Slider horizontal {sendData2DSliderHorizontal}");
            sendData2DSliderVertical = (_intObj as InteractionSlider).normalizedVerticalValue;
            //Debug.Log($"Sending 2D Slider vertical {sendData2DSliderVertical}");

        }
        else
        {
            isEngaged = false;
            sendData2DSliderHorizontal = 0;
            sendData2DSliderVertical = 0;
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
