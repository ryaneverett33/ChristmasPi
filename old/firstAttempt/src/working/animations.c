#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include <string.h>
#include <pthread.h>
#include <time.h>
#include <wiringPi.h>

const char *animation[8] = {"","Twinkle","Toggle Each Branch","Flash","Staircase","Mirage","Binary","All Off"};
//Arrays start at zero, but integers start at 1. Normalizing array distribution for ease. Eg: Branch1 = branch[1]
int branch[7] = {-1,3,7,2,0,6,4};
bool firstrun;
int train = 5;      //Not used, but kept anyway
int star = 1;       //Same as train
pthread_t thread1;
int animthread; //thread integer for animation manager
bool quit = false;

int fn;
void turnon(int pin)
{
    digitalWrite(pin,LOW);
}
void turnoff(int pin)
{
    //Sending 3V3 to the relay turns it off, sending 0V turns it on.
    //Inverted logic, trying to simplify for future use
    digitalWrite(pin,HIGH);
}
void alloff()
{
    int pins[8] = {1,4,5,6,7,0,2,3};
    for(int i=0;i<9;i++)
    {
        //Set pins to off
        turnoff(pins[i]);
    }
}
void allon()
{
    int pins[8] = {1,4,5,6,7,0,2,3};
    for(int i=0;i<9;i++)
    {
        //Set pins to off
        turnon(pins[i]);
    }
}

//Individual Animation Functions
void stop()
{
    for(int b=1;b<8;b++)
    {
        turnon(branch[b]);
    }
}
void twinkle()
{
    //pick random branch, turn it off
    srand(time(NULL));
    int r = rand() % 7;
    //printf("Random branch: %i",r);
    if(r==0)
    {
        //There is no branch 0
        r=1;
    }
    for(int b=1;b<8;b++)
    {
        if(b==r)
        {
            turnoff(branch[b]);
        }
        else
        {
            turnon(branch[b]);
        }
    }
    delay(1500);
}
void toggleeachbranch()
{
    alloff();
    for(int t=1;t<8;t++)
    {
        turnon(branch[t]);
        delay(500);
        turnoff(branch[t]);
        //printf("t = %i \n ",t);
    }
}
void flash()
{
    alloff();
    for(int t=1;t<8;t++)
    {
        turnon(branch[t]);
    }
    delay(500);
    for(int t=1;t<8;t++)
    {
        turnoff(branch[t]);
    }
    delay(500);
}
void staircase()
{
    alloff();
    turnon(branch[1]);
    turnon(branch[6]);
    delay(750);
    turnoff(branch[1]);
    turnoff(branch[6]);
    turnon(branch[2]);
    turnon(branch[5]);
    delay(750);
    turnoff(branch[2]);
    turnoff(branch[5]);
    turnon(branch[3]);
    turnon(branch[4]);
    delay(750);
    turnoff(branch[3]);
    turnoff(branch[4]);
    turnon(branch[2]);
    turnon(branch[5]);
    delay(750);
    turnoff(branch[2]);
    turnoff(branch[5]);
    turnon(branch[1]);
    turnon(branch[6]);
}
void mirage()
{
    alloff();
    for(int i=1;i<7;i++)
    {
        turnon(branch[i]);
        delay(750);
        turnoff(branch[i]);
    }
    for(int a=6;a>0;a--)
    {
        turnon(branch[a]);
        delay(750);
        turnoff(branch[a]);
    }
}
void binary()
{
    alloff();
    int current[6];
    for(int i=0;i<64;i++)
    {
        if(i>31)
        {
            //111111
            int x = i / 2;
            int x1 = i % 2;
            if(x1 == 0)
            {
                current[5] = 0;
            }
            else {
                current[5] = 1;
            }
            int y = x/2;
            int y1 = x%2;
            if(y1 == 0)
            {
                current[4] = 0;
            }
            else {
                current[4] = 1;
            }
            int z = y/2;
            int z1 = y%2;
            if(z1 == 0)
            {
                current[3] = 0;
            }
            else {
                current[3] = 1;
            }
            int w = z/2;
            int w1 = z%2;
            if(w1 == 0)
            {
                current[2] = 0;
            }
            else {
                current[2] = 1;
            }
            int a = w/2;
            int a1 = w%2;
            if(a1 == 0)
            {
                current[1] = 0;
            }
            else {
                current[1] = 1;
            }
            int b = a/2;
            int b1 = a%2;
            if(b1 == 0)
            {
                current[0] = 0;
            }
            else {
                current[0] = 1;
            }
            //printf("Number: %i%i%i%i%i%i \n",current[0],current[1],current[2],current[3],current[4],current[5]);
        }
        else if(i>15 && i<32)
        {
            //11111
            int x = i / 2;
            int x1 = i % 2;
            if(x1 == 0)
            {
                current[4] = 0;
            }
            else {
                current[4] = 1;
            }
            int y = x/2;
            int y1 = x%2;
            if(y1 == 0)
            {
                current[3] = 0;
            }
            else {
                current[3] = 1;
            }
            int z = y/2;
            int z1 = y%2;
            if(z1 == 0)
            {
                current[2] = 0;
            }
            else {
                current[2] = 1;
            }
            int w = z/2;
            int w1 = z%2;
            if(w1 == 0)
            {
                current[1] = 0;
            }
            else {
                current[1] = 1;
            }
            int a = w/2;
            int a1 = w%2;
            if(a1 == 0)
            {
                current[0] = 0;
            }
            else {
                current[0] = 1;
            }
            //printf("Number: %i%i%i%i%i \n",current[0],current[1],current[2],current[3],current[4]);
        }
        else if(i>7 && i<16)
        {
            //1111
            int x = i / 2;
            int x1 = i % 2;
            if(x1 == 0)
            {
                current[3] = 0;
            }
            else {
                current[3] = 1;
            }
            int y = x/2;
            int y1 = x%2;
            if(y1 == 0)
            {
                current[2] = 0;
            }
            else {
                current[2] = 1;
            }
            int z = y/2;
            int z1 = y%2;
            if(z1 == 0)
            {
                current[1] = 0;
            }
            else {
                current[1] = 1;
            }
            int w = z/2;
            int w1 = z%2;
            if(w1 == 0)
            {
                current[0] = 0;
            }
            else {
                current[0] = 1;
            }
            //printf("Number: %i%i%i%i \n",current[0],current[1],current[2],current[3]);
        }
        else if(i>2 && i<8)
        {
            //111
            int x = i / 2;
            int x1 = i % 2;
            if(x1 == 0)
            {
                current[2] = 0;
            }
            else {
                current[2] = 1;
            }
            int y = x/2;
            int y1 = x%2;
            if(y1 == 0)
            {
                current[1] = 0;
            }
            else {
                current[1] = 1;
            }
            int z = y/2;
            int z1 = y%2;
            if(z1 == 0)
            {
                current[0] = 0;
            }
            else {
                current[0] = 1;
            }
            //printf("Number: %i%i%i \n",current[0],current[1],current[2]);
        }
        else if(i>1)
        {
            //11
            int x = i / 2;
            int x1 = i % 2;
            if(x1 == 0)
            {
                current[1] = 0;
            }
            else {
                current[1] = 1;
            }
            int y = x/2;
            int y1 = x%2;
            if(y1 == 0)
            {
                current[0] = 0;
            }
            else {
                current[0] = 1;
            }
            //printf("Number: %i%i \n",current[0],current[1]);
        }
        else if(i<2 && i > -1)
        {
            //1
            if(i==1)
            {
                current[0] = 1;
            }
            else if(i==0)
            {
                current[0] = 0;
            }
            //printf("Number: %i \n",current[0]);
        }
        //Turn off branches that don't need to be on
        for(int q = 0;q<6;q++)
        {
            if(current[q] == 0) turnoff(branch[q+1]);
        }
        //Turn on branches that need to be on
        for(int q1 = 0;q1<6;q1++)
        {
            if(current[q1] == 1) turnon(branch[q1+1]);
        }
        delay(500);
    }
    //Reset current
    for(int cr = 0;cr<6;cr++)
    {
        current[cr] = 0;
    }
}

