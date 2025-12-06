using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoSerialInput : MonoBehaviour
{
    [Header("Serial Config")]
    public string portName = "COM4";
    public int baudRate = 9600;

    // Public values your game can read
    public static float HorizontalInput { get; private set; }
    public static float VerticalInput { get; private set; }

    private SerialPort serialPort;
    private Thread readThread;
    private volatile bool isRunning = false;

    // Buffer for passing thread -> main thread
    private string latestLine = "";
    private readonly object lineLock = new object();

    private void Start()
    {
        TryOpenPort();
    }

    private void TryOpenPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.NewLine = "\n";          // important
            serialPort.ReadTimeout = 50;

            serialPort.Open();

            isRunning = true;
            readThread = new Thread(SerialReadLoop);
            readThread.Start();

            Debug.Log("[Serial] Port " + portName + " opened.");
        }
        catch (Exception e)
        {
            Debug.LogError("[Serial] Failed to open " + portName + ": " + e.Message);
        }
    }

    private void SerialReadLoop()
    {
        while (isRunning && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine().Trim();

                lock (lineLock)
                {
                    latestLine = line;  // store for main thread
                }
            }
            catch (TimeoutException)
            {
                // expected, ignore
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Serial Thread] Error: " + e.Message);
            }
        }
    }

    private void Update()
    {
        string line = "";

        lock (lineLock)
        {
            if (!string.IsNullOrEmpty(latestLine))
            {
                line = latestLine;
                latestLine = "";
            }
        }

        if (!string.IsNullOrEmpty(line))
        {
            Debug.Log("[Serial] " + line);  // safe to log on main thread
            ParseLine(line);
        }
    }

    private void ParseLine(string line)
    {
        if (!line.Contains("|"))
            return;

        string[] parts = line.Split('|');
        if (parts.Length != 2)
            return;

        if (float.TryParse(parts[0], out float h) &&
            float.TryParse(parts[1], out float v))
        {
            HorizontalInput = h;
            VerticalInput = v;
        }
    }

    private void OnApplicationQuit()
    {
        isRunning = false;

        if (readThread != null && readThread.IsAlive)
            readThread.Join();

        ClosePort();
    }

    public void ClosePort()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log("[Serial] Port closed.");
        }
    }
}
