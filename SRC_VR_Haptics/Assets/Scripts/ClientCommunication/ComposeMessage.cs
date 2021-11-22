using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leap;
using System.IO;
using System;
using System.Text;


[RequireComponent(typeof(AudioSource))]
public class ComposeMessage : MonoBehaviour
{
    //distance from button of button before click happens
    public float maxButtonTickDistance = 0.025f;
    //pressure at click
    public float buttonTickAmplitude = 0.5f;
    //duration of click. if you set this to 0, you functionally disable the click
    public float buttonTickTime = 0.25f;
    public AudioClip buttonClip;

    //distance from actual tick mark before tick engages
    public float maxSliderTickDistance = 0.025f;
    //pressure at tick
    public float sliderTickAmplitude = 0.6f;
    //duration of tick
    public float sliderTickTime = 0.25f;
    //normalized position of tick
    public float[] sliderTicks = { 0.2f, 0.4f, 0.6f, 0.8f };
    public AudioClip sliderClip;

    //max amplitude of 2D slider (reached at complete right side of screen)
    public float slider2DMaxAmplitude = 1.0f;
    //max frequency of 2D slider (reached at complete top of screen)
    public float slider2DMinAmplitude = 0.3f;

    public float lowerTwoDFreqSlider = 1.5f;
    public float upperTwoDFreqSlider = 3.0f;
    public AudioClip slider2DClipLow;
    public AudioClip slider2DClipHigh;
    AudioClip currentClip;

    //this thread handles continuous outgoing data
    static float updateTimestep;
    static float update = 0.0f;
    static float startEngageTime;
    bool engageStarted = false;
    float currentTime;
    static double[] currentSignal;
    static int counter = 0;

    //initialize simulation variables
    private float tempSignal;
    int currentFinger;

    public AudioSource source;

    public GameObject buttonObject;
    [HideInInspector]
    public GameObject[] allButtons;
    [HideInInspector]
    public GameObject[] all2DSliders;
    [HideInInspector]
    public GameObject[] allSliders;
    float tempHorizontal;
    float tempVertical;

    bool sliderEngaged = false;
    bool sliderReady = false;
    float start2DEngageTime = 0;
    float currentTick;
    int nrButtonsEngaged = 0;

    bool buttonEngaged = false;
    bool buttonReady = false;
    float startbuttonEngageTime = 0;
    float tempFreq;
    float prevFreq;

    StringBuilder sb = new StringBuilder();

    public void Start()
    {
            updateTimestep = 1.0f / Settings.updateFrequency;

            currentSignal = new double[Settings.fingerColliders.Length];
            source = GetComponent<AudioSource>();

        if (Settings.storingData == true)
        {
            Settings.filePath = Settings.filePath + DateTime.Now.ToString("yyyy-mm-dd-hh-mm-ss") + ".txt";
            sb.Append("Time [s] , Frame nr");

            for (int i = 0; i < Settings.fingerColliders.Length; i++)
            {
                sb.Append(",");
                sb.Append($"Sent pressure {Settings.fingerColliders[i]}");
            }

            if (Settings.receivingData == true)
            {
                for (int i = 0; i < Settings.fingerColliders.Length; i++)
                {
                    sb.Append(",");
                    sb.Append($"Received pressure {Settings.fingerColliders[i]}");
                }
            }

            sb.AppendLine();
        }
    }

