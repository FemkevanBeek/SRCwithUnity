using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        if (Settings.sendingData == true)
        {
            Client.instance.tcp.SendData(_packet);
        }
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            //_packet.Write(Client.statWelcomeMessage);
            //SendTCPData(_packet);
            //commenting out because we are not sending this any more
            Debug.Log("Sending welcome data.");
        }
    }

    public static void SendPressure(double[] _pressures)
    {
        if (ClientHandle.welcomeAccepted)       //only start sending pressure once welcome message came through
        {
            using (Packet _packet = new Packet((int)ClientPackets.pressureData))
            {

                _packet.Write(_pressures.Length);
                foreach (double _pressure in _pressures)
                {
                    _packet.Write(_pressure);
                }//check to see if we can write multiple numbers (multiple write commands), and then just send a single packet
                //Debug.Log("writing a pressure");
                SendTCPData(_packet);
            }
        }
    }
    #endregion
}