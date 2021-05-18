using UnityEngine;
using System.Collections;
using Uduino; // adding Uduino NameSpace 

public class MultipleButtons : MonoBehaviour
{

    public class UduinoButton
    {
        public int _pin;
        //public GameObject _buttonGameObject;
        int buttonValue = 0;
        int prevButtonValue = 0;

        public UduinoButton(int pin)
        {
            _pin = pin;
            //_buttonGameObject = gameObject;
            UduinoManager.Instance.pinMode(pin, PinMode.Input_pullup);
        }

        public int Read() {
            buttonValue = UduinoManager.Instance.digitalRead(_pin);
            return buttonValue;
        }

        public bool pressedDown() {
            if (valueChanged() && buttonValue == 1)
            {
                prevButtonValue = buttonValue;
                return true;
            }
            return false;
        }

        public bool pressedUp() {
            if (valueChanged() && buttonValue == 0)
            {
                prevButtonValue = buttonValue;
                return true;
            }
            return false;
        }

        bool valueChanged() {
            if (buttonValue != prevButtonValue)
                return true;
            return false;
        }

    }

    UduinoManager u; // The instance of Uduino is initialized here
    private int buttonOnePin = 9; // momentary
    private int buttonTwoPin = 13; // toggle
    public GameObject buttonGameObject_mom;
    public GameObject buttonGameObject_tog;

    UduinoButton buttonOne;
    UduinoButton buttonTwo;


    void Start()
    {
        buttonOne = new UduinoButton(buttonOnePin);
        buttonTwo = new UduinoButton(buttonTwoPin);

        UduinoManager.Instance.OnDataReceived += DataReceived;
    }

    void DataReceived(string value, UduinoDevice board) {
        //Debug.Log(value); //Used for debugging purpose

        try
        {
            int proximity = int.Parse(value);
            Counter.gripper_value = proximity;
            // Debug.Log(proximity);
        }
        catch (System.FormatException e)
        {
            // Debug.Log("filtered out bad stuff");
        }
    }


    private void Update()
    {
        buttonOne.Read();
        buttonTwo.Read();

    
        if (buttonOne.pressedDown())
            PressedDown(buttonOne);
        else if (buttonOne.pressedUp())
            PressedUp(buttonOne);

        if (buttonTwo.pressedDown())
            PressedDown(buttonTwo);
        else if (buttonTwo.pressedUp())
            PressedUp(buttonTwo);


        //UduinoManager.Instance.SendBundle("PinRead");
    }

    void PressedDown(UduinoButton button)
    {
        if (button._pin == 9)
        {
            //Debug.Log("Momentary");
            MyButton.mom = true;
            Counter.gripper_calibration++;
            buttonGameObject_mom.GetComponent<Renderer>().material.color = Color.red;
            //buttonGameObject_mom.transform.Translate(Vector3.up / 20);
        }
        else if (button._pin == 13)
        {
            //Debug.Log("Toggle");
            MyButton.tog = true;
            buttonGameObject_tog.GetComponent<Renderer>().material.color = Color.red;
            //buttonGameObject_tog.transform.Translate(Vector3.up / 20);
        }
        
        //Debug.Log("Pressed down");
    }

    void PressedUp(UduinoButton button)
    {
        if (button._pin == 9)
        {
            //Debug.Log("Momentary");
            MyButton.mom = false;
            buttonGameObject_mom.GetComponent<Renderer>().material.color = Color.white;
            //buttonGameObject_mom.transform.Translate(Vector3.down / 20);
        }
        else if (button._pin == 13)
        {
            //Debug.Log("Toggle");
            Counter.clutch_counter++;
            //Debug.Log(Counter.clutch_counter);
            MyButton.tog = false;
            buttonGameObject_tog.GetComponent<Renderer>().material.color = Color.white;
            //buttonGameObject_tog.transform.Translate(Vector3.down / 20);
        }
        //Debug.Log("Pressed up");
    }
}