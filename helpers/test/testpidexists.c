#include <stdio.h>

extern int pidexists(int pid);

int main() {
    printf("pidexists(0): %d\n", pidexists(0));
}