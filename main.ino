#define moter1_1_EN 3
#define moter1_2_EN 5
#define moter2_1_EN 10
#define moter2_2_EN 11

#define moter1_1_IN 2
#define moter1_2_IN 4
#define moter2_1_IN 12
#define moter2_2_IN 13

void setup() {
  analogWrite(moter1_1_EN, 0);
  analogWrite(moter1_2_EN, 0);
  analogWrite(moter2_1_EN, 0);
  analogWrite(moter2_2_EN, 0);

  pinMode(moter1_1_IN, OUTPUT);
  pinMode(moter1_2_IN, OUTPUT);
  pinMode(moter2_1_IN, OUTPUT);
  pinMode(moter2_2_IN, OUTPUT);

  digitalWrite(moter1_1_IN, HIGH);
  digitalWrite(moter1_2_IN, LOW);
  digitalWrite(moter2_1_IN, LOW);
  digitalWrite(moter2_2_IN, HIGH);
}

void loop() {
  analogWrite(moter1_1_EN, 127);
  analogWrite(moter1_2_EN, 127);
  analogWrite(moter2_1_EN, 127);
  analogWrite(moter2_2_EN, 127);
  while(1);
}
