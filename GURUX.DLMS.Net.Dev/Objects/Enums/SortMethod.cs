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
    /// Sort methods.
    /// </summary>
    public enum SortMethod
    {
        /// <summary>
        /// First in first out
        /// </summary>        
        /// <remarks>
        /// When circle buffer is full first item is removed.
        /// </remarks>
        [Description("FIFO")]
        FiFo = 1,
        /// <summary>
        /// Last in first out.
        /// </summary>
        /// <remarks>
        /// When circle buffer is full last item is removed.
        /// </remarks>
        [Description("LIFO")]
        LiFo,
        /// <summary>
        /// Largest is first.
        /// </summary>
        [Description("�� ��������")]
        Largest,
        /// <summary>
        /// Smallest is first.
        /// </summary>
        [Description("�� �����������")]
        Smallest,
        /// <summary>
        /// Nearest to zero is first.
        /// </summary>
        [Description("������ - ��������� � ����")]
        NearestToZero,
        /// <summary>
        /// Farest from zero is first.
        /// </summary>
        [Description("������ - ����� ��������� �� ����")]
        FarestFromZero
    }
}
