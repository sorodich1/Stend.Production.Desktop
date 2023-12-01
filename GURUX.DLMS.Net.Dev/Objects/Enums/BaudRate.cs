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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Defines the baudrates.
    /// </summary>
    public enum BaudRate
    {
        /// <summary>
        /// Baudrate is 300.
        /// </summary>
        [Description("300")]
        Baudrate300 = 0,
        /// <summary>
        /// Baudrate is 600.
        /// </summary>
        [Description("600")]
        Baudrate600,
        /// <summary>
        /// Baudrate is 1200.
        /// </summary>
        [Description("1200")]
        Baudrate1200,
        /// <summary>
        /// Baudrate is 2400.
        /// </summary>
        [Description("2400")]
        Baudrate2400,
        /// <summary>
        /// Baudrate is 4800.
        /// </summary>
        [Description("4800")]
        Baudrate4800,
        /// <summary>
        /// Baudrate is 9600.
        /// </summary>
        [Description("9600")]
        Baudrate9600,
        /// <summary>
        /// Baudrate is 19200.
        /// </summary>
        [Description("19200")]
        Baudrate19200,
        /// <summary>
        /// Baudrate is 38400.
        /// </summary>
        [Description("38400")]
        Baudrate38400,
        /// <summary>
        /// Baudrate is 57600.
        /// </summary>
        [Description("57600")]
        Baudrate57600,
        /// <summary>
        /// Baudrate is 115200.
        /// </summary>
        [Description("115200")]
        Baudrate115200
    }
}
