using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasServer {
    public class ConfigFile {
        public int pin_count { get; set; }
        public int server_port { get; set; }
        public List<Pin> pins { get; set; }
    }
    public class Pin {
        public int gpio_pin { get; set; }
        public int branch { get; set; }
        public int status { get; set; }
    }
}
