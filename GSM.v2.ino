//Это код для проекта теплого пола для даче, по задумки этим теплым полом можно было бы упровлять через СМС блогадря GSM модему
//Библиотекти 
#include <iarduino_DHT.h>
#include <SoftwareSerial.h>
#include <GPRS_Shield_Arduino.h>
#include <String.h>
#include <EEPROM.h>

//объявляем GPRS и DHT
SoftwareSerial Serial1(7, 8);//Tx, Rx
GPRS gprs(Serial1);
iarduino_DHT sensor(3);

//Переменые 
int rele = 2;
int dhtPin =3; 
char message[160];
char phone[16];
char datetime[24];
#define tel "+79519761227"
String com ="";
String tstr = "";
int l=0;
int t;
float EEMEM t_addr;
char strChar[100];
int pz;
String pzstr;
//***************************************

void setup() {
  //Инцилизация Serial порта  
  Serial.begin(9600);
  Serial.print("Serial init OK\r\n");
  
 //Инцилизация Реле и GSM
  pinMode(rele,OUTPUT);
  digitalWrite(rele, HIGH);
  Serial1.begin(9600);
  gprs.powerOn();
  
 //Установка связи с GSM
   while (!gprs.init()) {
      delay(1000);
      //Провал 
      Serial.print("GPRS Init error\r\n");}
 //Успех
  Serial.println("GPRS init success");
  Serial.println("Please send SMS message to me!\nИнцилизация завершина");
  Serial.println();
//все готово!
}

//*************************************
 
void loop() {
  //Получаем SMS
 if (gprs.ifSMSNow()) {
    //Считываем даные 
    gprs.readSMS(message, phone, datetime);
    //Выводим 
    Serial.print("From number: ");
    Serial.println(phone);
    Serial.print("Datetime: ");
    Serial.println(datetime);
    Serial.print("Recieved Message: ");
    Serial.print(message);
    Serial.print("(");
    String com(message); 
    Serial.print(com);
    Serial.println(")");

//Команды

//Help
if(com.indexOf("Help", 0)!=-1){
  Serial.println("***Help***\n");
     //Сборка и отправка ответа 
    String str = "Help-spisoc comand\nInf-obshaua informaciua\nTemp-ystanovca temp\nDla podrobnou inf. pishi \nTemp?";
    memset(strChar, 0, 100);
    str.getBytes((unsigned char *)strChar, 99);  
    gprs.sendSMS(phone, strChar);}
//Temp?
if(com=="Temp?"){
    Serial.println("***Temp?***\n");
     //Сборка и отправка ответа 
    String str = "t-predpochitaemua temperatyra\n\nvi>Temp\nvi>\"t\"\nI>Ok\nTemp=\"t\"";
    memset(strChar, 0, 100);
    str.getBytes((unsigned char *)strChar, 99);  
    gprs.sendSMS(phone, strChar);}
  




//inf-запрос информации
    if(com.indexOf("Inf", 0)!=-1){
      Serial.println("***Inf***");
      
      //Сборка строки
      String str = (String) sensor.tem;
      
      for(int i=0; i!=3;i++){
         EEPROM.get(i*2, t);
         EEPROM.get(6, pz);
         String tstr = String(t,DEC);
         str =str+"\nTemp"+(String) i+"= "+ tstr;
         if(pz==i){str+="*";}}
      //перевод из String в Char
      int n = str.length();
      memset(strChar, 0, 50);
      str.getBytes((unsigned char *)strChar, 99);
      
      //Отправка
      gprs.sendSMS(phone, strChar);
      
      //Выводим то что отправили 
      Serial.print(">>>");
      Serial.println(str);
      Serial.println();}


 
    

//Установка Temp  
//Отслеживаем Temp 
  if(-1!=com.indexOf("Temp")){ 
//Ищим и передаем pz
pzstr=com.substring(com.indexOf("Temp")+4);
pz=pzstr.toInt(); 
 
 //Ищим значение Temp и передаем его в tstr
 tstr=com.substring(com.indexOf("Temp")+5);

    //Установка 
    t=tstr.toInt();
    EEPROM.put(pz*2,t); 
    
    //Отправка в Serial
    Serial.print(">Temp=");
    Serial.println(t);
    Serial.println(); 
    
    //Сборка и отправка ответа 
    String str = "Ok\nTemp"+(String) pz +"="+(String) t;
    memset(strChar, 0, 50);
    str.getBytes((unsigned char *)strChar, 99);  
    gprs.sendSMS(phone, strChar);
    }


    
    //Temp_ on
    if(com.indexOf("T")!=-1){
      if(com.indexOf("on")!=-1){
    Serial.println("***Temp_ on***");
    
    //Ищим и передаем pz
    if(com.indexOf("p")!=-1){
    pzstr=com.substring(com.indexOf("Temp")+4);
    }else{pzstr=com.substring(com.indexOf("T")+1);}
    
    pz=pzstr.toInt();
    EEPROM.put(6,pz);
     String str = "";
      //Собираем ответ 
      for(int i=0; i!=3;i++){
         EEPROM.get(i*2, t);
         EEPROM.get(6, pz);
         String tstr = String(t,DEC);
         str =str+"\nTemp"+(String) i+"="+ tstr;
         if(pz==i){str+="*";}}
         //переводим в char 
        memset(strChar, 0, 50);
        str.getBytes((unsigned char *)strChar, 99);
         
    gprs.sendSMS(phone, strChar);
    }}}
    

    
    //Постороние действие 
    
    //Обновляем/получаем даные
    
    EEPROM.get(6, pz);
    EEPROM.get(pz*2, t);
    //Вывод информации 
    sensor.read();
    Serial.print("Temp:");
    Serial.print(sensor.tem);
    Serial.print(";");
    Serial.print(pz);
    Serial.print(".");
    Serial.print(t);
    Serial.println("<");

    
    ///Проверка сериал пройти
    if(Serial.available() > 0){
        Serial.println("Ok");
        com = Serial.readString();
        if(com.indexOf("Inf:") != -1){
      Serial.print("Inf:");
      
      //Сборка строки
      String str = (String) sensor.tem;
      
      for(int i=0; i!=3;i++){
         EEPROM.get(i*2, t);
         EEPROM.get(6, pz);
         str = str+(String) t + ";";}
          
      str = str + (String) pz + "<";
      Serial.println(str);
          
          }
      }




    
    //Проверка реле 
    if(t<(int)sensor.tem){
      digitalWrite(rele, HIGH);}
    else{
      digitalWrite(rele, LOW);}
   //Тайминг
    delay(500); 
    }
