#include <Wire.h>

// MPU6050 I2C address
#define GYRO_ADDRESS 0x68

// Gyro registers
#define GYRO_XOUT_H 0x43
#define GYRO_YOUT_H 0x45

// Expected angular velocity range for mapping (deg/sec)
const float MIN_GYRO = -10.0f;
const float MAX_GYRO = 10.0f;

// Calibration
float neutralX = 0;
float neutralY = 0;
const int CALIBRATION_SAMPLES = 200;

// Filtering
float prevX = 0;
float prevY = 0;
const float ALPHA = 0.1; // smoothing factor (0 = no movement, 1 = no smoothing)

void setup() {
  Serial.begin(9600);
  Wire.begin();

  // Wake up MPU6050
  Wire.beginTransmission(GYRO_ADDRESS);
  Wire.write(0x6B);
  Wire.write(0x00);
  Wire.endTransmission(true);
  delay(100);

  // --- Calibration ---
  long sumX = 0;
  long sumY = 0;
  for (int i = 0; i < CALIBRATION_SAMPLES; i++) {
    int16_t rawX = readGyro(GYRO_XOUT_H);
    int16_t rawY = readGyro(GYRO_YOUT_H);
    sumX += rawX;
    sumY += rawY;
    delay(5);
  }
  neutralX = (float)sumX / CALIBRATION_SAMPLES / 131.0; // convert to deg/sec
  neutralY = (float)sumY / CALIBRATION_SAMPLES / 131.0;

  Serial.println("Gyro calibrated. Neutral values:");
  Serial.print("X: "); Serial.println(neutralX, 4);
  Serial.print("Y: "); Serial.println(neutralY, 4);
  delay(500);
}

void loop() {
  // Read raw gyro values
  float gyroX = readGyro(GYRO_XOUT_H) / 131.0 - neutralX;
  float gyroY = readGyro(GYRO_YOUT_H) / 131.0 - neutralY;

  // Apply simple low-pass filter to reduce jitter
  float filteredX = prevX * (1.0 - ALPHA) + gyroX * ALPHA;
  float filteredY = prevY * (1.0 - ALPHA) + gyroY * ALPHA;
  prevX = filteredX;
  prevY = filteredY;

  // Map to -1 → +1
  float mappedX = mapToMinus1Plus1(filteredX);
  float mappedY = mapToMinus1Plus1(filteredY);

  // Send to Unity
  Serial.print(mappedX, 4);
  Serial.print("|");
  Serial.println(mappedY, 4);

  delay(50);
}

// Maps a value from MIN_GYRO → MAX_GYRO to -1 → 1
float mapToMinus1Plus1(float value) {
  float n = (2.0f * (value - MIN_GYRO) / (MAX_GYRO - MIN_GYRO)) - 1.0f;
  if (n < -1) n = -1;
  if (n > 1) n = 1;
  return n;
}

// Reads a 16-bit gyro value from the sensor
int16_t readGyro(uint8_t reg) {
  Wire.beginTransmission(GYRO_ADDRESS);
  Wire.write(reg);
  Wire.endTransmission(false);

  Wire.requestFrom(GYRO_ADDRESS, 2);
  return (Wire.read() << 8) | Wire.read();
}
