
#include <U8g2lib.h>
#include <U8x8lib.h>
U8G2_SSD1306_128X64_NONAME_F_SW_I2C u8g2(U8G2_R0, /* clock=*/ 15, /* data=*/ 4, /* reset=*/ 16);

#include <WiFi.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>

const char* ssid = "IULIALIEN";
const char* password = "11112222";
//const char* ssid = "IULI42";
//const char* password = "41862513";

bool ledState = 0;


// ESP32 OLED WiFi Kit onboard LED
#define LED_PIN 25
// ESP32 OLED WiFi Kit "PRG" button for input to programs
#define PRG_BUTTON_PIN 0

#define P2_PIN 35
#define P3_PIN 34
#define P4_PIN 33

// Create AsyncWebServer object on port 80
AsyncWebServer server(80);
AsyncWebSocket ws("/ws");

#include "Webserver.hh"

void setup() {

  // SET UP PINS
  analogReadResolution(10);
  pinMode(P2_PIN, INPUT);
  pinMode(P3_PIN, INPUT_PULLUP);
  pinMode(P4_PIN, INPUT_PULLUP);
  
  
  // SET UP DISPLAY

  // TODO:IR: switch to cursor caret https://github.com/olikraus/u8g2/blob/master/sys/arduino/u8g2_page_buffer/ScrollingText/ScrollingText.ino
  
  u8g2.begin();
  u8g2.setFont(u8g2_font_6x12_mf); // fairly small font
  u8g2.setFontRefHeightExtendedText();
  u8g2.setDrawColor(1); // normal, not inverted
  u8g2.setFontPosTop(); // x,y is at top of font
  u8g2.setFontDirection(0); // not rotated
  
  u8g2.drawStr(0, 0, "It Works! Connecting to wifi...");
  u8g2.sendBuffer();

  // SERIAL PORT
  Serial.begin(115200);

  // PINS
  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);
  pinMode(PRG_BUTTON_PIN, INPUT);
  
  // WIFI
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi..");
  }

  // Print ESP Local IP Address
  Serial.println(WiFi.localIP());

  u8g2.clear();
  u8g2.drawStr(0, 0, WiFi.localIP().toString().c_str());
  u8g2.sendBuffer();
  
  // WEB SERVER
  
  initWebSocket();

  // Route for root / web page
  /*
  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send_P(200, "text/html", index_html, processor);
  });
  */
  
  // Start server
  server.begin();
  
}

int lastPRG = 0;


int P1 = 0;
int P2 = 0;
int P3 = 0;
int P4 = 0;

void wsSendAllSensors() {
  String msg = "";
  msg += "burst::";
  msg += ""+String(P1);
  msg += "-"+String(P2);
  msg += "-"+String(P3);
  msg += "-"+String(P4);
  
  ws.textAll(msg);
    
}

int nextUpdate;

void loop() {

  digitalWrite(LED_PIN, ledState);


  if (millis() < nextUpdate) {
    return;
  }
  nextUpdate = millis() + 600;

  
  int p = digitalRead(PRG_BUTTON_PIN);
  if (p != lastPRG) {
    P1 = p * 1000;
    lastPRG = p;
  }

  P2 = analogRead(P2_PIN); delay(1);
  P3 = analogRead(P3_PIN); delay(1);
  P4 = analogRead(P4_PIN); delay(1);
  
  wsSendAllSensors();
}
