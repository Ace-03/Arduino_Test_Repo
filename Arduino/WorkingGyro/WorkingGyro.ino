#include <Wire.h>

// Replace with your gyro's I2C address (MPU6050 defaults to 0x68)
#define GYRO_ADDRESS 0x68

// Gyroscope register addresses for MPU6050
#define GYRO_XOUT_H 0x43
#define GYRO_YOUT_H 0x45

// Expected gyro range (adjust if needed)
const float MIN_GYRO = -250.0f;
const float MAX_GYRO =  250.0f;

void setup() {
  Serial.begin(9600);
  Wire.begin();

  // Wake up the MPU6050
  Wire.beginTransmission(GYRO_ADDRESS);
  Wire.write(0x6B);
  Wire.write(0x00);
  Wire.endTransmission(true);

  delay(100);
}

void loop() {
  // Read raw gyro values
  int16_t rawX = readGyro(GYRO_XOUT_H);
  int16_t rawY = readGyro(GYRO_YOUT_H);

  // Convert raw to degrees/sec
  const float gyroScale = 131.0;
  float gyroX = rawX / gyroScale;
  float gyroY = rawY / gyroScale;

  // Normalize to -1 → +1
  float mappedX = mapToMinus1Plus1(gyroX);
  float mappedY = mapToMinus1Plus1(gyroY);

  // Convert floats to strings safely
  char xStr[10];
  char yStr[10];

  dtostrf(mappedX,  1, 4, xStr); // (value, min width, decimals, buffer)
  dtostrf(mappedY,  1, 4, yStr);

  // Build final message
  char message[32];
  strcpy(message, xStr);
  strcat(message, "|");
  strcat(message, yStr);
  strcat(message, "\n");

  // Send the safe message
  Serial.print(message);

  delay(50);
}

// Maps a value in MIN_GYRO → MAX_GYRO to -1 → 1
float mapToMinus1Plus1(float value) {
  float n = (2.0f * (value - MIN_GYRO) / (MAX_GYRO - MIN_GYRO)) - 1.0f;

  // Clamp
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
