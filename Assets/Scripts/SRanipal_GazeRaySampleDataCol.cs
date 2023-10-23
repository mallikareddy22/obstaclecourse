//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections;



namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_GazeRaySampleDataCol : MonoBehaviour
            {
                public int LengthOfRay = 25;
                public string Condition;
                [SerializeField] public GameObject CenterEye;
                Camera camL, camR, camC;

                //[SerializeField] private LineRenderer GazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;

                // data to save
                public Vector3 screenPosC;
                public Vector3 screenPosL;
                public Vector3 screenPosR;
                public Vector3 worldPosC;
                public Vector3 worldPosL;
                public Vector3 worldPosR;
                public Vector3 GazeDirectionCombined;
                public Vector3 GazeOriginCombinedLocal;
                private Vector3 GazeDirectionCombinedLocal;
                private RaycastHit hitInfoC, hitInfoL, hitInfoR;
                public Vector3 L_Origin;
                public Vector3 R_Origin;
                public Vector3 L_Direction;
                public Vector3 R_Direction;
                private StreamWriter sw;
                private StreamWriter sw2;

                public GameObject desk;
                public Text text;
                public GameObject cameraRig;
                public Camera camera;
                public GameObject spawner;
                SpawnEquidistant spawnScript;
                GameObject container;
                int trial;
                int type;
                bool collectingData;

                public string monocular = ""; //L, R, or blank
                public string inverted = ""; //I or blank
                private Vector3 headPosition;

                private void Start()
                {
                    //camera.stereoSeparation = 0.5f;
                    //print(camera.stereoSeparation);
                    collectingData = false;
                    trial = 0;
                    if (Equals(monocular, "L"))
                    {
                        camera.stereoTargetEye = StereoTargetEyeMask.Left;
                    }
                    else if (Equals(monocular, "R")) {
                        camera.stereoTargetEye = StereoTargetEyeMask.Right;
                    }
                    spawnScript = spawner.GetComponent<SpawnEquidistant>();
                    container = spawnScript.getContainer();
                    type = spawnScript.getType();

                    if (Equals(inverted, "I")) {
                        StartCoroutine(WaitHeadPos());
                    }

                    sw = File.AppendText(Condition + "_" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + "GazeDataAll.txt");

                    string textAll = "Trial" + ", " + "Time" + ", " + "Type" + ", " + "L Pixel X " + " , " + "L Pixel Y " + " , " + "L Pixel Z" +
                    ", " + "L hit point in Z" + " , " + "L Looking At" + ", " + "L World X" + ", " + "L World Y" + ", " + "R Pixel X " + ", " + "R Pixel Y " + " , " + "R Pixel Z" +
                    ", " + "R hit point in Z" + " , " + "R Looking At" + ", " + "R World X" + ", " + "R World Y" + ", " + "C Pixel X " + ", " + "C Pixel Y " + " , " + "C Pixel Z" +
                    ", " + "C hit point in Z" + " , " + "C Looking At" + ", " + "C World X" + ", " + "C World Y" + ", " + "L Origin X " + ", " + "L Origin Y " + " , " + "L Origin Z" +
                    ", " + "R Origin X " + " , " + "R Origin Y " + " , " + "R Origin Z" + "\n";
                    sw.Write(textAll);

                    sw2 = File.AppendText(Condition + "_" + System.DateTime.Now.ToString("MM-dd-yyyy") + "_" + "ObjectPositions.txt");
                    //newTrial();
                    /*string textAll2 = "Trial, Time, Type, Target, ";
                    if (container != null)
                    {
                        foreach (Transform child in container.transform)
                        {
                            textAll2 += child.gameObject.name + " asdfPos X , Pos Y, Pos Z, Rot X, Rot Y, Rot Z, ";
                        } //Whenever next trial starts, need to reprint these headers b/c diff objects or print name of obj each time
                    }
                    textAll2 += "\n";
                    sw2.Write(textAll2);
                    sw2.Write("hello" + trial);*/

                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    //Assert.IsNotNull(GazeRayRenderer);

                    camC = CenterEye.GetComponent<Camera>();


                }

                private IEnumerator WaitHeadPos()
                {
                    yield return new WaitForSeconds(1);
                    InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
                    device.TryGetFeatureValue(CommonUsages.devicePosition, out headPosition);
                    Debug.Log(headPosition.y);
                    //desk.transform.position.y = headPosition.y + 
                    //cameraRig.transform.position = new Vector3(-1, 4.92f, -1);
                    cameraRig.transform.position = new Vector3(-1, headPosition.y + 3.17f, -1);
                    //0,4.92,1.75
                    cameraRig.transform.localRotation = Quaternion.Euler(180, 35, 0);
                    text.transform.localRotation = Quaternion.Euler(0, 90, 180);
                }

                private void Update()
                {

                    //if (trial < spawnScript.getTrialNumber()) {}
                    if (collectingData)
                    {
                        GetGazeData();
                    }
                    //GazeRayRenderer.SetPosition(0, camC.transform.position - camC.transform.up * 0.05f);
                    //GazeRayRenderer.SetPosition(1, hitInfoC.point);

                }

                public void newTrial()
                {
                    /*Debug.Log("Trial: " + trial);
                    if (spawnScript != null && trial != 0)
                    {
                        container = spawnScript.getContainer();
                        sw2.Write("\n\n");
                        string textAll2 = trial + "Trial, Time, Type, Target, ";
                        foreach (Transform child in container.transform)
                        {
                            textAll2 += child.gameObject.name + " Pos X , Pos Y, Pos Z, Rot X, Rot Y, Rot Z, ";
                        } //Whenever next trial starts, need to reprint these headers b/c diff objects or print name of obj each time
                        textAll2 += "\n";
                        sw2.Write(textAll2);
                        Debug.Log(textAll2);
                        trial++;
                        type = spawnScript.getType();
                        //saveObjectData(target);
                    }*/
                }

                public void saveObjectData() {
                    //newTrial();
                    spawnScript.setWallsInactive();
                    container = spawnScript.getContainer();
                    if (trial == 0)
                        trial = 1;
                    /*if (trial == 0)
                    {
                        Debug.Log("huh?");
                        string textAll = "Trial, Time, Type, Target, ";
                        foreach (Transform child in container.transform)
                        {
                            textAll += child.gameObject.name + " Pos X , Pos Y, Pos Z, Rot X, Rot Y, Rot Z, ";
                        } //Whenever next trial starts, need to reprint these headers b/c diff objects or print name of obj each time
                        textAll += "\n";
                        sw2.Write(textAll);
                        Debug.Log(textAll);
                        trial += 1;
                        //newTrial();
                    }*/
                    string textAll = "Trial, Time, Type, Target, ";
                    foreach (Transform child in container.transform)
                    {
                        textAll += child.gameObject.name + " Pos X , Pos Y, Pos Z, Rot X, Rot Y, Rot Z, ";
                    } //Whenever next trial starts, need to reprint these headers b/c diff objects or print name of obj each time
                    textAll += "\n";
                    sw2.Write(textAll);
                    string textAll2 = trial + ", " + System.DateTime.Now.Ticks.ToString() + ", " + type + ", " + spawnScript.getTargetName() + ", ";
                    try
                    {
                        foreach (Transform child in container.transform)
                        {
                            if (child != null)
                            {
                                textAll2 += child.position.x + ", " + child.position.y + ", " + child.position.z + ", " + child.rotation.eulerAngles.x + ", " + child.rotation.eulerAngles.y + ", " + child.rotation.eulerAngles.z + ", ";
                            }
                            else
                            {
                                Debug.Log("child null");
                                textAll2 += "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A";
                            }
                        }
                    }
                    catch
                    {
                        Debug.Log("container null");
                        textAll2 += "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A" + ", " + "N/A";
                    }
                    textAll2 += "\n\n";
                    sw2.Write(textAll2);
                    trial += 1;
                }

                public void startCollectingData()
                {
                    collectingData = true;
                }

                public void stopCollectingData()
                {
                    collectingData = false;
                }

                private void GetGazeData()
                {
                    //await Task.Delay(TimeSpan.FromSeconds(0));


                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }

                    if (eye_callback_registered)
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal, eyeData))
                        {
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out L_Origin, out L_Direction, eyeData);
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out R_Origin, out R_Direction, eyeData);

                        }
                        else
                        {
                            return;
                        }

                    }

                    else
                    {
                        if (SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out GazeOriginCombinedLocal, out GazeDirectionCombinedLocal))
                        {
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out L_Origin, out L_Direction);
                            SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out R_Origin, out R_Direction);
                        }
                        else
                        {
                            return;
                        }
                    }

                    string lObjectName = "N/A";
                    string rObjectName = "N/A";
                    string cObjectName = "N/A";

                    camL = camC;
                    camL.transform.position += new Vector3(-0.03f, 0, 0);
                    Vector3 GazeDirectionTestL = camL.transform.TransformDirection(L_Direction);
                    if (Physics.Raycast(camL.transform.position, GazeDirectionTestL, out hitInfoL, 1000))
                    {
                        lObjectName = hitInfoL.collider.gameObject.name;
                        screenPosL = camL.WorldToScreenPoint(hitInfoL.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosL = hitInfoL.point;
                        screenPosL.z = hitInfoL.point.z;
                    }

                    camR = camC;
                    camR.transform.position += new Vector3(0.03f, 0, 0);
                    Vector3 GazeDirectionTestR = camR.transform.TransformDirection(R_Direction);
                    if (Physics.Raycast(camR.transform.position, GazeDirectionTestR, out hitInfoR, 1000))
                    {
                        rObjectName = hitInfoR.collider.gameObject.name;
                        screenPosR = camR.WorldToScreenPoint(hitInfoR.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosR = hitInfoR.point;
                        screenPosR.z = hitInfoR.point.z;
                    }

                    Vector3 GazeDirectionTestC = camC.transform.TransformDirection(GazeDirectionCombinedLocal);
                    if (Physics.Raycast(camC.transform.position, GazeDirectionTestC, out hitInfoC, 1000))
                    {
                        cObjectName = hitInfoC.collider.gameObject.name;
                        screenPosC = camC.WorldToScreenPoint(hitInfoC.point, Camera.MonoOrStereoscopicEye.Mono);
                        worldPosC = hitInfoC.point;
                        screenPosC.z = hitInfoC.point.z;
                    }

                    string textAll = trial + ", " + System.DateTime.Now.Ticks.ToString() + ", " + type + ", " + screenPosL.x + ", " + screenPosL.y + ", " + screenPosL.z +
                    ", " + hitInfoL.distance + ", " + lObjectName + ", " + worldPosL.x + ", " + worldPosL.y + ", " + screenPosR.x + ", " + screenPosR.y + ", " + screenPosR.z +
                    ", " + hitInfoR.distance + ", " + rObjectName + ", " + worldPosR.x + ", " + worldPosR.y + ", " + screenPosC.x + ", " + screenPosC.y + ", " + screenPosC.z +
                    ", " + hitInfoC.distance + ", " + cObjectName + ", " + worldPosC.x + ", " + worldPosC.y + ", " + L_Origin.x + ", " + L_Origin.y + ", " + L_Origin.z +
                     ", " + R_Origin.x + ", " + R_Origin.y + ", " + R_Origin.z + "\n";
                    
                    sw.Write(textAll);

                    
                }

                private void OnDisable()
                {
                    Release();
                }

                void OnApplicationQuit()
                {
                    Release();
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                    }
                }

                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;



                }
            }
        }
    }
}
