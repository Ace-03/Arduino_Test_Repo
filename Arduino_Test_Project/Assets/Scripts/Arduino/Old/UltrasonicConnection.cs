using System.IO.Ports;
using UnityEngine;

public class UltrasonicConnection : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SerialPort serial = new SerialPort("COM5", 9600);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        serial.Open();
        serial.ReadTimeout = 50;
    }

    // Update is called once per frame
    void Update()
    {
        string data = serial.ReadLine();
        Debug.Log("Recevied: '" + data + "'");
        float value = float.Parse(data);
        
        transform.position = new Vector3(value, 0, 0);
        
        /*
        float normalized = (value - 512) / 512f;
        float rotationSpeed = normalized * 100f;

        transform.rotation = Quaternion.Euler(0, value * 360 / 1023, 0);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        */
    }
}
