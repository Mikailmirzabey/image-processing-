#include <Servo.h>

Servo servo1;
int inbyte=0;
double i;
void setup(){
 
  servo1.attach(2);
  Serial.begin(9600);
  Serial.println("Start!");
  
  servo1.write(90);
}


void loop(){
 
  if(Serial.available()>0)
  {
    inbyte=Serial.read();
    i=map(inbyte,0,320,0,240);
  }
    servo1.write(i);
    Serial.print(i);
    delay(1000);

}

