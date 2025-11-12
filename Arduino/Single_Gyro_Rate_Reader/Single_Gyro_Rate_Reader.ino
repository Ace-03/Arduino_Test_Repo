#include <Arduino_BMI270_BMM150.h> 

const long BAUD_RATE = 9600;

// *** NEW ANGLE CONSTANTS ***
// Max tilt angle (in degrees) that corresponds to a normalized input of +/- 1.0
const float MAX_TILT_ANGLE = 35.0; 
// Dead Zone is now an angular dead zone (in degrees)
const float ANGULAR_DEAD_ZONE_DEG = 2.0; 

// Variables for output
float horizontalInput = 0.0;
float verticalInput = 0.0;   

void setup() {
  Serial.begin(BAUD_RATE);

  if (!IMU.begin()) {
    Serial.println("Failed to initialize IMU! (Fatal Error)");
    while (1); 
  }
  
  delay(100); 

  Serial.println("IMU Initialized (Reading Angle via Accelerometer).");
  Serial.print("Max Angle for Full Input: +/- ");
  Serial.print(MAX_TILT_ANGLE, 1);
  Serial.println(" degrees");
  Serial.print("Angular Dead Zone: +/- ");
  Serial.print(ANGULAR_DEAD_ZONE_DEG, 1);
  Serial.println(" degrees");
}

void loop() {
  // ax, ay, az will be the acceleration values in G's (force of gravity)
  float ax, ay, az;

  if (IMU.accelerationAvailable()) { 
    IMU.readAcceleration(ax, ay, az);
    
    // --- 1. CALCULATE PITCH AND ROLL ANGLES ---
    
    // Calculate Roll Angle (Rotation around X-axis -> Horizontal Control)
    // Roll (Horizontal) is typically determined by the Y-axis acceleration
    float rollAngle = atan2(ay, az) * 180.0 / PI; 

    // Calculate Pitch Angle (Rotation around Y-axis -> Vertical Control)
    // Pitch (Vertical) is typically determined by the X-axis acceleration
    float pitchAngle = atan2(-ax, sqrt(ay*ay + az*az)) * 180.0 / PI;
    
    // --- 2. NORMALIZE ANGLES TO INPUT RANGE [-1.0, 1.0] ---
    
    // Vertical Input (Pitch)
    if (abs(pitchAngle) > ANGULAR_DEAD_ZONE_DEG) {
        // Map the angle from the dead zone to the max tilt angle
        verticalInput = mapFloat(pitchAngle, -MAX_TILT_ANGLE, MAX_TILT_ANGLE, ANGULAR_DEAD_ZONE_DEG);
    } else {
        verticalInput = 0.0;
    }
    
    // Horizontal Input (Roll)
    if (abs(rollAngle) > ANGULAR_DEAD_ZONE_DEG) {
        horizontalInput = mapFloat(rollAngle, -MAX_TILT_ANGLE, MAX_TILT_ANGLE, ANGULAR_DEAD_ZONE_DEG);
    } else {
        horizontalInput = 0.0;
    }

    // --- 3. FORMAT AND SEND DATA ---
    if (Serial.availableForWrite() > 0) {
      // Output format: H_VALUE|V_VALUE\n
      Serial.print(horizontalInput, 2); 
      Serial.print("|");
      Serial.println(verticalInput, 2); 
    }
  }
  
  delay(20); 
}

// Custom map function for floating-point numbers with a dead zone.
float mapFloat(float inVal, float minIn, float maxIn, float deadZone) {
    if (abs(inVal) <= deadZone) {
        return 0.0;
    }

    float outVal;
    
    if (inVal > 0) {
        // Map positive side: [deadZone, maxIn] -> [0.0, 1.0]
        outVal = (inVal - deadZone) / (maxIn - deadZone);
    } else { // inVal < 0
        // Map negative side: [minIn, -deadZone] -> [-1.0, 0.0]
        outVal = (inVal + deadZone) / (abs(minIn) - deadZone);
    }
    
    // Ensure the output is clamped between -1.0 and 1.0 
    return constrain(outVal, -1.0, 1.0);
}