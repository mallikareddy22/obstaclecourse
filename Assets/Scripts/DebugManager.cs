using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

/** For Editor and Debugging Purposes
 *  Performs initial checks on connected devices.
 *  Allows switches between desired demo mode.
 */
public class DebugManager : MonoBehaviour
{

    public GameObject CubePlayer;
    public GameObject CubeCamera;
    public GameObject CimCam;
    public GameObject VRPlayer;
    public GameObject SimVRPlayer;
    public GameObject SimTool;

    public bool isVR;
    public bool isSim;

    private void Awake()
    {
        if (isVR)
        {
            CubePlayer.SetActive(false);
            CubeCamera.SetActive(false);
            CimCam.SetActive(false);
            VRPlayer.SetActive(true);
        }
        else
        {
            CubePlayer.SetActive(true);
            CubeCamera.SetActive(true);
            CimCam.SetActive(true);
            VRPlayer.SetActive(false);
        }

        if (isSim)
        {
            SimVRPlayer.SetActive(true);
            SimTool.SetActive(true);
            VRPlayer.SetActive(false);
            CubePlayer.SetActive(false);
            CubeCamera.SetActive(false);
            CimCam.SetActive(false);
        }
        else
        {
            if (!isVR)
            {
                XRSettings.LoadDeviceByName("");
                SimVRPlayer.SetActive(false);
                SimTool.SetActive(false);
            }
        }
    }

    public void StartExpt()
    {
        if (isSim)
        {
            SimVRPlayer.GetComponent<random_location>().SetupInfo();
        }
        else if (isVR)
        {
            VRPlayer.GetComponent<random_location>().SetupInfo();
        }
        else
        {
            Debug.Log("Non-VR version is unsupported! Please use the Simulator or VR version.");
            CubePlayer.GetComponent<random_location>().SetupInfo();
        }
    }
}
