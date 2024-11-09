using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSL.AST.Parsing.Preprocessing
{
    [Flags]
    public enum BslCompileContexts
    {
        None = 0b_00000000_00000000,

        MobileAppServer = 0b_00000000_00000001,
        MobileStandaloneServer = 0b_00000000_00000010,
        Server = 0b_00000000_00000011,

        ThinClient = 0b_00000000_00000100,
        MobileClient = 0b_00000000_00001000,
        WebClient = 0b_00000000_00010000,
        ThickClientManagedApplication = 0b_00000000_00100000,
        ThickClientOrdinaryApplication = 0b_00000000_01000000,
        MobileAppClient = 0b_00000000_10000000,
        Client = 0b_00000000_11111100,

        ExternalConnection = 0b_00000001_00000000,

        All = 0b_00000001_11111111,
    }
}
