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
using System.Xml.Serialization;
using ZIP.DLMS;

namespace Gurux.DLMS.Objects
{
    public class GXDLMSSpecialDay
    {
        public UInt16 Index
        {
            get;
            set;
        }
#if ZIP
        // sem 14.01.2022
        [JsonIgnore]
        //[JsonConverter(typeof(DLMSDateConverter))]
        public GXDate Date
        {
            get; set;
        }

        // sem 14.01.2022
        [XmlIgnore()]
        [JsonProperty("Date")]
        public string HexStr
        {
            get { return Date?.ToHex(false, false); }
            set { Date = new GXDate(GXDateTime.ParseHex(value)); }
        }
#else
        public GXDate Date
        {
            get;
            set;
        }
#endif
        public byte DayId
        {
            get;
            set;
        }

        public override string ToString()
        {
            return Index.ToString() + " " + Date.ToString() + " " + DayId.ToString();
        }
    }
}
