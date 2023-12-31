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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Credit status.
    /// </summary>
    [Flags]
    public enum AccountCreditStatus
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,
        /// <summary>
        /// In credit.
        /// </summary>
        InCredit = 0x1,
        /// <summary>
        /// Low credit.
        /// </summary>
        LowCredit = 0x2,
        /// <summary>
        /// Next credit enabled.
        /// </summary>
        NextCreditEnabled = 0x4,
        /// <summary>
        /// Next credit selectable.
        /// </summary>
        NextCreditSelectable = 0x8,
        /// <summary>
        /// Credit reference list.
        /// </summary>
        CreditReferenceList = 0x10,
        /// <summary>
        /// Selectable credit in use.
        /// </summary>
        SelectableCreditInUse = 0x20,
        /// <summary>
        /// Out of credit.
        /// </summary>
        OutOfCredit = 0x40,
        /// <summary>
        /// Reserved.
        /// </summary>
        Reserved = 0x80
    }
}
