#include "Arduino.h"
#include "UnityConnector.hpp"

// This example receives channels C1, C3 from unity and sends C4, C5 to unity
//    The value of input C3 influences the arduino onboard LED (turns on if >500)
//    The value of output C4 is half the value of C3
//    The value of output C5 is incremented by this sketch and reset periodically
// Messages sent/received to Unity are in the format ":X:V" where X=channel id 2 characters (ex: "C4") and V=integer value


// inputs from unity
int C3;
// outputs to unity
int C4,C5;

void setup() {
  // onboard led will be for output, A0 for input
  pinMode(13, OUTPUT);
  pinMode(A0, INPUT);

  // setup channels and variable bindings
  SERIALU_AddInputChannel("C3",&C3);
  SERIALU_AddOutputChannel("C4",&C4);
  SERIALU_AddOutputChannel("C5",&C5);
  
  // begin serial communication
  Serial.begin(9600);
  Serial.setTimeout(100);
}

int TEMP_count = 0;

void loop()
{
  // receive from unity & send data to unity
  SERIAL_CheckSerialMessages();  
  SERIALU_SimpleChannelSenderAll(); 


  // Now take action based on whatever we recived
  
  // C3 value controls LED
  digitalWrite(13, C3 > 500 ? HIGH : LOW);
  
  // C4 is half C3
  C4 = C3/2;

  // C5 is the value of input A0
  C5 = analogRead(A0);
  
}
