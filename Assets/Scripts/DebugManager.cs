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
        }
        else
        {
            XRSettings.LoadDeviceByName("");
            SimVRPlayer.SetActive(false);
            SimTool.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
