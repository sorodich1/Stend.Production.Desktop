//
// --------------------------------------------------------------------------
//  Gurux Ltd
//
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details.
//
// More information of Gurux products: https://www.gurux.org
//
// This code is licensed under the GNU General Public License v2.
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using Gurux.DLMS.Objects.Enums;
using System;
#if ZIP
using Newtonsoft.Json;
using ZIP.DLMS;
#endif

namespace Gurux.DLMS.Objects
{
    public class GXDLMSIp4SetupIpOption
    {
        Ip4SetupIpOptionType type = Ip4SetupIpOptionType.Security;
        public Ip4SetupIpOptionType Type
        {
            get
            {
                return type;
            }
            set
            {
                switch (value)
                {
                    case Ip4SetupIpOptionType.Security:
                    case Ip4SetupIpOptionType.LooseSourceAndRecordRoute:
                    case Ip4SetupIpOptionType.StrictSourceAndRecordRoute:
                    case Ip4SetupIpOptionType.RecordRoute:
                    case Ip4SetupIpOptionType.InternetTimestamp:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Invalid Ip4 setup IP option type.");
                }
                type = value;
            }
        }

        public byte Length
        {
            get;
            set;
        }

#if ZIP
        [JsonConverter(typeof(DlmsBinaryConverter))]
#endif
        public byte[] Data
        {
            get;
            set;
        }
    }
}