    public void Update()
    {
                
                update += Time.deltaTime;               //this controls the timing of the loop
                if (update > updateTimestep)
                {
                        for (int i = 0; i < Settings.fingerColliders.Length; i++)
                        {
                        //0 is standard, we set it to a value if a button is actually touched
                        tempSignal = 0;

                        //this routine gets activated when the buttons are touched
                        allButtons = GameObject.FindGameObjectsWithTag("ButtonHaptics");

                        nrButtonsEngaged = 0;

                        foreach (GameObject button in allButtons)
                        {
                            if (button.GetComponent<SendButtonData>().isEngaged)
                             {
                                nrButtonsEngaged++;
                                //print("touching buttons");
                                //Debug.Log($"Received finger of type {button.GetComponent<SendButtonData>().Finger}");
                                currentFinger = button.GetComponent<SendButtonData>().Finger;

                                if (currentFinger == Settings.fingerColliders[i])
                                {
                                    //Debug.Log($"We've found a match for {Settings.fingerColliders[i]}!");
                                    tempSignal = button.GetComponent<SendButtonData>().sendDataButton;

                                    if (Mathf.Abs(1 - tempSignal) < maxButtonTickDistance)
                                    {
                                        if (buttonReady == false)
                                        {
                                            buttonEngaged = true;
                                            startbuttonEngageTime = Time.realtimeSinceStartup;
                                            source.clip = buttonClip;
                                            source.loop = false;
                                            source.volume = 0.75f;
                                            source.Play();
                                            buttonReady = true;
                                        }
                                    }

                                    if (buttonEngaged == true)
                                    {
                                        if ((Time.realtimeSinceStartup - startbuttonEngageTime) < buttonTickTime)
                                        {
                                            tempSignal = buttonTickAmplitude;
                                        }
                                        else
                                        {
                                            buttonEngaged = false;
                                        }
                                    }

                                    if(button.name == "Cube UI Button (1)")
                                    {
                                        tempSignal = 0;
                                    }

                                    if (button.name == "Cube UI Button (4)")
                                    {
                                        tempSignal = 0;
                                    }

                                    if (button.name == "Cube UI Button (7)")
                                    {
                                        tempSignal = 0;
                                    }

                                }
                            }
                        }

                        if(nrButtonsEngaged == 0)
                        {
                            buttonReady = false;
                            buttonEngaged = false;
                        }

                        //this routine gets activated when the 2D slider is touched
                        all2DSliders = GameObject.FindGameObjectsWithTag("2DSliderHaptics");

                        foreach (GameObject TwoDSlider in all2DSliders)
                        {
                            if (TwoDSlider.GetComponent<Send2DSliderData>().isEngaged)
                            {
                                currentFinger = TwoDSlider.GetComponent<Send2DSliderData>().Finger;

                                if (currentFinger == Settings.fingerColliders[i])
                                {
                                    if (engageStarted == false)
                                    {
                                        startEngageTime = Time.realtimeSinceStartup;
                                        engageStarted = true;
                                    }
                                    
                                    //Debug.Log("touching 2D slider");

                                    currentTime = Time.realtimeSinceStartup - startEngageTime;
                                    tempHorizontal = TwoDSlider.GetComponent<Send2DSliderData>().sendData2DSliderHorizontal;
                                    tempVertical = TwoDSlider.GetComponent<Send2DSliderData>().sendData2DSliderVertical;

                                    if (tempVertical < 0.5f)
                                    {
                                        tempFreq = lowerTwoDFreqSlider;
                                    }
                                    else
                                    {
                                        tempFreq = upperTwoDFreqSlider;
                                    }

                                    if (!(prevFreq == tempFreq))
                                    {
                                        if (tempFreq == lowerTwoDFreqSlider)
                                        {
                                            currentClip = slider2DClipLow;
                                        }
                                        else
                                        {
                                            currentClip = slider2DClipHigh;
                                        }
                                        source.clip = currentClip;
                                        source.loop = true;
                                        source.Play();
                                    }

                                    //horizontal data scales the amplitude of the sine, the vertical data scales the frequency
                                    source.volume =  + 0.7f * tempHorizontal;
                                    prevFreq = tempFreq;
                                    tempSignal = 0.5f * slider2DMaxAmplitude + 0.5f * tempHorizontal * slider2DMaxAmplitude * Mathf.Sin(2 * Mathf.PI * tempFreq * currentTime);
                                }
                            }
                            else
                            {
                                engageStarted = false;
                                prevFreq = 0;
                                if (source.clip == currentClip && source.isPlaying)
                                {
                                    source.Stop();
                                }
                            }
                        }

                        //this routine gets activated when the 1D slider is touched
                        allSliders = GameObject.FindGameObjectsWithTag("SliderHaptics");

                        foreach (GameObject Slider in allSliders)
                        {
                            if (Slider.GetComponent<SendSliderData>().isEngaged)
                            {
                                currentFinger = Slider.GetComponent<SendSliderData>().Finger;

                                if (currentFinger == Settings.fingerColliders[i])
                                {
                                    //Debug.Log("touching 1D slider");
                                    tempSignal = (float)Slider.GetComponent<SendSliderData>().sendDataSlider;

                                    for (int f = 0; f < sliderTicks.Length; f++)
                                    {
                                        if (Mathf.Abs(tempSignal - sliderTicks[f]) < maxSliderTickDistance)
                                        {
                                            if (sliderReady == false)
                                            {
                                                sliderEngaged = true;
                                                start2DEngageTime = Time.realtimeSinceStartup;
                                                source.clip = sliderClip;
                                                source.loop = false;
                                                source.volume = 0.75f;
                                                source.Play();
                                                currentTick = sliderTicks[f];
                                                sliderReady = true;
                                            }
                                        }
                                    }

                                    if (sliderEngaged == true)
                                    {
                                        if ((Time.realtimeSinceStartup - start2DEngageTime) < sliderTickTime)
                                        {
                                            if (currentTick < 0.5)
                                            {
                                                tempSignal = currentTick + sliderTickAmplitude;
                                            }
                                            else
                                            {
                                                tempSignal = currentTick - sliderTickAmplitude;
                                            }
                                        }
                                        else
                                        {
                                            sliderReady = false;
                                            sliderEngaged = false;
                                        }
                                    }
                                }
                                
                            }
                        }

                        currentSignal[i] = (double)tempSignal;

                        }

                    if (Settings.dataToConsole)
                    {
                        for (int i = 0; i < Settings.fingerColliders.Length; i++)
                        {
                            Debug.Log($"Trying to send: {currentSignal[i]} for finger {Settings.fingerColliders[i]}");
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

                    SendInputToServer(currentSignal);
                    update = 0.0f;
                    counter++;

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