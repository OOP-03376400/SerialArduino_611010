// the setup routine runs once when you press reset:
int i;
void setup() {
  // initialize serial communication at 9600 bits per second:
  Serial.begin(9600);
  i = 0;
}

// the loop routine runs over and over again forever:
void loop() {
  float something = millis()/1000.0;
  int value = 128.0 + 128 * sin( something * 2.0 * PI  );
 
  Serial.print(i);
  Serial.print(", ");
  Serial.println(value);
  i++;
  delay(50);        // delay in between reads for stability
}
