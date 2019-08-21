using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristmasServer.Animations {
    interface IAnimation {
        void playAnimation();
        int getBranchCount();
        int getLength();
    }
}
