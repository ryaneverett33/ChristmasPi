using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server {
        public class Head {
        public double ver { get; set; }
        public int request { get; set; }
        public string type { get; set; }
    }

    public class Login {
        public string password { get; set; }
        public int machineid { get; set; }
    }

    public class Animation {
        public string name { get; set; }
        public bool play { get; set; }
    }

    public class Branch {
        public int index { get; set; }
        public bool status { get; set; }
    }

    /*public class Branches {
        
    }*/

    public class Data {
        public Login login { get; set; }
        public Animation animation { get; set; }
        public List<Branch> branches { get; set; }
        public int star { get; set; }
    }

    public class Client {
        public int machineid { get; set; }
        public string ip { get; set; }
        public string name { get; set; }
    }

    public class Document {
        public Head head { get; set; }
        public Data data { get; set; }
        public Client client { get; set; }
    }
}
