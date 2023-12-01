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
    public enum SingleActionScheduleType
    {
#if ZIP
        /// <summary>
        /// Size of execution_time not defined.
        /// </summary>
        [Description("<не определено>")]
        SingleActionScheduleType0 = 0,
#endif

        /// <summary>
        /// Size of execution_time = 1. Wildcard in date allowed.
        /// </summary>
#if ZIP
        [Description("Однократно. Шаблон даты вкл.")]
#endif
        SingleActionScheduleType1 = 1,

        /// <summary>
        /// Size of execution_time = n. 
        /// All time values are the same, wildcards in date not allowed.
        /// </summary>
#if ZIP
        [Description("Многократно. Шаблон даты выкл., времена одинаковы")]
#endif
        SingleActionScheduleType2 = 2,

        /// <summary>
        /// Size of execution_time = n. 
        /// All time values are the same, wildcards in date are allowed,
        /// </summary>
#if ZIP
        [Description("Многократно. Шаблон даты вкл., времена одинаковы")]
#endif
        SingleActionScheduleType3 = 3,

        /// <summary>
        /// Size of execution_time = n.
        /// Time values may be different, wildcards in date not allowed,
        /// </summary>
#if ZIP
        [Description("Многократно. Шаблон даты выкл., времена различны")]
#endif
        SingleActionScheduleType4 = 4,
        /// <summary>
        /// Size of execution_time = n.
        /// Time values may be different, wildcards in date are allowed
        /// </summary>
#if ZIP
        [Description("Многократно. Шаблон даты вкл., времена различны")]
#endif
        SingleActionScheduleType5 = 5
    }
}
