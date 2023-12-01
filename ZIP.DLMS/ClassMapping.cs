using Gurux.DLMS.Enums;
using Gurux.DLMS.Objects;
using System;
using System.Collections.Generic;

namespace ZIP.DLMS
{
    public static class ClassMapping
    {
        public static readonly Dictionary<ObjectType, Type> DLMSObjectTypeMapping = new Dictionary<ObjectType, Type>
        {
            { ObjectType.Data, typeof(GXDLMSData) },
            { ObjectType.Register, typeof(GXDLMSRegister) },
            { ObjectType.DemandRegister, typeof(GXDLMSDemandRegister) },
            { ObjectType.ExtendedRegister, typeof(GXDLMSExtendedRegister) },
            /// ...
            { ObjectType.Arbitrator,  typeof(GXDLMSArbitrator)}
        };
    }
}
