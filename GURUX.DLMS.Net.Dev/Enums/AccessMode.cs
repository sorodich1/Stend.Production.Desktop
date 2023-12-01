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

using System.ComponentModel;
using System.Xml.Serialization;

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// The AccessMode enumerates the access modes.
    /// </summary>
    public enum AccessMode
    {
        /// <summary>
        /// No access.
        /// </summary>
        [XmlEnum("0")]
        [Description("��� �������")]
        NoAccess = 0,
        /// <summary>
        /// The client is allowed only reading from the server.
        /// </summary>
        [XmlEnum("1")]
        [Description("������")]
        Read = 1,
        /// <summary>
        /// The client is allowed only writing to the server.
        /// </summary>
        [XmlEnum("2")]
        [Description("������")]
        Write = 2,
        /// <summary>
        /// The client is allowed both reading from the server and writing to it.
        /// </summary>
        [XmlEnum("3")]
        [Description("������ � ������")]
        ReadWrite = 3,
        /// <summary>
        /// Authenticated read is used.
        /// </summary>
        [XmlEnum("4")]
        [Description("������������������� ������")]
        AuthenticatedRead = 4,
        /// <summary>
        /// Authenticated write is used.
        /// </summary>
        [XmlEnum("5")]
        [Description("������������������� ������")]
        AuthenticatedWrite = 5,
        /// <summary>
        /// Authenticated Read Write is used.
        /// </summary>
        [Description("������������������� ������ � ������")]
        [XmlEnum("6")]
        AuthenticatedReadWrite = 6
    }
}