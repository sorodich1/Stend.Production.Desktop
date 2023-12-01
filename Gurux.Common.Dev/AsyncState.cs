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
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------


/* �������������� ������� �� ������� "Gurux.Common (netcoreapp3.1)"
��:
using System;
�����:
using Gurux.Common;
using System;
*/

/* �������������� ������� �� ������� "Gurux.Common (netstandard2.1)"
��:
using System;
�����:
using Gurux.Common;
using System;
*/

/* �������������� ������� �� ������� "Gurux.Common (netcoreapp3.1)"
��:
using System.Threading;
using Gurux.Common;
�����:
using System.Threading;
*/

/* �������������� ������� �� ������� "Gurux.Common (netstandard2.1)"
��:
using System.Threading;
using Gurux.Common;
�����:
using System.Threading;
*/
namespace Gurux.Common
{
    /// <summary>
    /// Async state.
    /// </summary>
    public enum AsyncState
    {
        /// <summary>
        /// New work is started.
        /// </summary>
        Start,
        /// <summary>
        /// Work is done.
        /// </summary>
        Finish,
        /// <summary>
        /// Work is canceled.
        /// </summary>
        Cancel
    }
}
