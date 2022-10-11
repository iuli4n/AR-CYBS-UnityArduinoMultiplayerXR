// Requires Websockets library v0.1b by Lasse S.B. imported through the Sketch>Add Library menu

// This starts a websocket server and sends C1,C2,C3 to Unity through the websocket

import websockets.*;

int C1,C2,C3;

WebsocketServer ws;
int now;
float x,y;

void setup(){
  size(500,500);
  ws= new WebsocketServer(this,8317,"/");
  now=millis();
  x=0;
  y=0;
}

// Send the variables to the websocket, by first splitting them up into low/high bytes
void wsuSend() {
    int x1 = (int) (C1 & 0xFF);;
    int x2 = (int) ((C1 >> 8) & 0xFF);
    
    int y1 = (int) (C2 & 0xFF);;
    int y2 = (int) ((C2 >> 8) & 0xFF);
 
    int z1 = (int) (C3 & 0xFF);;
    int z2 = (int) ((C3 >> 8) & 0xFF);
 
   String msg = "{\"stream\":[0,"+x2+","+x1+","+y2+","+y1+","+z2+","+z1+",0,0]}"; 
   ws.sendMessage(msg);
   println(msg);
    
}

void draw(){
  background(0);
  ellipse(x,y,10,10);
  
  C1 = mouseX;
  C2 = mouseY;
  C3 = mouseX+mouseY;
  
  if(millis()>now+100){
    wsuSend();   
    now=millis();
  }
}

void webSocketServerEvent(String msg){
 println("RECEIVED FROM WEBSOCKET: "+msg);
 x=random(width);
 y=random(height);
}
