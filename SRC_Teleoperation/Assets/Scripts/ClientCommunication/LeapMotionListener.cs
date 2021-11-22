using System.Linq;
using UnityEngine;
using Leap;

public class LeapMotionListener
{
    Leap.Vector metaCarpal;
    Leap.Vector proximal;
    Leap.Vector intermediate;
    Leap.Vector distal;
    public static float[] AllDirections;
    public static bool rightHandEnabled = false;
    //Please note that the MaxAngle settings could be specific to the user, so you might need to tweak these for yourself.
    //MaxAngle represent the max values for a closed fist, from thumb to pinky finger.
    public static float[] MaxAngle = { 0.5f, 1.2f, 1.2f, 1.2f, 1.0f };
    //AverageAngle represents a ratio to MaxAngle, and is limited in code to the range 0-1.
    public static float[] AverageAngle;
    float Angle_temp;

    public void OnConnect(object sender, DeviceEventArgs args)
    {
        Debug.Log("Connected");
    }

    public void OnFrame(object sender, FrameEventArgs args)
    {
        // Get the most recent frame and report some basic information
        Frame frame = args.frame;

        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                
            rightHandEnabled = true;

                for (int i = 0; i < Settings.fingerAngleIndex.Length; i++)
                {
                    //This piece is custom, and should be changed to achieve anything else than taking the average angle across all phalanges
                    metaCarpal = hand.Fingers[Settings.fingerAngleIndex[i]].Bone(Bone.BoneType.TYPE_METACARPAL).Direction;
                    proximal = hand.Fingers[Settings.fingerAngleIndex[i]].Bone(Bone.BoneType.TYPE_PROXIMAL).Direction;
                    intermediate = hand.Fingers[Settings.fingerAngleIndex[i]].Bone(Bone.BoneType.TYPE_INTERMEDIATE).Direction;
                    distal = hand.Fingers[Settings.fingerAngleIndex[i]].Bone(Bone.BoneType.TYPE_DISTAL).Direction;

                    AllDirections[0] = metaCarpal.AngleTo(proximal);
                    AllDirections[1] = proximal.AngleTo(intermediate);
                    AllDirections[2] = intermediate.AngleTo(distal);
                    
                    Angle_temp = AllDirections.Average();
                    Angle_temp /= MaxAngle[Settings.fingerAngleIndex[i]];
                    
                    //to make sure that the commanded pressures never exceed 1 (even if the real angle > max angle)
                    if(Angle_temp > 1.0f)
                    {
                        Angle_temp = 1.0f;
                    }

                    //because angles never really reach 0, not even for a fully open hand
                    if (Angle_temp < 0.2f)
                    {
                        Angle_temp = 0.0f;
                    }

                    AverageAngle[i] = Angle_temp;
                }
            }
            else
            {
                rightHandEnabled = false;
            }

            }

    }
}
