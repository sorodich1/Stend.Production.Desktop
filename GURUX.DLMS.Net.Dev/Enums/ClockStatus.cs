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

using System;
using System.ComponentModel;

namespace Gurux.DLMS.Enums
{
    /// <summary>
    /// Defines Clock status.
    /// </summary>
    [Flags]
    public enum ClockStatus
    {
        /// <summary>
        /// OK.
        /// </summary>
        [Description("��")]
        Ok = 0,
        /// <summary>
        /// Invalid a value,
        /// </summary>
        [Description("������������ ��������")]
        InvalidValue = 0x1,
        /// <summary>
        /// Doubtful b value
        /// </summary>
        [Description("������������ ��������")]
        DoubtfulValue = 0x2,
        /// <summary>
        /// Different clock base c
        /// </summary>
        [Description("�������� ���� �����")]
        DifferentClockBase = 0x4,
        /// <summary>
        /// Invalid clock status d
        /// </summary>
        [Description("������������ ��������� �����")]
        InvalidClockStatus = 0x8,
        /// <summary>
        /// Daylight saving active.
        /// </summary>
        [Description("������ �����")]
        DaylightSavingActive = 0x80,
        /// <summary>
        /// Clock status is skipped.
        /// </summary>
        [Description("������� �������")]
        Skip = 0xFF
    }
}
