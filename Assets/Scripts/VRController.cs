using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.XR;

public class VRController : MonoBehaviour
{
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_rotationIncrement = 20f;

    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;
    public SteamVR_Action_Boolean m_RotPress = null;
    public SteamVR_Action_Vector2 m_RotValue = null;

    private float m_Speed = 0.0f;
    private float m_Orient;

    private Rigidbody m_RigidBody = null;
    private CapsuleCollider m_CapsuleCollider = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    public random_location m_RandManager;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
    }

    void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
        m_Orient = transform.eulerAngles.y;
        resetPos();
    }


    // Update is called once per frame
    void Update()
    {
        if (!m_RandManager.gameOver)
        {
            HandleHeight();
            CalculateMvmt();
            CalculateRot();
        }

        if (m_RandManager.gameOver && m_RandManager.curTrialNum < m_RandManager.numTrials && m_RandManager.counter == 0)
        {
            resetPos();
            m_RandManager.counter++;
        }
    }

    private void HandleHead()
    {
        // curr transform vals
        Vector3 oldPosition = m_CameraRig.position;
        Quaternion oldRotation = m_CameraRig.rotation;


        // handle rotation of head
        transform.eulerAngles = new Vector3(0.0f, m_Head.rotation.eulerAngles.y, 0.0f);


        // restore transform after rotation
        m_CameraRig.position = oldPosition;
        m_CameraRig.rotation = oldRotation;
    }

    // Movement using controllers.
    private void CalculateMvmt()
    {
        if (m_MoveValue == null)
        {
            return;
        }

        // find mvmt orientation
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        // not moving
        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            m_Speed = 0;
        }

        // if pressed
        if (m_MovePress.GetState(SteamVR_Input_Sources.RightHand))
        {
            // clamp
            m_Speed += m_MoveValue.axis.y * m_Sensitivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            // orientation
            movement += orientation * (m_Speed * Vector3.forward) * Time.deltaTime;
        }

        // apply mvmt
        m_RigidBody.MovePosition(transform.position + movement);
    }

    // Rotation using controllers.
    private void CalculateRot()
    {
        float snapValue = 0.0f;

        if (m_RotPress.GetState(SteamVR_Input_Sources.LeftHand) && (m_RotValue.axis.y <= 0.45 && m_RotValue.axis.y >= -0.6))
        {
            if (m_RotValue.axis.x <= 0)
            {
                snapValue = -Mathf.Abs(m_rotationIncrement);
            }
            else
            {
                snapValue = Mathf.Abs(m_rotationIncrement);
            }

            transform.RotateAround(m_Head.position, Vector3.up, snapValue * Time.deltaTime);
        }
    }

    // Change height of collider to match height of headset in real space.
    private void HandleHeight()
    {
        // get head in local space
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CapsuleCollider.height = headHeight;

        // cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CapsuleCollider.height / 2;

        // move capsule in local space
        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        // rotate
        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        // apply
        m_CapsuleCollider.center = m_Head.localPosition;
    }

    // Reset the position of the subject to the beginning of the course.
    public void resetPos()
    {
        float x = 22.0f;
        float y = 0.25f;
        float z = Random.Range(-2, 4);
        float rot = -85.273f;

        Vector3 pos = new Vector3(x, y, z);
        this.transform.position = pos;

        this.transform.rotation = Quaternion.Euler(new Vector3(0f, rot, 0f));
    }
}



