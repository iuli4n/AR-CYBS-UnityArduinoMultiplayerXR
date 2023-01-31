#include "Arduino.h"
#include "UnityConnector.hpp"

// DESCRIPTION: 
// This example receives channels C1 from unity and sends C2, C4 to unity
//    The value of C1 (from Unity) influences the arduino onboard LED (on if >500)
//    The value of C2 (to Unity) is what is read on A0
//    The value of C3 (to Unity) is half the value of C1
//
// WIRING:
// Follow this wiring diagram: https://www.arduino.cc/en/Tutorial/BuiltInExamples/AnalogReadSerial
//
// NOTE:
// Messages sent/received to Unity are in the format ":X:V" where X=channel id 2 characters (ex: "C4") and V=integer value

// these variables will hold the channel values
int C1,C2,C3;

void setup() {
  // setup channels and variable bindings
  SERIALU_AddInputChannel("C1",&C1);
  SERIALU_AddOutputChannel("C2",&C2);
  SERIALU_AddOutputChannel("C3",&C3);

  // onboard led will be for output
  pinMode(13,OUTPUT);
  // A0 is for reading
  pinMode(A0,INPUT);
  
  // begin serial communication
  Serial.begin(9600);
  Serial.setTimeout(100);
}

void loop()
{  
  // LED is set according to C1
  digitalWrite(13, C1 > 500 ? HIGH : LOW);

  // C2 comes from A0
  C2 = analogRead(A0);
  
  // C3 is half C1
  C3 = C1/2;

  // sync any new updates from/to unity
  SERIALU_ProcessMessages();
}
