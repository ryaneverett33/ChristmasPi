using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class BackgroundActivity {
        long timeOfActivity;
        const long TIMEOUT = 9000000000;
        bool workerAlive;
        bool inActive;

        //Objects
        Logger log;
        Manager man;

        DateTime dt = new DateTime();
        public BackgroundActivity(Logger log, Manager man) {
            this.log = log;
            this.man = man;
            timeOfActivity = dt.Ticks;
            workerAlive = true;
            inActive = false;
        }
        public bool timedOut() {
            if ((dt.Ticks - timeOfActivity) > TIMEOUT) {
                return true;
            }
            return false;
        }
        public void updateActivity() {
            this.timeOfActivity = dt.Ticks;
        }
        public void Worker() {
            while (workerAlive) {

            }
        }
    }
}
