/**
 * CYB Arduino-Unity example
 * 
 * Arduino reads data from a button / potentiometer / ultrasonic sensor and sends that to Unity on channel C2
 * 
 * Unity sends data on C1, which is then used to turn the onboard LED on/off. 
 * 
 * Arduino sends back C4 which is half of C1
 * 
 */


#include "Arduino.h"
#include "UnityConnector.hpp"

#include <Servo.h>
Servo myservo;  // create servo object to control a servo
int prevServoVal = 0;

// ultrasonic stuff
#define echoPin 12 // attach pin D2 Arduino to pin Echo of HC-SR04
#define trigPin 13 //attach pin D3 Arduino to pin Trig of HC-SR04


// channels with unity
int C1,C2,C3;

void setup() {

  // ultrasonic setup
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);

  // servo setup
  myservo.attach(3);

  pinMode(13, OUTPUT);
  
  pinMode(A0, INPUT);
  pinMode(A2, INPUT);

  SERIALU_AddInputChannel("C3",&C3);
  
  SERIALU_AddOutputChannel("C1",&C1);
  SERIALU_AddOutputChannel("C2",&C2);
  
  // begin serial communication
  Serial.begin(9600);
  Serial.setTimeout(100);
}

int ultrasonicRead() {
  long duration; // variable for the duration of sound wave travel
  int distance; // variable for the distance measurement

  // Clears the trigPin condition
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin HIGH (ACTIVE) for 10 microseconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH);
  // Calculating the distance
  distance = duration * 0.034 / 2; // Speed of sound wave divided by 2 (go and back)
  
  return distance;
}

void loop()
{
  // receive from unity & send data to unity
  SERIAL_CheckSerialMessages();  
  SERIALU_SimpleChannelSenderAll(); 

  
  C1 = analogRead(A0);
  //C2 = analogRead(A2);
  
  float distance = ultrasonicRead();
  // noise cancel
  int preDist = distance * 5;
  if (preDist > 1024 || preDist< 5) 
    C2 = C2; // keep it as is
  else
    C2 = distance * 5;

  

  int val = map(C3, 0, 1023, 0, 180);     // scale it to use it with the servo (value between 0 and 180)
  if (prevServoVal != val) {
    myservo.write(val);
    prevServoVal = val;
  }
  
}
