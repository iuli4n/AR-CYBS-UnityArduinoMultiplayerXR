#include "Arduino.h"
#include "UnityConnector.hpp"

#include <Servo.h>
Servo myservo;  // create servo object to control a servo



// channels with unity
int C1,C2,C3;

void setup() {

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


void loop()
{
  // receive from unity & send data to unity
  SERIAL_CheckSerialMessages();  
  SERIALU_SimpleChannelSenderAll(); 


  C1 = analogRead(A0);
  C2 = analogRead(A2);

  int val = map(C3, 0, 1023, 0, 180);     // scale it to use it with the servo (value between 0 and 180)

  myservo.write(val);
  
}
