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
    public enum ClockBase
    {
        /// <summary>
        /// Not defined
        /// </summary>
        [Description("Не определена")]
        None,
        /// <summary>
        /// Internal Crystal
        /// </summary>
        [Description("Внутренний кварц")]
        Crystal,
        /// <summary>
        /// Mains frequency 50 Hz,
        /// </summary>
        [Description("Частота сети 50 Гц")]
        Frequency50,
        /// <summary>
        /// Mains frequency 60 Hz,
        /// </summary>
        [Description("Частота сети 60 Гц")]
        Frequency60,
        /// <summary>
        /// Global Positioning System.
        /// </summary>
        [Description("GPS (глобальная система позиционирования)")]
        GPS,
        /// <summary>
        /// Radio controlled.
        /// </summary>
        [Description("Радиоуправление")]
        Radio
    }
}
