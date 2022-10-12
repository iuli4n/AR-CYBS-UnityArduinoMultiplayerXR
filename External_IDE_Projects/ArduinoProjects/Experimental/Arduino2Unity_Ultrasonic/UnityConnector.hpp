#include "Arduino.h"

// ----------- UNITY CHANNELS ----------------------
// ----------- UNITY CHANNELS ----------------------

const int MAX_CHANNELS = 5;

typedef void FuncSVoid(String&); 
typedef void FuncSInt(String&, int); 


// database of channel names and variables
String inputChannelNames[MAX_CHANNELS];
int* inputChannelVariables[MAX_CHANNELS];
String outputChannelNames[MAX_CHANNELS];
int* outputChannelVariables[MAX_CHANNELS];

// tracks how many inputs/outputs we used
int iinputs=0;
int ioutputs=0;

// register a new input channel and variable
void SERIALU_AddInputChannel(String channelName, int* channelVar) {
  inputChannelNames[iinputs]=channelName; 
  inputChannelVariables[iinputs]=channelVar; 
  iinputs++; 
}
// register a new output channel and variable
void SERIALU_AddOutputChannel(String channelName, int* channelVar) {
  outputChannelNames[ioutputs]=channelName; 
  outputChannelVariables[ioutputs]=channelVar; 
  ioutputs++; 
}


// send a channel to unity
void SERIALU_SendToUnity(String cname, int* val) {
  Serial.println(":"+cname+":"+String(*val));
}

// respond to receiving a new value, and set it in the database
void SERIALU_SimpleChannelResponderAll(String& channelName, int newVal) {
  for (int i=0; i<MAX_CHANNELS; i++) {
    if (inputChannelNames[i]!=NULL && inputChannelNames[i].equals(channelName)) {
      (*inputChannelVariables[i]) = newVal;
    }
  }
}
// send all the output channel variables
void SERIALU_SimpleChannelSenderAll() {
  for (int i=0; i<MAX_CHANNELS; i++) {
    if (outputChannelNames[i] != NULL) {
      SERIALU_SendToUnity(outputChannelNames[i], outputChannelVariables[i]);
    }
  }
}

// pointer to function that handles new values received for a channel
const FuncSInt* SERIALU_FNRESPONSE_NEWCHANVALUE = SERIALU_SimpleChannelResponderAll; 


// receive serial messages and parse them
// Format is :X:V where X is a 2-character channel (ex: "C2") and V is an integer value
// But a lot of the time the Unity library might send broken messages
void SERIAL_CheckSerialMessages() {
  String result = Serial.readStringUntil('\n');
  if (result.length() > 1) {
    Serial.println("Received: "+result+" ("+result.length()+")");

    if (result.startsWith(":")) {
      
      if (result.length()<=4 || !result.substring(3,4).equals(":")) {
        // bad message
        return;
      }
      
      String cname = result.substring(1,3); // get channel name
      String v = result.substring(4); // PERF: not new string
      int vi = v.toInt();
      
      // check if this substring is concatenated with others
      int spliti = v.indexOf(":");
      if (spliti >= 0) {
        // TODO: This happens if there's multiple messages concatenated together without split by \n. If that happens we take the first and lose the rest
        
        //Serial.println("ARDUINO Received bad message? --"+v+"----"+result);
        return;
        
        /***
        // attempts to salvage the message
        v = v.substring(0, spliti);
        
        if (v.length()<=0) {
          // bad message
          return;
        }
        
        vi = v.toInt();
        ***/
      }

      

      SERIALU_FNRESPONSE_NEWCHANVALUE(cname, vi);
    }
  }
}
