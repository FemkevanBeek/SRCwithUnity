using Leap.Unity;
using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
[RequireComponent(typeof(InteractionBehaviour))]

public class SendButtonData : MonoBehaviour
{
    private InteractionBehaviour _intObj;
  //  private GameObject _settingsObj;
    [HideInInspector]
    public float sendDataButton = 0;
    [HideInInspector]
    public bool isEngaged = false;
    [HideInInspector]
    public int Finger;

   // public string[] _fingerButColliders = new string[] { "thumb", "index", "middle" };
   // public string[] _boneButColliders = new string[] { "bone3", "bone3", "bone3" };
   // public Finger sendDataFinger;
   //static public enum Finger sendDataFinger;

    // Start is called before the first frame update
    void Start()
    {
        _intObj = GetComponent<InteractionBehaviour>();
        Finger = 99; //random number to indicate that nothing is touching

        _intObj.OnContactBegin -= OnContactBegin;
        _intObj.OnContactBegin += OnContactBegin;

        _intObj.OnContactEnd -= OnContactEnd;
        _intObj.OnContactEnd += OnContactEnd;
    }

    // Update is called once per frame
    void Update()
    {
        if ((_intObj as InteractionButton).pressedAmount > 0.01)
        {
            isEngaged = true;
            sendDataButton = (_intObj as InteractionButton).pressedAmount;
        }
        else
        {
            isEngaged = false;
            sendDataButton = 0;
        }

    }

    private void OnContactBegin()
    {
        if(!(_intObj.primaryHoveringFinger==null)) { 
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
