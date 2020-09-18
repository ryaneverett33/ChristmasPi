#include <stdio.h>

extern int pidexists(int pid);

int main() {
    printf("pidexists(60000): %d\n", pidexists(60000));
}
