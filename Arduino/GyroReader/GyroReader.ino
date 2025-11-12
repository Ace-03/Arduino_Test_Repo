// Define the baud rate for serial communication.
// This MUST match the baud rate in your Unity script.
const long BAUD_RATE = 9600;

// Variables to hold your gyro readings
// Use 'float' for smooth, analog-like values
float horizontalInput = 0.0; // Yaw/Horizontal Control
float verticalInput = 0.0;   // Pitch/Vertical Control

void setup() {
  Serial.begin(BAUD_RATE);
  // Initialize your gyros here
  // e.g., Wire.begin();
  // e.g., initializeGyro1();
  // e.g., initializeGyro2();
}

void loop() {
  // 1. Read the raw gyro data
  // **REPLACE THESE STUBS with your actual gyro reading code**
  // For demonstration, these stubs assume a function that returns a value between -1.0 and 1.0,
  // which is common for mapped inputs. If your gyros return angles (e.g., 0-360), you will
  // need to map them to the range (-1.0 to 1.0) or (min to max) that your Unity script expects.
  horizontalInput = readHorizontalGyro(); // Replace with your actual horizontal gyro logic
  verticalInput = readVerticalGyro();     // Replace with your actual vertical gyro logic

  // 2. Format the data string
  // Format: "H_VALUE|V_VALUE\n"
  // - H and V are the horizontal and vertical floating-point values.
  // - The '|' is a SEPARATOR that Unity will use to split the string.
  // - The '\n' (newline) is the TERMINATOR that Unity will wait for.
  // The precision '2' means two decimal places.
  
  // Example data string: "-0.56|0.92\n"

  if (Serial.availableForWrite() > 0) {
    // Print Horizontal Value
    Serial.print(horizontalInput, 2); 
    // Print Separator
    Serial.print("|");
    // Print Vertical Value and the Line Terminator
    Serial.println(verticalInput, 2);
  }

  // Delay slightly to prevent flooding the serial port and allow Unity to process.
  delay(10); 
}

// *** Placeholder Functions - Replace with your actual gyro reading logic ***
float readHorizontalGyro() {
  // Example: Return a random value between -1.0 and 1.0 for testing
  return (random(-100, 101) / 100.0);
}

float readVerticalGyro() {
  // Example: Return a random value between -1.0 and 1.0 for testing
  return (random(-100, 101) / 100.0);
}
