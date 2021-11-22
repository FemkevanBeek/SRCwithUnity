using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    //this class processes incoming data. It triggers atomatically whenever data comes in,
    //and if it has the right packetID, it will enter the right function call.
    //do make sure to intiazlize extra packets in Client.InitializeClientData
    public static bool welcomeAccepted = true;
    public static double[] pressureReceived = new double[Settings.fingerColliders.Length];

    public static void Welcome(Packet _packet)
    { 
        string _msg = _packet.ReadString();
        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = Client.ExtraId;                           //there is only 1 client
        welcomeAccepted = true;
    }

    public static void ReceivePressure(Packet _packet)
    {
        int _nrPressures = _packet.ReadInt();

        for (int i = 0; i < _nrPressures; i++)
        {
            pressureReceived[i] = _packet.ReadDouble();
            if (Settings.dataToConsole)
            {
                Debug.Log($"Pressure received: {pressureReceived[i]} for finger {Settings.fingerColliders[i]}");
            }
        }
    }

}
