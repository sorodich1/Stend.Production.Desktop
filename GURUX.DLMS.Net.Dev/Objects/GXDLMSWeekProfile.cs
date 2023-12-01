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
using System.Text;
using System.Xml.Serialization;

namespace Gurux.DLMS
{
    public class GXDLMSWeekProfile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSWeekProfile()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSWeekProfile(string name, int monday, int tuesday,
            int wednesday, int thursday, int friday, int saturday, int sunday)
        {
            if (name != null)
            {
                Name = ASCIIEncoding.ASCII.GetBytes(name);
            }
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Sunday = sunday;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSWeekProfile(byte[] name, int monday, int tuesday,
            int wednesday, int thursday, int friday, int saturday, int sunday)
        {
            Name = name;
            Monday = monday;
            Tuesday = tuesday;
            Wednesday = wednesday;
            Thursday = thursday;
            Friday = friday;
            Saturday = saturday;
            Sunday = sunday;
        }

        // sem 14.01.2022
#if ZIP
        [JsonIgnore]
        public byte[] Name
        {
            get;
            set;
        }

        [XmlIgnore()]
        [JsonProperty("Name")]
        public string NameStr
        {
            get { return Encoding.UTF8.GetString(Name); }
            set { Name = Encoding.UTF8.GetBytes(value); }
        }
#else
        public byte[] Name
        {
            get;
            set;
        }
#endif
        public int Monday
        {
            get;
            set;
        }

        public int Tuesday
        {
            get;
            set;
        }

        public int Wednesday
        {
            get;
            set;
        }

        public int Thursday
        {
            get;
            set;
        }

        public int Friday
        {
            get;
            set;
        }

        public int Saturday
        {
            get;
            set;
        }

        public int Sunday
        {
            get;
            set;
        }

        public override string ToString()
        {
            return GXDLMSTranslator.ToHex(Name);
        }

    }
}
