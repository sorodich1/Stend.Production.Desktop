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

#if ZIP
using Newtonsoft.Json;
using ZIP.DLMS;
#endif


namespace Gurux.DLMS
{
    /// <summary>
    /// GXDLMSGateway contains information that is needed if gateway is used between the client and the meter.
    /// </summary>
    public class GXDLMSGateway
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public GXDLMSGateway()
        {
        }

        /// <summary>
        /// Gateway network ID.
        /// </summary>
        public byte NetworkId
        {
            get;
            set;
        }

        /// <summary>
        /// Physical device address.
        /// </summary>
#if ZIP
        [JsonConverter(typeof(DlmsBinaryConverter))]
#endif
        public byte[] PhysicalDeviceAddress
        {
            get;
            set;
        }
    }
}