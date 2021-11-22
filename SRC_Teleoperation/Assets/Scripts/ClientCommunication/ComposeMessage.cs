using UnityEngine;
using Leap;
using System.IO;
using System;
using System.Text;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class ComposeMessage : MonoBehaviour
{
    //initialize variables that are visible in Unity
    public AudioClip introClip;
    public AudioClip spacePressedClip;
    public AudioClip pausedClip;

    //Initialize variables that don't show up in Unity
    static float updateTimestep;
    static float update = 0.0f;
    bool sendingActivated = false;
    bool handReady = false;
    static int counter = 0;
    static double[] currentSignal;
    StringBuilder sb = new StringBuilder();

    private void Start()
    {
        updateTimestep = 1.0f / Settings.updateFrequency;
        currentSignal = new double[Settings.fingerAngleIndex.Length];
        
        //initialize Leap Motion variables    
        LeapMotionListener.AverageAngle = new float[Settings.fingerAngleIndex.Length];
        LeapMotionListener.AllDirections = new float[] { 0, 0, 0 };

        //set up Controllers, necessary for reading Leap Motion data   
        Controller controller = new Controller();
        LeapMotionListener listener = new LeapMotionListener();
        controller.Device += listener.OnConnect;
        controller.FrameReady += listener.OnFrame;

        if (Settings.storingData == true)
        {
            Settings.filePath = Settings.filePath + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".txt";
            sb.Append("Time [s] , Frame nr");

            for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
            {
                sb.Append(",");
                sb.Append($"Sent pressure {Settings.fingerAngleIndex[i]}");
            }

            if (Settings.receivingData == true)
            {
                for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                {
                    sb.Append(",");
                    sb.Append($"Received pressure {Settings.fingerAngleIndex[i]}");
                }
            }

            sb.AppendLine();
        }
    }

    private void Update()
    {

        if(Settings.startOnPlay == true)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = spacePressedClip;
            audio.Play();
            sendingActivated = true;
        }
        else if(Settings.startOnPlay == false)
        {
            
            if (LeapMotionListener.rightHandEnabled && handReady == false)
            {
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = introClip;
                audio.Play();
                handReady = true;
            }

            if (Input.GetKeyDown("space") && handReady == true)

                if (sendingActivated == false) { 
                    AudioSource audio = GetComponent<AudioSource>();
                    audio.clip = spacePressedClip;
                    audio.Play();
                    sendingActivated = true;
                }
                else
                {
                    AudioSource audio = GetComponent<AudioSource>();
                    audio.clip = pausedClip;
                    audio.Play();

                    //send out zero pressure to reset the actuators when we pause tracking
                    for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                    {
                        currentSignal[i] = 0;
                    }

                    SendInputToServer(currentSignal);
                    Debug.Log("Trying to send actuator reset");
                    sendingActivated = false;
                }

        }

        update += Time.deltaTime;
            if (update > updateTimestep)
            {

                if (sendingActivated == true)
                {
                    for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                    {
                        currentSignal[i] = (double)LeapMotionListener.AverageAngle[i];
                    }

                    SendInputToServer(currentSignal);

                    if (Settings.dataToConsole)
                    {
                        for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                        {
                            Debug.Log($"Trying to send: {currentSignal[i]} for finger {Settings.fingerAngleIndex[i]}");
                        }
                    }

                    if (Settings.storingData == true)
                    {
                        sb.Append(Time.realtimeSinceStartup);
                        sb.Append(",");
                        sb.Append(counter);

                        for (int index = 0; index < currentSignal.Length; index++)
                        {
                            sb.Append(",");
                            sb.Append(currentSignal[index]);

                            if (Settings.receivingData == true)
                            {
                                sb.Append(",");
                                sb.Append(ClientHandle.pressureReceived[index]);
                            }
                        }
                    
                        //here more data can be stored, if this is also reflected in the header
                        sb.AppendLine();

                    }

                }

                update = 0.0f;
                counter ++;
             }

    }

    private void SendInputToServer(double[] _pressureMessage)
    {
        ClientSend.SendPressure(_pressureMessage);
        //Debug.Log("Sending out pressure");
    }

    private void OnApplicationQuit()
    {
        if (!File.Exists(Settings.filePath))
        {
            File.WriteAllText(Settings.filePath, sb.ToString());
        }
        else
        {
            File.AppendAllText(Settings.filePath, sb.ToString());
        }
        Debug.Log("Quitting");
    }
}