using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChristmasPi.Data.Models {
    #region Delegates
    public delegate void BeforeRenderHandler(object sender, RenderArgs args);
    public delegate void AfterRenderHandler(object sender, RenderArgs args);
    #endregion
    #region Argument Objects
    public class RenderArgs : EventArgs {

    }
    #endregion
}
