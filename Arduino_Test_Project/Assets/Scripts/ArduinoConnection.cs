using UnityEngine;
using System.IO.Ports;

public class ArduinoConnection : MonoBehaviour
{
    SerialPort serial = new SerialPort("COM5", 115200);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        serial.Open();
        serial.ReadTimeout = 200;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        string data = serial.ReadLine();
        //Debug.Log("Recevied: '" + data + "'");
        int value = int.Parse(data);

        float normalized = (value - 512) / 512f;
        float rotationSpeed = normalized * 100f;

        //transform.rotation = Quaternion.Euler(0, value * 360 / 1023, 0);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        */

        string data = serial.ReadLine().Trim();

        string[] parts = data.Split(',');

        if (parts.Length == 2)
        {
            int xValue = int.Parse(parts[0]);
            int yValue = int.Parse(parts[1]);

            float xNormalized = (xValue - 512) / 512f;
            float yNormalized = (yValue - 512) / 512f;

            float rotaionSpeed = 100;

            float xRotation = -xNormalized * rotaionSpeed * Time.deltaTime;
            float yRotation = yNormalized * rotaionSpeed * Time.deltaTime;

            transform.Rotate(xRotation, yRotation, 0, Space.Self);
        }

        
    }
}
