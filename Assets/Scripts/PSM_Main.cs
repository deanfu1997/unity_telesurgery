using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class PSM_Main : MonoBehaviour
{
    // openzen declaration
    ZenClientHandle_t mZenHandle = new ZenClientHandle_t();
    ZenSensorHandle_t mSensorHandle = new ZenSensorHandle_t();

    // 2nd IMU
    ZenSensorHandle_t mSensorHandle2 = new ZenSensorHandle_t();

    // 3rd IMU
    ZenSensorHandle_t mSensorHandle3 = new ZenSensorHandle_t();

    public enum OpenZenIoTypes { SiUsb, Bluetooth };

    public GameObject IMU29, IMU98, IMUusb;
    [Tooltip("IO Type which OpenZen should use to connect to the sensor.")]
    public string OpenZenIoType = "Bluetooth";
    public OpenZenIoTypes OpenZenIoType2 = OpenZenIoTypes.SiUsb;
    [Tooltip("Idenfier which is used to connect to the sensor. The name depends on the IO type used and the configuration of the sensor.")]
    public string OpenZenIdentifier = "00:04:3E:53:E9:29";
    public string OpenZenIdentifier2 = "00:04:3E:53:E9:98";
    public string OpenZenIdentifier3 = "lpmsto2000985";
    private string filepath_origin = "Assets/data2/Dean_MTM.txt";
    private string filepath = "Assets/data2/Dean_MTM.txt";
    private string calibrationfile = "Assets/data/calibration/Calibration.txt";
    // Humanoid declaration
    public GameObject Human;
    private GameObject[] HumanjointList = new GameObject[4];
    public GameObject Hand_demo;
    List<GameObject> currentObjects;
    // public string jsonFilePath = "Assets/Scripts/data2.json";
    public float lu = 0.3f;
    public float lf = 0.25f;
    public float lh = 0.2f;

    public GameObject Left_Anchor;
    public GameObject Right_Anchor;

    private bool isIMU = false;
    private bool isUDP = false;
    private Vector3 hand_orientation;
    private Vector3 spheretrans = new Vector3(0.125f, 0f, 0f);
    private Vector3 spheretrans2 = new Vector3(0.05f, 0f, 0f);
    private Vector3 x0;
    private Vector3 x1;
    private Vector3 s;
    private float radius_halfcircle = 0.05f;
    private Quaternion q;
    private float timer = 0.0f;
    private float dist = -1.0f;
    private float visualTime = 0.0f;
    private int fileCounter = 1;
    private bool generateSphere;
    private Vector3 last_pos;
    private Quaternion last_rot;
    private Vector3 last_FK;
    private Quaternion last_FKrot = Quaternion.identity;
    private int gripper_min;
    private int gripper_max;
    private int gripper_min_now;
    private int gripper_max_now;

    // UDP Stuff here
    private bool sendmsg = false;
    private string sendIp = "10.194.20.182";
    // private int sendPort = 48055;
    private int sendPort = 2345;
    private int receivePort = 48056;

    private UdpConnection connection;
    // Start is called before the first frame update
    void Start()
    {
        // connection = new UdpConnection();
        // connection.StartConnection(sendIp, sendPort, receivePort);

        Hand_demo.SetActive(!Hand_demo.activeSelf);
        initializeJoints();


        // create OpenZen
        OpenZen.ZenInit(mZenHandle);
        // Hint: to get the io type and identifer for all connected sensor,
        // you cant start the DiscoverSensorScene. The information of all 
        // found sensors is printed in the debug console of Unity after
        // the search is complete.

        // print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
        //    " with sensor name " + OpenZenIdentifier);


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


        // print("Trying to connect to OpenZen Sensor on IO " + OpenZenIoType +
        //  " with sensor name " + OpenZenIdentifier2);

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

        var sensorInitError3 = OpenZen.ZenObtainSensorByName(mZenHandle,
            OpenZenIoType2.ToString(),
            OpenZenIdentifier3,
            0,
            mSensorHandle3);

        if (sensorInitError3 != ZenSensorInitError.ZenSensorInitError_None)
        {
            print("Error while connecting to sensor USB.");
        }
        else
        {
            ZenComponentHandle_t mComponent3 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle3, OpenZen.g_zenSensorType_Imu, 0, mComponent3);

            // enable sensor streaming, normally on by default anyways
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle3, mComponent3,
               (int)EZenImuProperty.ZenImuProperty_StreamData, true);

            // set offset mode to heading 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle3, mComponent3,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);

            // set the sampling rate to 100 Hz
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle3, mComponent3,
               (int)EZenImuProperty.ZenImuProperty_SamplingRate, 100);

            // filter mode using accelerometer & gyroscope & magnetometer
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle3, mComponent3,
               (int)EZenImuProperty.ZenImuProperty_FilterMode, 1);

            // Ensure the Orientation data is streamed out
            OpenZen.ZenSensorComponentSetBoolProperty(mZenHandle, mSensorHandle3, mComponent3,
               (int)EZenImuProperty.ZenImuProperty_OutputQuat, true);

            print("Sensor USB configuration complete");
        }

        // Gripper calibration here:
        Debug.Log("Begin Gripper Calibration");
    }

    // Update is called once per frame
    void Update()
    {

        // assign q and x0 to be hand's orentation and position
        q = Hand_demo.transform.rotation;
        x0 = Hand_demo.transform.position;
        Debug.DrawRay(x0, q * Vector3.up, Color.green);
        hand_orientation = q * Vector3.up;

        currentObjects = Counter.currentWires;
        // check if there's collision with a wire
        if (currentObjects != null)
        {
            if (currentObjects.Count != 0)
            {
                float dist_closest = Mathf.Infinity;
                Vector3 hc_orientation = Vector3.zero;
                Vector3 orientation_closest = Vector3.zero;
                float angle_closest = Mathf.Infinity;
                float angle = angle_closest;
                foreach (var currentObject in currentObjects)
                {
                    // Debug.Log(currentObject.name);
                    if (currentObject.name.Contains("curve")){
                        if (currentObject.name.Contains("curve1"))
                        {
                            Vector3 s_offset = new Vector3(0.0125f + 0.05f, 0f, 0.0125f);
                            Vector3 center = currentObject.transform.position + s_offset;
                            dist = distanceTocenter(x0, center, radius_halfcircle);
                            hc_orientation = x0 - center ;
                            Debug.DrawRay(center, hc_orientation, Color.red);
                            angle = Vector3.Angle(hc_orientation, hand_orientation);
                            angle -= 90;
                            angle = Mathf.Abs(angle);
                            // Debug.Log(angle);


                        }
                        else if (currentObject.name.Contains("curve2")){
                            Vector3 s2_offset = new Vector3(-0.0125f - 0.05f, 0f, 0.0125f);
                            Vector3 center = currentObject.transform.position + s2_offset;
                            dist = distanceTocenter(x0, center, radius_halfcircle);
                            hc_orientation = x0 - center;
                            Debug.DrawRay(center, hc_orientation, Color.red);
                            angle = Vector3.Angle(hc_orientation, hand_orientation);
                            angle -= 90;
                            angle = Mathf.Abs(angle);
                            // Debug.Log(angle);
                        }
                    }

                    else
                    {
                        Vector3 wire_orientation = currentObject.transform.rotation * Vector3.up;
                        Debug.DrawRay(currentObject.transform.position, wire_orientation, Color.red);

                        x1 = currentObject.transform.position;
                        s = wire_orientation;
                        dist = distanceToclosestPoint(x0, x1, s);
                        angle = Vector3.Angle(wire_orientation, hand_orientation);
                    }
                    if (dist < dist_closest)
                    {
                        dist_closest = dist;
                        angle_closest = angle;
                    }
                    

                }


                if (Counter.recordData)
                {
                    SaveFile(filepath, dist_closest, angle_closest);
                }
                // Debug.Log("Returned cloeset distance:");
                // Debug.Log(dist_closest);
            }

        }
        // generate a new data file that has incremental index
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("Generating new data file...");
            string[] file_array = filepath_origin.Split('.');
            filepath = file_array[0] + fileCounter.ToString() + '.' + file_array[1];
            Debug.Log(filepath);
            fileCounter++;
        }
        // toggle visibility
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleVisibility();
        }
        // switch tasks
        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchTasks();
        }
        // using IMU input
        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Now using IMU input");
            isIMU = true;
            isUDP = false;
        }
        // using UDP input
        if (Input.GetKeyDown(KeyCode.U))
        {
            print("Now using udp input");
            isIMU = false;
            isUDP = true;
        }

        // check if current data is for a new trial, must hit green button to initiate new try
        if (Counter.isNewTry)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
            {
                file.WriteLine("New Try");
            }
            Counter.isNewTry = false;
        }

        if (timerclass.timeStart)
        {
            timer += Time.deltaTime;
            visualTime = timer;
        }

        if (timerclass.timeReset)
        {
            timer = 0.0f;
            timerclass.timeReset = false;
        }

        if (timerclass.timeRecord)
        {
            Debug.Log("Task complete!");
            /*
            Debug.Log("Task elapsed time is: ");
            Debug.Log(visualTime);
            Debug.Log("peg counter is: ");
            Debug.Log(Counter.pegcounter);
            Debug.Log("wire counter is: ");
            Debug.Log(Counter.wirecounter);
            */
            timerclass.timeRecord = false;
        }

        if (MyButton.mom)
        {
            if (gripper_max == 0 && Counter.gripper_calibration % 2 == 1)
            {
                gripper_max = Counter.gripper_value;
                Debug.Log("grippermax value is " + gripper_max);
                gripper_min = 0;
            }
            if (gripper_min == 0 && Counter.gripper_calibration % 2 == 0)
            {
                gripper_min = Counter.gripper_value;
                Debug.Log("grippermin value is " + gripper_min);
                if (!MyButton.tog)
                {
                    gripper_min_now = gripper_min;
                    gripper_max_now = gripper_max;
                    Debug.Log("gripper calibration result: " + gripper_max_now + " " + gripper_min_now);
                }
                gripper_max = 0;
                

            }
        }
        if (MyButton.tog)
        {
            // Debug.Log("Begin Pre-planned path");
        }

        // run if there is an openZen event
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
                            HumanjointList[1].transform.rotation = sensorOrientation;
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
                            float angle = 0.0f;
                            Vector3 axis = Vector3.zero;
                            sensorOrientation.ToAngleAxis(out angle, out axis);
                            // Debug.DrawRay(IMU98.transform.position, axis, Color.green);
                            // Rotate elbow joint
                            HumanjointList[2].transform.rotation = Quaternion.AngleAxis(angle, axis);
                            break;
                    }
                }

                if (zenEvent.sensor.handle == mSensorHandle3.handle)
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
                            IMUusb.transform.rotation = sensorOrientation;
                            float angle = 0.0f;
                            Vector3 axis = Vector3.zero;
                            sensorOrientation.ToAngleAxis(out angle, out axis);
                            // Rotate wrist joint
                            Quaternion rawquat = Quaternion.AngleAxis(angle, axis);
                            Quaternion usboffset = Quaternion.AngleAxis(195, Vector3.up);
                            Quaternion usboffset2 = Quaternion.AngleAxis(90, Vector3.forward);
                            Quaternion usboffset3 = Quaternion.AngleAxis(-15, Vector3.right);
                            HumanjointList[3].transform.rotation = rawquat * usboffset * usboffset2 * usboffset3;
                            break;
                    }
                }
            }
        }

        // run if the mode is set to IMU control
        // if (isIMU)

        if (isIMU && !MyButton.tog)
        {
            last_pos = Hand_demo.transform.position;
            last_rot = Hand_demo.transform.rotation;
            last_FK = Vector3.zero;
            last_FKrot = Quaternion.identity;
        }
        if (isIMU && MyButton.tog)
        {
            // trans1 is the upper arm length
            // trans2 is the lower arm length
            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 trans0 = HumanjointList[1].transform.position;
            Vector3 trans1 = new Vector3(lu, 0f, 0f); // apply calibrated link length of upperarm
            Vector3 trans2 = new Vector3(lf, 0f, 0f); // apply calibrated link length of forearm
            Vector3 trans3 = new Vector3(lh, 0f, 0f); // apply calibrated link length of hand
            Matrix4x4 TrueShoulder = HumanjointList[1].transform.localToWorldMatrix;
            Matrix4x4 World2Shoulder = Matrix4x4.TRS(trans0, HumanjointList[1].transform.rotation, scale);
            Matrix4x4 Shoulder2Elbow = Matrix4x4.TRS(trans1, Quaternion.Inverse(HumanjointList[1].transform.rotation) * HumanjointList[2].transform.rotation, scale);
            Matrix4x4 Elbow2Wrist = Matrix4x4.TRS(trans2, Quaternion.Inverse(HumanjointList[2].transform.rotation) * HumanjointList[3].transform.rotation, scale);

            Vector3 Wrist2Palm = new Vector3(lh, 0f, 0f);

            Matrix4x4 FK = World2Shoulder * Shoulder2Elbow * Elbow2Wrist;
            Vector3 FKpos = FK.MultiplyPoint3x4(Wrist2Palm);
            Vector3 FKpos1 = new Vector3(FK[0, 3], FK[1, 3], FK[2, 3]);
            
            Quaternion rot = QuaternionFromMatrix(FK);
            // Quaternion handoffset = Quaternion.AngleAxis(-90, Vector3.right);
            Quaternion handoffset = Quaternion.AngleAxis(0, Vector3.right);

            // Calculate actual gripper value here:
            float actual_gripper = Mathf.InverseLerp((float)gripper_min_now, (float)gripper_max_now, (float)Counter.gripper_value);
            // Debug.Log(actual_gripper);

            // visualize the gripper here:
            float left_z = Mathf.Lerp(3, -15, actual_gripper);
            //Debug.Log("left is: "+left_z);
            Left_Anchor.transform.localEulerAngles = new Vector3(0.0f, 0.0f, left_z);
            float right_z = Mathf.Lerp(-3, 15, actual_gripper);
            //Debug.Log("right is: " + right_z);
            Right_Anchor.transform.localEulerAngles = new Vector3(0.0f, 0.0f, right_z);

            if (Counter.clutch_counter < 1)
            {
                // Assign pos and rot here for end-effector
                Hand_demo.transform.position = FKpos;
                Hand_demo.transform.rotation = rot * handoffset;
            }
            else
            {
                if (last_FK == Vector3.zero)
                {
                    last_FK = FKpos;
                }

                if (last_FKrot == Quaternion.identity)
                {
                    last_FKrot = rot * handoffset;
                }

                Hand_demo.transform.position =  FKpos + (last_pos - last_FK);
                Hand_demo.transform.rotation = rot * handoffset * Quaternion.Inverse(last_FKrot) * last_rot;
            }

            // send UDP
            // Backup string sendF = VectorFromMatrix(FK);
            string sendF = FKpos1.x.ToString() + "," + FKpos1.y.ToString() + "," + FKpos1.z.ToString() + "," + rot.x.ToString() + "," + rot.y.ToString() + "," + rot.z.ToString() + "," + rot.w.ToString();
            if (sendmsg)
            {
                //Debug.Log(FK);
                //Debug.Log(sendF);
                // connection.Send(sendF);
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                sendmsg = !sendmsg;
                if (sendmsg == true)
                {
                    Debug.Log("Sending UDP");
                }
                else
                {
                    Debug.Log("Stopped Sending UDP");
                }
                
            }


            if (Input.GetKeyDown(KeyCode.R))
            {
                print("calibration sequence: bl,br,tr,tl");
                print("Recorded Transformation Matrices: ");
                calibration(calibrationfile, World2Shoulder, Shoulder2Elbow, Elbow2Wrist);

            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("New Calibration Trail");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(calibrationfile, true))
                {
                    file.WriteLine("New Try \n");
                }
            }
            if (Input.GetKeyDown(KeyCode.J))
            {
                
                // Serialize transformation matrix to mock json string
                string mockF = VectorFromMatrix(FK);
                Debug.Log("Use the following for Simulated Json strings");
                Debug.Log(mockF);

                generateSphere = true;
                if (generateSphere)
                {

                }

            }
        }
        if (isUDP)
        {
            if (UDPInfo.lastReceivedUDPPacket != null)
            {
                // output json stream
                // Debug.Log(UDPInfo.lastReceivedUDPPacket);
                Matrix4x4 m = parser.GetMatrix4X4(UDPInfo.lastReceivedUDPPacket);
                Vector3 UDPpos = m.MultiplyPoint3x4(Vector3.zero);
                // Debug.Log(UDPpos);
                Vector3 offset = new Vector3(0f, 0.15f, 0.92f);
                Hand_demo.transform.position = UDPpos + offset;
                Quaternion rot = QuaternionFromMatrix(m);
                Quaternion handoffset = Quaternion.AngleAxis(0, Vector3.right);
                Hand_demo.transform.rotation = rot * handoffset;

                string sendU = Hand_demo.transform.position.x.ToString() + "," + Hand_demo.transform.position.y.ToString() + "," + Hand_demo.transform.position.z.ToString() + "," + Hand_demo.transform.rotation.x.ToString() + "," + Hand_demo.transform.rotation.y.ToString() + "," + Hand_demo.transform.rotation.z.ToString() + "," + Hand_demo.transform.rotation.w.ToString();
                if (sendmsg)
                {
                    //Debug.Log(FK);
                    //Debug.Log(sendU);
                    // connection.Send(sendU);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    sendmsg = !sendmsg;
                    if (sendmsg == true)
                    {
                        Debug.Log("Sending UDP with MTM input");
                    }
                    else
                    {
                        Debug.Log("Stopped Sending UDP with MTM input");
                    }

                }
            }

        }
    }
    // extract simulated json string from a FK matrix
    public string VectorFromMatrix(Matrix4x4 m)
    {
        jsonFloat jsonfloat = new jsonFloat();
        string jsonString = m.ToString();
        string rotation = string.Empty;
        rotation += "[";
        float[] translation = new float[3];
        string[] sArray = jsonString.Split('\n');
        for (int i = 0; i < 3; i++) // only get the first 3 rows of Transformation matrix
        {
            Vector3 row = m.GetRow(i);
            Vector4 row_trans = m.GetRow(i);
            translation[i] = row_trans[3];
        }
        for (int i = 0; i < sArray.Length - 2; i++) // only get the first 3 rows of Transformation matrix
        {
            string[] rArray = sArray[i].Split('\t');
            rotation += "[";
            for (int j = 0; j < rArray.Length - 1; j++)
            {
                if (j != rArray.Length - 2)
                {
                    rotation += rArray[j] + ",";
                }
                else
                {
                    rotation += rArray[j];
                }
            }
            if (i != sArray.Length - 3)
            {
                rotation += "],";
            }
            else
            {
                rotation += "]";
            }
        }
        rotation += "]";
        jsonfloat.Rotation = rotation;
        jsonfloat.Translation = translation;
        string mockString = JsonUtility.ToJson(jsonfloat);
        // begin funny string operation because jsonUtility doesn't support 2d array serialization
        string[] mockArray = mockString.Split('[');
        string first_part = mockArray[0].Substring(0, mockArray[0].Length - 1);
        string second_part = mockString.Substring(mockArray[0].Length, mockString.Length - mockArray[0].Length);
        string[] mockArray_2 = second_part.Split('\"');
        string second_part_2 = mockArray_2[0];
        string third_part = second_part.Substring(second_part_2.Length + 1, second_part.Length - (second_part_2.Length + 1));
        string final_string = first_part + second_part_2 + third_part;
        final_string = "\"Position\":" + final_string;
        return final_string;
    }

    // get quaternion from FK matrix
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
    { 
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); 
    }

    // find all joints in a nested gameobject
    void initializeJoints()
    {
        var HumanChildren = Human.GetComponentsInChildren<Transform>();
        for (int i = 0; i < HumanChildren.Length; i++)
        {
            if (HumanChildren[i].name == "Shoulder")
            {
                HumanjointList[0] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "upperarm")
            {
                HumanjointList[1] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "lowerarm")
            {
                HumanjointList[2] = HumanChildren[i].gameObject;
            }
            else if (HumanChildren[i].name == "hand_empty")
            {
                HumanjointList[3] = HumanChildren[i].gameObject;
            }
        }
    }

    // GUI
    void OnGUI()
    {
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;
        GUIStyle labelDetails = new GUIStyle(GUI.skin.GetStyle("label"));
        labelDetails.fontSize = 14;
        labelDetails.normal.textColor = Color.black;
        if (GUI.Button(new Rect(Screen.width - 170, 60, 150, 30), "IMU29 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU29");
            ZenComponentHandle_t mComponent = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle, OpenZen.g_zenSensorType_Imu, 0, mComponent);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle, mComponent,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

        if (GUI.Button(new Rect(Screen.width - 170, 100, 150, 30), "IMU98 Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMU98");
            ZenComponentHandle_t mComponent2 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle2, OpenZen.g_zenSensorType_Imu, 0, mComponent2);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle2, mComponent2,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

        if (GUI.Button(new Rect(Screen.width - 170, 140, 150, 30), "IMUusb Heading Reset", myButtonStyle))
        {
            print("Performing Heading Reset for IMUusb");
            ZenComponentHandle_t mComponent3 = new ZenComponentHandle_t();
            OpenZen.ZenSensorComponentsByNumber(mZenHandle, mSensorHandle3, OpenZen.g_zenSensorType_Imu, 0, mComponent3);
            // perform heading reset 
            OpenZen.ZenSensorComponentSetInt32Property(mZenHandle, mSensorHandle3, mComponent3,
                (int)EZenImuProperty.ZenImuProperty_OrientationOffsetMode, 1);
        }

        //if (GUI.Button(new Rect(20, 20, 150, 30), "Calibrate Mousepad", myButtonStyle))
        //{
        //    print("Record palm position");
        //    
        //}
        GUI.Label(new Rect(20,20,150,50),
            "Timer: " + visualTime.ToString("f4") + " seconds.", labelDetails);
    }

    void ToggleVisibility()
    {
        Human.SetActive(!Human.activeSelf);
        Hand_demo.SetActive(!Hand_demo.activeSelf);
    }

    void SwitchTasks()
    {


    }

    void OnDestroy()
    {
        if (mSensorHandle != null)
        {
            OpenZen.ZenReleaseSensor(mZenHandle, mSensorHandle);
        }
        OpenZen.ZenShutdown(mZenHandle);

        // connection.Stop();
    }

    void createSphere(Vector3 pos)
    {
        GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Vector3 newLoc = new Vector3(0.025f, 0.025f, 0.025f);
        mySphere.transform.localScale = newLoc;
        mySphere.transform.position = pos;
        generateSphere = false;
    }

    // return the closest point from the hand to the wire, correct but not used
    Vector3 closestPoint(Vector3 x0, Vector3 x1, Vector3 x2)
    {
        Vector3 norm = x1 - x2;
        float t = - Vector3.Dot(x1 - x0, x2 - x1) / Mathf.Pow(norm.magnitude,2);
        return x1 + (x2 - x1) * t;
    }

    // return the distance from the hand to the wire
    float distanceToclosestPoint(Vector3 x0, Vector3 x1, Vector3 s)
    {
        Vector3 crossproduct = Vector3.Cross(x1 - x0, s);
        float d =  crossproduct.magnitude / s.magnitude;
        return d;
    }

    float distanceTocenter(Vector3 x0, Vector3 center, float radius)
    {
        float d = Vector3.Distance(x0, center);
        d = Mathf.Abs(d - radius);
        return d;
    }

    public void SaveFile(string filepath, float distance, float myangle)
    {
        /*
        float mytime = timer;
        float mydistance = dist;
        float[] mypos = new float[3];
        mypos[0] = x0.x;
        mypos[1] = x0.y;
        mypos[2] = x0.z;
        float[] myquat = new float[4];
        myquat[0] = q.x;
        myquat[1] = q.y;
        myquat[2] = q.z;
        myquat[3] = q.w;
        HandData data = new HandData(mytime,mydistance,mypos, myquat);
        */
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(timer + "," + distance + "," + myangle );
        }
        Debug.Log(timer + "," + distance + "," + myangle);
    }

    public void calibration(string filepath, Matrix4x4 w2s, Matrix4x4 s2e, Matrix4x4 e2w)
    {

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(w2s);
            file.WriteLine(s2e);
            file.WriteLine(e2w);

        }
    }

}
