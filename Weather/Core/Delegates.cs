using System;

namespace Weather.Core
{
    public delegate void StringEventHandler(object sender, string value);
    public delegate void ExceptionEventHandler(object sender, Exception exception);
}
