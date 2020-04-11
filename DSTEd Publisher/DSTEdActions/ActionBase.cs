using System;
using System.Collections.Generic;
using System.Text;

using static DSTEd.Publisher.SteamWorkshop.Steam;
namespace DSTEd.Publisher.DSTEdActions
{
    internal abstract class ActionBase
    {
        public abstract ExitCodes Do(object dataObject);

        public abstract string Name { get; }
        public abstract Type DataType { get; }
        public object ResultObject;
        protected ExitCodes ExitCode;
        protected bool AllAsyncOperationsFinished;
    }
}
