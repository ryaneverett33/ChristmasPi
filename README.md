# ChristmasPi
A Christmas Tree powered by a Raspberry Pi. Specifically, a program written in C that allows animations to play. 
# Setup
There are six "branches", one star, and one train. The train and the star have not been set up, just connected to show the possibilty. Each branch is defined by a strand of 50 mini Christmas lights. These branches, the train, and the star all hook up to a [SainSmart Eight Channel Relay Board](http://www.amazon.com/dp/B0057OC5WK/). This relay board is connected to the Pi via the GPIO pins. The program is then run on the Pi and preferably controlled via SSH.
# Animations.c
### Functions
```C
void turnon(int pin)
```
Sends 0V to the relay as indicated by the pin argument. Turns on the relay.
```C
void turnoff(int pin)
```
Sends 3V3 to the relay as indicated by the pin argument. Turns off the relay.
```C
void alloff()
```
Turns off all branches, does not include the train and the star.
```C
void allon()
```
Turns on all branches, does not include the train and the star.
```C
void SetAnimation(int anim)
```
Changes the integer 'fn' to whatever animation needs to play next. If the function has not been executed yet, it starts the animation handler on another thread. If 'anim' equals '-1', then the function will terminate the execution of the animation handler.
```C
void *Animation()
```
Executes the appropriate animation function according to the integer 'fn'. It loops upon start and doesn't end until the program closes. 
