#include <Ultrasonic.h>
#include <OneWire.h>
#include <SPI.h>
#include <Ethernet.h>

//http://arduinobasics.blogspot.co.nz/2012/11/arduinobasics-hc-sr04-ultrasonic-sensor.html

//Ultrasonic
#define echoPin 5 // Echo Pin
#define trigPin 6 // Trigger PinM
int maximumRange = 5000; // Maximum range needed
int minimumRange = 0; // Minimum range needed
long duration, distance; // Duration used to calculate distance
boolean debug=false;

//Temperature DS18B20 pin
OneWire ds(2);

//Webserver endpoints
char server[] = "waterlevelweb.azurewebsites.net";  //Azure Web App
String endpoint = "/logger.aspx";

//Ethernet
byte mac[] = { 0xFE, 0xED, 0x00, 0x00, 0xBE, 0xEF };
IPAddress ip(192, 168, 0, 3);   // Set the static IP address to use if the DHCP fails to assign
EthernetClient client;

void setup() {
  Serial.begin(9600);
  Serial.println("System booting...");
  if (Ethernet.begin(mac) == 0) {
    Serial.println("Failed to configure Ethernet using DHCP");
    // try to congifure using IP address instead of DHCP:
    Ethernet.begin(mac, ip);
  }
  Serial.print  ("IP: ");
  Serial.println (Ethernet.localIP());

  // give the Ethernet shield a second to initialize:
  delay(1000);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);

}

void loop(void) {
  double temp;
  int dist;
  
  getOneWireTemp(&temp);
  dist = getDistance();

  Serial.print ("Temp:  ");
  Serial.print (temp);
  Serial.print ("  Distance: ");
  Serial.println(dist);
  
  LogData(String(temp), String(dist));

  delay(90000); //Delay loop for each datapoint log.

}

void LogData(String temperature, String distance) {

  // if you get a connection, report back via serial:
  if (client.connect(server, 80)) {
    Serial.println ("Connected, logging.....  ");
    
    String postData = "level=" + distance + "&temp=" + temperature + "&uid=RandomStringNeedsToBeTheSameOnArduinoAndInVisualStudio";
    String endpointData = "POST " + endpoint + " HTTP/1.1";
    String hostData = "Host: " + String(server);
       
    client.println(endpointData);
    client.println(hostData);
    client.println("Cache-Control: no-cache");
    client.println("Connection: close");
    client.println("Content-Type: application/x-www-form-urlencoded");
    client.print  ("Content-Length: ");
    client.println(postData.length());

    client.println();
    client.println(postData);
    client.stop();
    
    if(debug)
    {
      Serial.println ("    Post Data: " + postData);
      Serial.println ("EndPoint Data: " + endpointData);
      Serial.println ("    Host Data: " + hostData);
      Serial.println ("   Web Server: " + String(server));
      Serial.println (" ");
    }
    
  }
  else {
    // kf you didn't get a connection to the server:
    Serial.println("connection failed");
    delay(50);
    void(* resetFunc) (void) = 0; //declare reset function @ address 0
    resetFunc();  //call reset
  }
}


int getDistance() {
  /* The following trigPin/echoPin cycle is used to determine the
  distance of the nearest object by bouncing soundwaves off of it. */
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);

  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);

  digitalWrite(trigPin, LOW);
  duration = pulseIn(echoPin, HIGH);

  //Calculate the distance (in mm) based on the speed of sound.
  distance = duration / 5.82;  //mm not too accurate, to return CM, use 58.2

  if (distance >= maximumRange || distance <= minimumRange) {
    /* Send a negative number to computer to indicate "out of range" */
    return -1;
  }
  else {
    return distance;
  }

}

boolean getOneWireTemp(double *retnTemp) {
  byte present = 0;
  byte data[12];
  byte addr[8];
  int raw;
  double temp;

  if ( !ds.search(addr)) {
    //no more sensors on chain, reset search
    ds.reset_search();
    return false;
  }

  if ( OneWire::crc8( addr, 7) != addr[7]) {
    Serial.println("CRC is not valid!");
    return false;
  }

  if ( addr[0] != 0x10 && addr[0] != 0x28) {
    Serial.print("Device is not recognized");
    return false;
  }

  ds.reset();
  ds.select(addr);
  ds.write(0x44, 1);        // start conversion, with parasite power on at the end

  present = ds.reset();
  if (present)  //only continue if the sensor is actually there and responding
  {
    ds.select(addr);
    ds.write(0xBE);         // Read Scratchpad which now contains the temperature data
    for ( int i = 0; i < 9; i++) {           // we need 9 bytes
      data[i] = ds.read();
    }
    if ( OneWire::crc8( data, 8) != data[8]) {
      //Serial.println("CRC is not valid!\n");
      return false;
    }

    ds.reset_search();

    raw = (data[1] << 8) + data[0]; //put the two bytes of the temperature (from the response) into a raw int
    *retnTemp = (double)raw * 0.0625; //convert to celcius
    //  *retnTemp=temp*1.8+32;//convert to fahrenheit and set the final return value
    return true;
  }
  return false;
}




