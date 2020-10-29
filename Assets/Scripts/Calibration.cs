﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;



public class Calibration : MonoBehaviour
{
    // openzen declaration
    ZenClientHandle_t mZenHandle = new ZenClientHandle_t();
    ZenSensorHandle_t mSensorHandle = new ZenSensorHandle_t();

    // 2nd IMU
    ZenSensorHandle_t mSensorHandle2 = new ZenSensorHandle_t();
    public GameObject IMU29, IMU98;
    [Tooltip("IO Type which OpenZen should use to connect to the sensor.")]
    public string OpenZenIoType = "Bluetooth";
    [Tooltip("Idenfier which is used to connect to the sensor. The name depends on the IO type used and the configuration of the sensor.")]
    public string OpenZenIdentifier = "00:04:3E:53:E9:29";
    public string OpenZenIdentifier2 = "00:04:3E:53:E9:98";

    // Humanoid declaration
    public GameObject Human;
    private GameObject[] HumanjointList = new GameObject[4];
    public GameObject mousepad;
    public GameObject Palm;
    public GameObject Palm_demo;
    public GameObject bottom_right;
    private GameObject[] calibrationposts = new GameObject[4];
    public Camera cam1;
    public Camera cam2;
    private float[] Tpose = { -14.996f, 0.547f, 1.458f, 0f, 5.685f, 1.042f, -4.332f };
    private float[] Initpose = { 64.42f, 0.547f, 1.458f, 82f, 80.71f, 1.042f, -4.332f };
    private Quaternion shoulderoffset;
    private Quaternion elbowoffset;
    private Quaternion wristoffset;
    // Start is called before the first frame update
    void Start()
    {
        Vector3 offset = new Vector3(-0.1869933f, -1.436951f, 0.05179569f);
        cam1.enabled = true;
        cam2.enabled = false;
        Palm_demo.SetActive(false);
        IMU98.SetActive(false);
        initializeposts();
        initializeJoints();
        initializeTpose();
        float dis = Vector3.Distance(calibrationposts[0].transform.position, calibrationposts[1].transform.position);
        float dis_short = Vector3.Distance(calibrationposts[1].transform.position, calibrationposts[2].transform.position);
        float dia_dis = Vector3.Distance(calibrationposts[0].transform.position, calibrationposts[2].transform.position);
        Debug.Log(dis);
        Debug.Log(dis_short);
        Debug.Log(dia_dis);

        //Debug.Log(HumanjointList[1].transform.position.x+", "+HumanjointList[1].transform.position.y + ", " + HumanjointList[1].transform.position.z);
        // create OpenZen
        OpenZen.ZenInit(mZenHandle);
        // Hint: to get the io type and identifer for all connected sensor,
        // you cant start the DiscoverSensorScene. The information of all 
        // found sensors is printed in the debug console of Unity after
        // the search is complete.

        print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
            " with sensor name " + OpenZenIdentifier);

        var sensorInitError = OpenZen.ZenObtainSensorByName(mZenHandle,
            OpenZenIoType,
            OpenZenIdentifier,
            0,
            mSensorHandle);


        if (sensorInitError != ZenSensorInitError.ZenSensorInitError_None)
        {
            print("Error while connecting to sensor 29.");
        }
        else
        {
            ZenComponentHandle_t mComponent = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle, OpenZen.g_zenSensorType_Imu, 0, mComponent);

