#include <Arduino_LSM9DS1.h>

// Define the baud rate for serial communication.
// This MUST match the baud rate in your Unity script (9600 in the previous example).
const long BAUD_RATE = 9600;

// The maximum expected gyroscope reading in degrees per second (dps).
// The LSM9DS1 has selectable ranges (245, 500, 2000 dps). 
// 2000 dps provides the widest range but is most sensitive to noise.
// 245 dps is a safe starting point for controller input.
const float MAX_GYRO_RATE = 245.0; // The range in deg/s.

// Variables to hold the normalized input values (range -1.0 to 1.0)
float horizontalInput = 0.0; // Corresponds to YAW (rotation around the Z-axis of the chip/board)
float verticalInput = 0.0;   // Corresponds to PITCH (rotation around the X-axis of the chip/board)

void setup() {
  Serial.begin(BAUD_RATE);
  // Wait for the serial monitor to be ready, helpful for debugging
  while (!Serial); 

  // Initialize the IMU (Gyroscope)
  if (!IMU.begin()) {
    Serial.println("Failed to initialize IMU!");
    // Loop indefinitely if IMU fails, as the core functionality is broken
    while (1); 
  }
  
  // Optional: Print a status message
  Serial.print("Gyroscope sensitivity is currently set to: ");
  Serial.print(IMU.gyroscopeRange());
  Serial.println(" deg/s");
}

void loop() {
  // Read the gyroscope data if available
  // gx, gy, gz will be the angular velocity in degrees per second (dps)
  float gx, gy, gz;

  if (IMU.gyroscopeAvailable()) {
    IMU.readGyroscope(gx, gy, gz);
    
    // --- MAPPING THE GYRO AXES TO GAME INPUTS ---
    // The Nano 33 BLE Sense board orientation generally maps:
    // * X-axis rotation (gx) to Pitch (up/down rotation)
    // * Y-axis rotation (gy) to Roll (banking)
    // * Z-axis rotation (gz) to Yaw (left/right rotation)

    // 1. Vertical Input (Pitch)
    // We'll use the X-axis rotation (gx) for Pitch (vertical control).
    // The value is normalized by dividing by the maximum expected rate.
    // The value will range from -1.0 to 1.0 (or slightly more/less if you exceed the MAX_GYRO_RATE).
    verticalInput = gx / MAX_GYRO_RATE;
    
    // 2. Horizontal Input (Yaw)
    // We'll use the Z-axis rotation (gz) for Yaw (horizontal control).
    horizontalInput = gz / MAX_GYRO_RATE;

    // Optional: Clamp the values to the -1.0 to 1.0 range just in case of extreme rotation
    horizontalInput = constrain(horizontalInput, -1.0, 1.0);
    verticalInput = constrain(verticalInput, -1.0, 1.0);

    // 3. Format and Send Data
    // Format: "H_VALUE|V_VALUE\n"
    if (Serial.availableForWrite() > 0) {
      // Print Horizontal Value (Yaw)
      Serial.print(horizontalInput, 2); 
      // Print Separator
      Serial.print("|");
      // Print Vertical Value (Pitch) and the Line Terminator
      Serial.println(verticalInput, 2);
    }
  }

  // Delay slightly to prevent flooding the serial port and allow Unity to process.
  delay(10); 
}
