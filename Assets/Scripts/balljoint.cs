using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class balljoint : MonoBehaviour
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
    string filepath = "Assets/debug.txt";
    // Humanoid declaration
    public GameObject Human;
    private GameObject[] HumanjointList = new GameObject[4];
    public GameObject Palm;
    public GameObject Hand_demo;
    public GameObject Pegs;
    public GameObject Wire;
    public GameObject Wire2;
    public GameObject Wire3;
    // public string jsonFilePath = "Assets/Scripts/data2.json";
    public float lu = 0.3f;
    public float lf = 0.25f;
    bool isIMU = false;
    bool isUDP = false;
    bool generateSphere = true;
    Vector3 spheretrans = new Vector3(0.125f, 0f, 0f);
    Vector3 x0;
    Vector3 x1;
    Vector3 x2;
    Quaternion q;
    private float timer = 0.0f;
    private float dist = -1.0f;
    private float visualTime = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        Hand_demo.SetActive(!Hand_demo.activeSelf);
        Wire.SetActive(!Wire.activeSelf);
        Wire2.SetActive(!Wire2.activeSelf);
        Pegs.SetActive(!Pegs.activeSelf);

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
    }

    // Update is called once per frame
    void Update()
    {
        q = Hand_demo.transform.rotation;
        x0 = Hand_demo.transform.position;
        x1 = Wire3.transform.position - spheretrans;
        x2 = Wire3.transform.position + spheretrans;

        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleVisibility();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SwitchTasks();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            print("Now using IMU input");
            isIMU = true;
            isUDP = false;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            print("Now using udp input");
            isIMU = false;
            isUDP = true;
        }

        if (Counter.isNewTry)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
            {
                file.WriteLine("New Try");
            }
            Counter.isNewTry = false;
        }
        if (Counter.recordData)
        {
            
            dist = distanceToclosestPoint(x0, x1, x2);
            Debug.Log(dist);
            SaveFile(filepath);
            Debug.Log("Saving file to txt:");
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
            Debug.Log("Task elapsed time is: ");
            Debug.Log(visualTime);
            Debug.Log("peg counter is: ");
            Debug.Log(Counter.pegcounter);
            Debug.Log("wire counter is: ");
            Debug.Log(Counter.wirecounter);
            timerclass.timeRecord = false;
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
            }
        }

        // run if the mode is set to IMU control
        if (isIMU)
        {
            // trans1 is the upper arm length
            // trans2 is the lower arm length
            Vector3 scale = new Vector3(1, 1, 1);
            Vector3 trans0 = HumanjointList[1].transform.position;
            Vector3 trans1 = new Vector3(lu, 0f, 0f); // apply calibrated link length of upperarm
            Vector3 trans2 = new Vector3(lf, 0f, 0f); // apply calibrated link length of forearm
            Matrix4x4 TrueShoulder = HumanjointList[1].transform.localToWorldMatrix;
            Matrix4x4 World2Shoulder = Matrix4x4.TRS(trans0, HumanjointList[1].transform.rotation, scale);
            Matrix4x4 Shoulder2Elbow = Matrix4x4.TRS(trans1, Quaternion.Inverse(HumanjointList[1].transform.rotation) * HumanjointList[2].transform.rotation, scale);
            Matrix4x4 Elbow2Wrist = Matrix4x4.TRS(trans2, Quaternion.Inverse(HumanjointList[2].transform.rotation) * HumanjointList[3].transform.rotation, scale);

            Vector3 Wrist2Palm = new Vector3(0.2f, 0f, 0f);

            Matrix4x4 FK = World2Shoulder * Shoulder2Elbow * Elbow2Wrist;
            Vector3 FKpos = FK.MultiplyPoint3x4(Wrist2Palm);
            Hand_demo.transform.position = FKpos;
            Quaternion rot = QuaternionFromMatrix(FK);
            Quaternion handoffset = Quaternion.AngleAxis(-90, Vector3.right);
            // Quaternion handoffset = Quaternion.AngleAxis(0, Vector3.right);
            Hand_demo.transform.rotation = rot * handoffset;

            if (Input.GetKeyDown(KeyCode.R))
            {
                print("Record Transformation Matrices: ");
                print(World2Shoulder);
                print(Shoulder2Elbow);
                print(Elbow2Wrist);
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
                    Vector3 cp = closestPoint(x0, x1, x2);
                    dist = distanceToclosestPoint(x0, x1, x2);
                    Debug.Log(dist);
                    createSphere(cp);
                }

            }
        }
        if (isUDP)
        {
            if (UDPInfo.lastReceivedUDPPacket != null)
            {
                Vector3 Wrist2Palm = new Vector3(0.2f, 0f, 0f);
                // Debug.Log(UDPInfo.lastReceivedUDPPacket);
                Matrix4x4 m = parser.GetMatrix4X4(UDPInfo.lastReceivedUDPPacket);
                Vector3 UDPpos = m.MultiplyPoint3x4(Wrist2Palm);
                // Debug.Log(UDPpos);
                Vector3 offset = new Vector3(0.0f, -0.1f, 0.9f);
                Hand_demo.transform.position = UDPpos + offset;
                Quaternion rot = QuaternionFromMatrix(m);
                Quaternion handoffset = Quaternion.AngleAxis(0, Vector3.right);
                Hand_demo.transform.rotation = rot * handoffset;
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

    public static Quaternion QuaternionFromMatrix(Matrix4x4 m) 
    { 
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1)); 
    }

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
            else if (HumanChildren[i].name == "hand")
            {
                HumanjointList[3] = HumanChildren[i].gameObject;
            }
        }
    }

    void OnGUI()
    {
        GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button);
        myButtonStyle.fontSize = 13;
        GUIStyle labelDetails = new GUIStyle(GUI.skin.GetStyle("label"));
        labelDetails.fontSize = 14;
        labelDetails.normal.textColor = Color.black;
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

        //if (GUI.Button(new Rect(20, 20, 150, 30), "Calibrate Mousepad", myButtonStyle))
        //{
        //    print("Record palm position");
        //    Calibrate();
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
        Pegs.SetActive(!Pegs.activeSelf);
        Wire.SetActive(!Wire.activeSelf);
        Wire2.SetActive(!Wire2.activeSelf);
    }

    void Calibrate()
    {

    }

    void OnDestroy()
    {
        if (mSensorHandle != null)
        {
            OpenZen.ZenReleaseSensor(mZenHandle, mSensorHandle);
        }
        OpenZen.ZenShutdown(mZenHandle);
    }

    void createSphere(Vector3 pos)
    {
        GameObject mySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Vector3 newLoc = new Vector3(0.05f, 0.05f, 0.05f);
        mySphere.transform.localScale = newLoc;
        mySphere.transform.position = pos;
        generateSphere = false;
    }
    // return the closest point from the hand to the wire
    // correct
    Vector3 closestPoint(Vector3 x0, Vector3 x1, Vector3 x2)
    {
        Vector3 norm = x1 - x2;
        float t = - Vector3.Dot(x1 - x0, x2 - x1) / Mathf.Pow(norm.magnitude,2);
        return x1 + (x2 - x1) * t;
    }

    // return the distance from the hand to the wire
    // correct
    float distanceToclosestPoint(Vector3 x0, Vector3 x1, Vector3 x2)
    {
        Vector3 norm = x1 - x2;
        Vector3 crossproduct = Vector3.Cross(x1 - x0, x2 - x1);
        float d =  crossproduct.magnitude / norm.magnitude;
        return d;
    }

    public void SaveFile(string filepath)
    {

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
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, true))
        {
            file.WriteLine(data.time + "," + data.distance + "," + x0.x + "," +x0.y + "," +x0.z);
        }
    }

}
