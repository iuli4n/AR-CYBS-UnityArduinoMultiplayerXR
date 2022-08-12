#include "Arduino.h"
#include "UnityConnector.hpp"

// This example receives channels C1, C3 from unity and sends C4, C5 to unity
//    The value of input C3 influences the arduino onboard LED (turns on if >500)
//    The value of output C4 is half the value of C3
//    The value of output C5 is incremented by this sketch and reset periodically
// Messages sent/received to Unity are in the format ":X:V" where X=channel id 2 characters (ex: "C4") and V=integer value


// outputs to unity
int C3;




#define echoPin 3 // attach pin D2 Arduino to pin Echo of HC-SR04
#define trigPin 2 //attach pin D3 Arduino to pin Trig of HC-SR04

// defines variables
long duration; // variable for the duration of sound wave travel
int distance; // variable for the distance measurement




void setup() {
  // onboard led will be for output
  pinMode(13,OUTPUT);

  // ultrasonic
  pinMode(trigPin, OUTPUT); // Sets the trigPin as an OUTPUT
  pinMode(echoPin, INPUT); // Sets the echoPin as an INPUT
 
  // setup channels and variable bindings
  //SERIALU_AddInputChannel("C1",&C1);
  //SERIALU_AddInputChannel("C3",&C3);
  //SERIALU_AddOutputChannel("C4",&C4);
  SERIALU_AddOutputChannel("C3",&C3);
  
  // begin serial communication
  Serial.begin(9600);
  Serial.setTimeout(100);
}

int TEMP_count = 0;

void loop()
{

  loop_ultrasonic();
  
  // C3 value controls LED
  //digitalWrite(13, C3 > 500 ? HIGH : LOW);
  
  // C4 is half C3
  //C4 = C3/2;

  // C5 gets incremented
  //C5 = (TEMP_count++ % 50) * 10;


  // receive from unity & send data to unity
  SERIAL_CheckSerialMessages();  
  SERIALU_SimpleChannelSenderAll(); 
}

void loop_ultrasonic() {
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

  // noise cancel
  int preC3 = C3;
  C3 = distance * 5;
  if (C3 > 1024 || C3 < 5) C3 = preC3 ;
}
