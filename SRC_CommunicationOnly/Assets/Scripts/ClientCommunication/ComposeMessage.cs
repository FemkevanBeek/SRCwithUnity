using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class ComposeMessage : MonoBehaviour
{

    //this thread handles continuous outgoing data
    static float updateTimestep;
    static float timeStarted;
    static float update = 0.0f;
    //ramp
    static float timeHighSignalRamp = 4.0f;
    //step
    static float timeHighSignalStep = 4.0f;
    //sine
    static float timeHighSignalSine = 15.0f;
    //pneunet
    static float timeHighSignalPneu = 5.0f;

    static float timeHighSignal;
    static float timeStartSignal = 3.0f;
    static float timeCurrent;

    //TO DO: think about if we can initialize this with a setting that says something about the length of the signal
    static double[] currentSignal;

    //initialize simulation variables
    int selectorVariable;
    static double simSignal;
    bool sendingActivated = false;
    bool handReady = false;
    static int counter = 0;
    StringBuilder sb = new StringBuilder();

    private void Start()
    {
            updateTimestep = 1.0f / Settings.updateFrequency;

            currentSignal = new double[Settings.fingerAngleIndex.Length];

        if (Settings.storingData == true)
        {
            Settings.filePath = Settings.filePath + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".txt";

            //StringBuilder sb = new StringBuilder();
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

        timeStarted = Time.realtimeSinceStartup;
    }

    private void Update()
    {
            if (ClientHandle.welcomeAccepted)
            {

                //if ((Input.GetKeyDown("space") && handReady == true && Settings.startOnPlay==false))
                //{
                //    if (sendingActivated == false)
                //    {
                //        sendingActivated = true;
                //        AudioSource audio = GetComponent<AudioSource>();
                //        audio.clip = spacePressedClip;
                //        audio.Play();
                //        print("System started");
                //    }
                //    else if (sendingActivated == true)
                //    {
                //        sendingActivated = false;
                //        AudioSource audio = GetComponent<AudioSource>();
                //        audio.clip = pausedClip;
                //        audio.Play();
                //        print("Tele-operation paused");

                //        for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                //        {
                //            currentSignal[i] = 0;
                //        }
                //        //Send zeros to system to put it at resting state
                //        SendInputToServer(currentSignal);
                //    }
                //}

                if(Settings.startOnPlay == true)
            {
                sendingActivated = true;
            }

            //if (Input.GetKeyDown("space") && timerStarted == false)
            //{
             //   
              //  timerStarted = true;
            //}

            update += Time.deltaTime;               //this controls the timing of the loop
            if (update > updateTimestep)
            {
                if (!Settings.simulateLeap)
                {
                    for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                    {
                        currentSignal[i] = 0;
                    }

                }
                else
                {

                    simSignal = 0.52f;

                    timeCurrent = Time.realtimeSinceStartup - timeStarted;

                    if (timeCurrent > timeStartSignal) {

                        if (Settings.stepInput == true)
                        {
                            simSignal = 0.625f;
                            timeHighSignal = timeHighSignalStep;
                        }

                        if(Settings.rampInput == true)
                        {
                            simSignal = 0.52f + 0.1f*(timeCurrent - timeStartSignal);
                            timeHighSignal = timeHighSignalRamp;
                            //counttime++;
                        }

                        if (Settings.sineInput == true)
                        {
                            simSignal = 0.57 + 0.05f * Mathf.Sin(Mathf.PI * Settings.simulationFrequency * (timeCurrent - timeStartSignal));
                            timeHighSignal = timeHighSignalSine;
                        }

                        if (Settings.pneuInput == true)
                        {
                            simSignal = 0.52f + 0.09f * (timeCurrent - timeStartSignal);
                            timeHighSignal = timeHighSignalPneu;
                        }

                        if (timeCurrent > timeHighSignal)
                        {
                            simSignal = 0.52f;
                        }

                    }

                    for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                    {
                        currentSignal[i] = (double)simSignal;
                    }

                    //Debug.Log($"timerStarted is {timerStarted}");
                }

                if (sendingActivated == true)
                {
                    SendInputToServer(currentSignal);

                    if (Settings.dataToConsole)
                    {
                        for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                        {
                            Debug.Log($"Trying to send: {currentSignal[i]} for finger {Settings.fingerAngleIndex[i]}");
                        }
                    }

                }
                
                if(Settings.storingData==true)
                { 
               // StringBuilder sb = new StringBuilder();
                
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
                    sb.AppendLine();
                    
                }

                update = 0.0f;
                counter ++;
                }
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
            //controller.StopConnection();
            //controller.Dispose();
    }
}