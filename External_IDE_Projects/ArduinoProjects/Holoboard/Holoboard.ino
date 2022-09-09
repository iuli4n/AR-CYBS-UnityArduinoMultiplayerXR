#include "Arduino.h"
#include "UnityConnector.hpp"

// outputs to unity
int C1,C2;

void setup() {

  pinMode(13, OUTPUT);
  
  pinMode(A0, INPUT);
  pinMode(A2, INPUT);

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
  
}
