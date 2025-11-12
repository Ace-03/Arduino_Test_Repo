using System.IO.Ports;
using UnityEngine;
using System.Threading;
using System;

public class ArduinoSerialInput : MonoBehaviour
{
    // --- Configuration ---
    [Header("Serial Configuration")]
    [Tooltip("The COM port your Arduino is connected to (e.g., COM3 on Windows, /dev/ttyACM0 on Linux)")]
    [SerializeField] private string portName = "COM3";
    [Tooltip("MUST match the BAUD_RATE in the Arduino sketch (e.g., 9600)")]
    [SerializeField] private int baudRate = 9600;

    // --- Static Input Properties ---
    // These static variables are what GlidingSystemV2 will read.
    public static float HorizontalInput { get; private set; } = 0f;
    public static float VerticalInput { get; private set; } = 0f;

    // --- Internal State ---
    private SerialPort serialPort;
    private Thread readThread;
    private bool isRunning = false;
    private readonly object lockObject = new object(); // For thread-safe access

    private void Start()
    {
        InitializeSerialPort();
    }

    private void InitializeSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.Open();
            serialPort.ReadTimeout = 1; // Short timeout to prevent thread from stalling

            isRunning = true;
            // Start the reading process in a separate thread
            readThread = new Thread(ReadSerialData);
            readThread.Start();

            Debug.Log($"Serial Port {portName} opened successfully at {baudRate} baud.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error opening serial port {portName}: {e.Message}");
            // Handle the error (e.g., disable controls if the port fails to open)
        }
    }

    private void ReadSerialData()
    {
        // 
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                // Read until the '\n' terminator (sent by Arduino's Serial.println)
                string data = serialPort.ReadLine();
                ParseData(data);
            }
            catch (TimeoutException)
            {
                // This is normal, just means no new line came in before the timeout
            }
            catch (Exception e)
            {
                // Log any unexpected errors
                if (isRunning) // Only log if we didn't explicitly close the port
                {
                    Debug.LogWarning($"Serial reading error: {e.Message}");
                }
            }
        }
    }

    private void ParseData(string data)
    {
        // Expected Format: "H_VALUE|V_VALUE"
        string[] values = data.Split('|');

        if (values.Length == 2)
        {
            float hVal = 0f;
            float vVal = 0f;

            // Attempt to parse the strings into floats
            bool hSuccess = float.TryParse(values[0], out hVal);
            bool vSuccess = float.TryParse(values[1], out vVal);

            if (hSuccess && vSuccess)
            {
                // Use the lock to ensure Unity's main thread doesn't read 
                // while the new thread is writing to the static variables
                lock (lockObject)
                {
                    HorizontalInput = hVal;
                    VerticalInput = vVal;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        // 1. Set the flag to stop the thread loop
        isRunning = false;

        // 2. Wait for the thread to finish (optional, but good practice)
        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join();
        }

        // 3. Close the serial port
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Serial Port {portName} closed.");
        }
    }
}