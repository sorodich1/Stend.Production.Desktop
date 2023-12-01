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

using Newtonsoft.Json;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSEmergencyProfile
    {
        //[DisplayName("ID")]
        public UInt16 ID
        {
            get;
            set;
        }
        //[DisplayName("Время активации")]
        // sem 14.01.2022
#if ZIP
        [JsonIgnore]
        public GXDateTime ActivationTime
        {
            get; set;
        }

        [XmlIgnore()]
        [JsonProperty("ActivationTime")]
        public string HexStr
        {
            get { return ActivationTime?.ToHex(false, false); }
            set { ActivationTime = GXDateTime.ParseHex(value); }
        }
#else
        public GXDateTime ActivationTime
        {
            get;
            set;
        }
#endif
        //[DisplayName("Длительность")]
        public UInt32 Duration
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ID);
            sb.Append(" ");
            if (ActivationTime != null)
            {
                sb.Append(ActivationTime.ToFormatString());
            }
            sb.Append(" ");
            sb.Append(Duration);
            return sb.ToString();
        }
    }
}
