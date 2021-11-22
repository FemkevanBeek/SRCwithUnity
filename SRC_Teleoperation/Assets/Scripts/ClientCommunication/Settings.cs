using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{

    [Tooltip("Should match receiving end. 127.0.0.1 means local")]
    public string _ip = "127.0.0.1";
    [Tooltip("Should match receiving end. Port 26950 is usually open")]
    public int _port = 26950;
    [Tooltip("Set to false to wait for space press to start sending data, or set to true to start sending data directly when program is run")]
    public bool _startOnPlay = true;
    [Tooltip("Set to true to send data out")]
    public bool _sendingData = true;
    [Tooltip("Set to true to receive data")]
    public bool _receivingData = false;
    [Tooltip("Set to true to store data in text file")]
    public bool _storingData = false;
    [Tooltip("Path of data file")]
    public string _filePath = "Data/Testfile";
    [Tooltip("Update frequency for ComposeMessage loop. Leap Motion system should be running at 120 Hz, so 120 is default.")]
    public float _updateFrequency = 120.0f;
    [Tooltip("Set to true to print data to console on each loop")]
    public bool _dataToConsole = true;
    [Tooltip("Index of finger(s) that we're tracking. Numbered 0-4, thumb-pinky")]
    public int[] _fingerAngleIndex = new int[] { 0, 1, 2 };

    //initialize static copy of fields, because we can only display non-static fields in Unity
    public static string ip;
    public static int port;
    public static bool startOnPlay;
    public static bool sendingData;
    public static bool receivingData;
    public static bool storingData;
    public static string filePath;
    public static float updateFrequency;
    public static bool dataToConsole;
    public static bool collisionToConsole;
    public static int[] fingerAngleIndex;

    private void Awake()
    {
        ip = _ip;
        port = _port;
        startOnPlay = _startOnPlay;
        updateFrequency = _updateFrequency;
        dataToConsole = _dataToConsole;
        sendingData = _sendingData;
        receivingData = _receivingData;
        storingData = _storingData;
        filePath = _filePath;
        fingerAngleIndex = _fingerAngleIndex;

    }
}
