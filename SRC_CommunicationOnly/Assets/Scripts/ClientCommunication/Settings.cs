using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    //make sure ip and port are the same as the ones in Simulink
    //127.0.0.1 means local
    public string _ip = "127.0.0.1";
    //port 26950 is usually open
    public int _port = 26950;
    //delayed start, or start directly when program is run
    public bool _startOnPlay = true;
    //Set to true to send out data to Matlab
    public bool _sendingData = true;
    //Set to true to receive data from Matlab
    public bool _receivingData = false;
    //Set to true to send out data to Matlab
    public bool _storingData = false;
    //step input for performance testing
    public bool _stepInput;
    //ramp input for performance testing
    public bool _rampInput;
    //sine input for performance testing
    public bool _sineInput;
    //ramp input for Pneunet performance testing
    public bool _pneuInput;
    //Path of data file
    public string _filePath = "Data/Testfile";
    //Update frequency for pressurecontrol loop
    public float _updateFrequency = 120.0f;
    //Are we printing data to the console on each tick
    public bool _dataToConsole = true;
    //Are we printing collision data to the console
    public bool _collisionToConsole = true;
    //If checked, we simulate leap motion data
    public bool _simulateLeap = false;
    //frequency of simulated block signal
    public float _simulationFrequency = 0.25f;
    //index of finger(s) that we're tracking. Numbered 0-4, thumb-pinky
    public int[] _fingerAngleIndex = new int[] { 0, 1, 2 };

    //initialize static copy of fields, because we can only display non-static fields in Unity
    public static string ip;
    public static int port;
    public static bool startOnPlay;
    public static bool sendingData;
    public static bool receivingData;
    public static bool storingData;
    public static bool stepInput;
    public static bool rampInput;
    public static bool sineInput;
    public static bool pneuInput;
    public static string filePath;
    public static float updateFrequency;
    public static bool dataToConsole;
    public static bool collisionToConsole;
    public static bool simulateLeap;
    public static float simulationFrequency;
    public static int[] fingerAngleIndex;

    private void Awake()
    {
        ip = _ip;
        port = _port;
        startOnPlay = _startOnPlay;
        updateFrequency = _updateFrequency;
        dataToConsole = _dataToConsole;
        collisionToConsole = _collisionToConsole;
        simulateLeap = _simulateLeap;
        sendingData = _sendingData;
        receivingData = _receivingData;
        storingData = _storingData;

        stepInput = _stepInput;
        rampInput = _rampInput;
        sineInput = _sineInput;
        pneuInput = _pneuInput;

    filePath = _filePath;
        simulationFrequency = _simulationFrequency;
        fingerAngleIndex = _fingerAngleIndex;

    }
}