//{"Twinkle","Toggle Each Branch","Random Branches","Staircase","Mirage","Binary","All Off"};
void *AnimationManager()
{
    do
    {
        if(fn==0)
        {
            stop();
        }
        else if(fn==1)
        {
            twinkle();
        }
        else if(fn==2)
        {
            toggleeachbranch();
        }
        else if(fn==3)
        {
            flash();
        }
        else if(fn==4)
        {
            staircase();
        }
        else if(fn==5)
        {
            mirage();
        }
        else if(fn==6)
        {
            binary();
        }
        else if(fn==7)
        {
            alloff();
        }
    }
    while(fn > 0);
    quit = true;
    puts("Closing Animations.");
}
int SetAnimation(int anim)
{
    fn = anim;
    //{"","Twinkle","Toggle Each Branch","Flash","Staircase","Mirage","Binary","All Off"};
    switch(anim)
    {
        case 0:
            puts("Stop (All On)");
            break;
        case 1:
            puts("Twinkle");
            break;
        case 2:
            puts("Toggle Each Branch");
            break;
        case 3:
            puts("Flash");
            break;
        case 4:
            puts("Staircase");
            break;
        case 5:
            puts("Mirage");
            break;
        case 6:
            puts("Binary");
            break;
        case 7:
            puts("Stop (All Off)");
            break;
    }

    if(firstrun==true)
    {
        //If this is first run then we must create the thread
        animthread = pthread_create( &thread1, NULL, AnimationManager, NULL);
        puts("Thread created");
        //Avoid creating multiple threads
        firstrun = false;
    }
    else{

    }
    return 1;
}
void setup()
{
    //wiringPi handles failure for us
    wiringPiSetup();
    int pins[8] = {1,4,5,6,7,0,2,3};
    for(int i=0;i<9;i++)
    {
        //Set pins to OUTPUT
        pinMode(pins[i],OUTPUT);
    }
    for(int i=0;i<9;i++)
    {
        //Set pins to off
        turnoff(pins[i]);
    }
    puts("Type the number corresponding to the animation you wish to play.");
    puts("Animations:");
    for(int x=0;x<8;x++)
    {
        if(x==0)
        {
            puts("0 - Stop (All On)");
        }
        else if(x==7)
        {
            puts("7 - Stop (All Off)");
        }
        else {
            printf("%i - %s \n",x,animation[x]);
        }
    }
    puts("Setup() completed.");
    //Setup animation handler
}

int main(int argc,char *argv[])
{
    //No way to check if a write failed or not.
    firstrun = true;
    setup();
    //struct animInfo animInfo;
    int i = 1;
    do {
        puts("Enter a number.");
        scanf("%d", &i);
        if(i > -1)
        {
            SetAnimation(i);
        }
        else if(i < 0)
        {
            SetAnimation(i);
            quit = true;
        }
    }
    while(quit!=true);
    int res = pthread_join(thread1,NULL);
    if(res == 0)
    {
        //Checking if the thread successfully joins.
        //Mostly for paranoia reasons
        puts("Animations successfully closed.");
    }
    allon();
    return 1;
}
