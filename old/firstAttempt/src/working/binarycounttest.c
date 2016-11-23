#define _BSD_SOURCE
#include <stdlib.h>
#include <stdio.h>
#include <stdbool.h>
#include <unistd.h>


int main()
{
    int current[6];
    int length;

    for(int i=0;i<64;i++)
    {
        printf("Original: %i \n",i);
//        if(x2==0)
//        {
//            puts("remainder is 0");
//        }
//        else {
//            printf("Remainder is: %i \n",x2);
//        }
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
            printf("Number: %i%i%i%i%i%i \n",current[0],current[1],current[2],current[3],current[4],current[5]);
            length = 6;
            printf("Length: %i \n",length);
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
            printf("Number: %i%i%i%i%i \n",current[0],current[1],current[2],current[3],current[4]);
            length = 5;
            printf("Length: %i \n",length);
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
            printf("Number: %i%i%i%i \n",current[0],current[1],current[2],current[3]);
            length = 4;
            printf("Length: %i \n",length);
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
            printf("Number: %i%i%i \n",current[0],current[1],current[2]);
            length = 3;
            printf("Length: %i \n",length);
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
            printf("Number: %i%i \n",current[0],current[1]);
            length = 2;
            printf("Length: %i \n",length);
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
            printf("Number: %i \n",current[0]);
            length = 1;
            printf("Length: %i \n",length);
        }
        usleep(500000);
    }
}
