using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasServer {
    /// <summary>
    /// Describes a gpio pin on the board
    /// </summary>
    class gpio_pin {
        public static readonly int statusOn = 1;
        public static readonly int statusOff = 0;
        public gpio_pin(int status,  int gpioPin, int branch, int listAddress) {
            this.status = status;
            this.gpioPin = gpioPin;
            this.branch = branch;
            this.listAddress = listAddress;
        }
        //Whether the pin is active or not
        public int status { get; set; }
        //The actual pin number
        public int gpioPin { get; set; }
        //What branch does this pin control?
        public int branch { get; set; }
        //Where is this pin description within gpio::pins
        public int listAddress { get; set; }
    }
}