            // enable sensor streaming, normally on by default anyways
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_StreamData, true);

            // set offset mode to heading 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);

            // set the sampling rate to 100 Hz
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_SamplingRate, 100);

            // filter mode using accelerometer & gyroscope & magnetometer
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_FilterMode, 1);

            // Ensure the Orientation data is streamed out
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle, mComponent,
               (int)EZenImuProperty.ZenImuProperty_OutputQuat, true);

            print("Sensor 29 configuration complete");
        }


        print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
           " with sensor name " + OpenZenIdentifier2);

        var sensorInitError2 = OpenZen.ZenObtainSensorByName(mZenHandle,
            OpenZenIoType,
            OpenZenIdentifier2,
            0,
            mSensorHandle2);

        if (sensorInitError2 != ZenSensorInitError.ZenSensorInitError_None)
        {
            print("Error while connecting to sensor 98.");
        }
        else
        {
            ZenComponentHandle_t mComponent2 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle2, OpenZen.g_zenSensorType_Imu, 0, mComponent2);

            // enable sensor streaming, normally on by default anyways
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_StreamData, true);

            // set offset mode to heading 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);

            // set the sampling rate to 100 Hz
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_SamplingRate, 100);

            // filter mode using accelerometer & gyroscope & magnetometer
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_FilterMode, 1);

            // Ensure the Orientation data is streamed out
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle2, mComponent2,
               (int)EZenImuProperty.ZenImuProperty_OutputQuat, true);

            print("Sensor 98 configuration complete");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleVisibility();
        }
        // run as long as there are new OpenZen events to process
        while (true)
        {
            ZenEvent zenEvent = new ZenEvent();

            // read all events which are waiting for us
            // use the rotation from the newest IMU event
            if (!OpenZen.ZenPollNextEvent(mZenHandle, zenEvent))
                break;

            // if compontent handle = 0, this is a OpenZen wide event,
            // like sensor search
            if (zenEvent.component.handle != 0)
            {
                if (zenEvent.sensor.handle == mSensorHandle.handle)
                {
                    switch (zenEvent.eventType)
                    {
                        case (int)ZenImuEvent.ZenImuEvent_Sample:
                            // read quaternion
                            OpenZenFloatArray fq = OpenZenFloatArray.frompointer(zenEvent.data.imuData.q);
                            // Unity Quaternion constructor has order x,y,z,w
                            // Furthermore, y and z axis need to be flipped to 
                            // convert between the LPMS and Unity coordinate system
                            Quaternion sensorOrientation = new Quaternion(fq.getitem(1),
                                                                        fq.getitem(3),
                                                                        fq.getitem(2),
                                                                        fq.getitem(0));

                            IMU29.transform.rotation = sensorOrientation;
                            //Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.up, Color.green); //y
                            //Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.right, Color.red); //x
                            //Debug.DrawRay(IMU29.transform.position, sensorOrientation * Vector3.forward, Color.blue); //z
                            HumanjointList[1].transform.rotation = sensorOrientation * shoulderoffset;
                            //Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.up, Color.green); //y
                            //Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.right, Color.red); //x
                            //Debug.DrawRay(HumanjointList[1].transform.position, HumanjointList[1].transform.rotation * Vector3.forward, Color.blue); //z
                            //Debug.DrawRay(HumanjointList[0].transform.position, Vector3.up, Color.green); //y
                            //Debug.DrawRay(HumanjointList[0].transform.position, Vector3.right, Color.red); //x
                            //Debug.DrawRay(HumanjointList[0].transform.position, Vector3.forward, Color.blue); //z


                            break;
                    }
                }
                if (zenEvent.sensor.handle == mSensorHandle2.handle)
                {
                    switch (zenEvent.eventType)
                    {
                        case (int)ZenImuEvent.ZenImuEvent_Sample:
                            // read quaternion
                            OpenZenFloatArray fq = OpenZenFloatArray.frompointer(zenEvent.data.imuData.q);
                            // Unity Quaternion constructor has order x,y,z,w
                            // Furthermore, y and z axis need to be flipped to 
                            // convert between the LPMS and Unity coordinate system
                            Quaternion sensorOrientation = new Quaternion(fq.getitem(1),
                                                                        fq.getitem(3),
                                                                        fq.getitem(2),
                                                                        fq.getitem(0));
                            IMU98.transform.rotation = sensorOrientation;
                            //Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.up, Color.green); //y
                            //Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.right, Color.red); //x
                            //Debug.DrawRay(IMU98.transform.position, sensorOrientation * Vector3.forward, Color.blue); //z

                            // Rotate elbow joint
                            HumanjointList[2].transform.localRotation = Quaternion.AngleAxis(IMU98.transform.localEulerAngles.y, -Vector3.forward);
                            Debug.Log("Elbow angle:");
                            Debug.Log(IMU98.transform.localEulerAngles.y);

                            // Rotate forearm supination/pronation joint
                            HumanjointList[3].transform.localRotation = Quaternion.AngleAxis(IMU98.transform.localEulerAngles.x, Vector3.up);
                            Debug.Log("Wrist Rotation:");
                            Debug.Log(IMU98.transform.localEulerAngles.x);



                            break;
                    }
                }

            }
            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 scale2 = new Vector3(1, 1, 1);
            Vector3 trans0 = HumanjointList[1].transform.position;
            Vector3 trans1 = HumanjointList[2].transform.localPosition;
            Vector3 trans2 = HumanjointList[3].transform.localPosition;
            Matrix4x4 TrueShoulder = HumanjointList[1].transform.localToWorldMatrix;
            Matrix4x4 World2Shoulder = Matrix4x4.TRS(trans0, HumanjointList[1].transform.rotation, scale);
            Matrix4x4 Shoulder2Elbow = Matrix4x4.TRS(trans1, HumanjointList[2].transform.localRotation, scale2);
            Matrix4x4 Elbow2Wrist = Matrix4x4.TRS(trans2, HumanjointList[3].transform.localRotation, scale2);

            Vector3 Wrist2Palm = new Vector3(0f, 0.058f, 0.004f);

            Matrix4x4 FK = World2Shoulder * Shoulder2Elbow * Elbow2Wrist;
            Vector3 FKpos = FK.MultiplyPoint3x4(Wrist2Palm);
            Palm_demo.transform.position = FKpos;
            

            if (Input.GetKeyDown(KeyCode.R))
            {
                print("Record Transformation Matrices: ");
                print(World2Shoulder);
                print(Shoulder2Elbow);
                print(Elbow2Wrist);
            }
        }
    }

    void initializeposts()
    {
        var mouseChildren = mousepad.GetComponentsInChildren<Transform>();
        for (int i = 0; i < mouseChildren.Length; i++)
        {
            if (mouseChildren[i].name == "Bottom_left")
            {
                calibrationposts[0] = mouseChildren[i].gameObject;
            }
            else if (mouseChildren[i].name == "Bottom_right")
            {
                calibrationposts[1] = mouseChildren[i].gameObject;
            }
            else if (mouseChildren[i].name == "Top_right")
            {
                calibrationposts[2] = mouseChildren[i].gameObject;
            }
            else if (mouseChildren[i].name == "Top_left")
            {
                calibrationposts[3] = mouseChildren[i].gameObject;
            }
        }
    }
    void initializeJoints()
    {
        var HumanChildren = Human.GetComponentsInChildren<Transform>();
        for (int i = 0; i < HumanChildren.Length; i++)
        {
            if (HumanChildren[i].name == "mixamorig1:RightShoulder")
            {
                HumanjointList[0] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightArm")
            {
                HumanjointList[1] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightForeArm")
            {
                HumanjointList[2] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "mixamorig1:RightHand")
            {
                HumanjointList[3] = HumanChildren[i].gameObject;
            }
        }
    }
    void initializeTpose()
    {
        Vector3 ShoulderRotation = HumanjointList[1].transform.localEulerAngles;
        Vector3 ElbowRotation = HumanjointList[2].transform.localEulerAngles;
        Vector3 WristRotation = HumanjointList[3].transform.localEulerAngles;

        ShoulderRotation.x = Tpose[0];
        ShoulderRotation.y = Tpose[1];
        ShoulderRotation.z = Tpose[2];
        HumanjointList[1].transform.localEulerAngles = ShoulderRotation;

        ElbowRotation.z = Tpose[3];
        HumanjointList[2].transform.localEulerAngles = ElbowRotation;

        WristRotation.y = Tpose[4];
        WristRotation.x = Tpose[5];
        WristRotation.z = Tpose[6];
        HumanjointList[3].transform.localEulerAngles = WristRotation;

        shoulderoffset = HumanjointList[1].transform.rotation;
        elbowoffset = HumanjointList[2].transform.rotation;
        wristoffset = HumanjointList[3].transform.rotation;
    }
    void initializeHumanJoints()
    {
        Vector3 ShoulderRotation = HumanjointList[1].transform.localEulerAngles;
        Vector3 ElbowRotation = HumanjointList[2].transform.localEulerAngles;
        Vector3 WristRotation = HumanjointList[3].transform.localEulerAngles;

        ShoulderRotation.x = Initpose[0];
        ShoulderRotation.y = Initpose[1];
        ShoulderRotation.z = Initpose[2];
        HumanjointList[1].transform.localEulerAngles = ShoulderRotation;

        ElbowRotation.z = Initpose[3];
        HumanjointList[2].transform.localEulerAngles = ElbowRotation;

        WristRotation.y = Initpose[4];
        WristRotation.x = Initpose[5];
        WristRotation.z = Initpose[6];
        HumanjointList[3].transform.localEulerAngles = WristRotation;

        shoulderoffset = HumanjointList[1].transform.rotation;
        elbowoffset = HumanjointList[2].transform.rotation;
        wristoffset = HumanjointList[3].transform.rotation;
    }

    void OnGUI()
    {
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;
        if (GUI.Button(new Rect(Screen.width - 170, 20, 150, 30), "IMU29 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU29");
            ZenComponentHandle_t mComponent = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle, OpenZen.g_zenSensorType_Imu, 0, mComponent);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

        if (GUI.Button(new Rect(Screen.width - 170, 60, 150, 30), "IMU98 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU98");
            ZenComponentHandle_t mComponent2 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle2, OpenZen.g_zenSensorType_Imu, 0, mComponent2);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }
        //if (GUI.Button(new Rect(Screen.width - 120, 90, 100, 30), "Initial position", myButtonStyle))
        //{
        //    initializeHumanJoints();
        //}
        //if (GUI.Button(new Rect(Screen.width - 120, 120, 100, 30), "T pose", myButtonStyle))
        //{
        //    initializeTpose();
        //}

        if (GUI.Button(new Rect(20, 20, 150, 30), "Calibrate Mousepad", myButtonStyle))
        {
            print("Record palm position");
            Calibrate();
        }
    }

    void ToggleVisibility()
    {
        IMU29.SetActive(!IMU29.activeSelf);
        //IMU98.SetActive(!IMU98.activeSelf);
    }

    void OnDestroy()
    {
        if (mSensorHandle != null)
        {
            OpenZen.ZenReleaseSensor(mZenHandle, mSensorHandle);
        }
        OpenZen.ZenShutdown(mZenHandle);
    }

    void Calibrate()
    {
        Vector3 pos = Palm.transform.position;
        //print(pos);
        Vector3 postpos = bottom_right.transform.position;
        Vector3 offset = postpos - pos;
        Vector3 currentpos = mousepad.transform.position;
        mousepad.transform.position = currentpos - offset;
        Vector3 camera = mousepad.transform.position;
        Vector3 camera2 = new Vector3(camera.x, camera.y+10f, camera.z);
        cam2.transform.position = camera2;

        Human.SetActive(!Human.activeSelf);
        ToggleVisibility();
        Palm_demo.SetActive(true);

        //print(FK.MultiplyPoint3x4(Wrist2Palm));
        //print("True position is: ");
        //print(postpos);


    }
}
