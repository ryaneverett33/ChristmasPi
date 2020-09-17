#include <sys/types.h>
#include <signal.h>
#include <errno.h>

// Checks if a pid exists
// Returns: 0 if it exists, 1 if it doesn't exist, -1 if error
int pidexists(int pid) {
    if (kill((pid_t)pid, 0) == -1) {
        if (errno == ESRCH)
            return 1;
        else
            return -1;
    }
    else {
        // kill returned zero so the process exists
        return 1;
    }
}