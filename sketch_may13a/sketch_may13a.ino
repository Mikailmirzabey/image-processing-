#include <Servo.h>

Servo servo1;
int inbyte=0;
int i;
void setup(){
  servo1.attach(2);
  Serial.begin(9600);
  Serial.println("Start!");
  
  
  

}

void loop(){
  if(Serial.available()>0){
    inbyte=Serial.read();
        
  }
  Serial.println(inbyte);
     servo1.write(map(inbyte,0,320,0,240));
     delay(1000);
}

