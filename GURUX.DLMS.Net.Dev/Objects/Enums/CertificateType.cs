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

namespace Gurux.DLMS.Objects.Enums
{
    /// <summary>
    /// Certificate type.
    /// </summary>
    public enum CertificateType
    {
        /// <summary>
        /// Certificate type is digital signature.
        /// </summary>
        /// <Remarks>
        /// Certificate is used with High ECDSA authentication.
        /// </Remarks>
        DigitalSignature = 0,
        /// <summary>
        /// Certificate type is key agreement.
        /// </summary>
        /// <Remarks>
        /// </Remarks>
        KeyAgreement = 1,
        /// <summary>
        /// Certificate type is TLS.
        /// </summary>
        /// <Remarks>
        /// </Remarks>
        TLS = 2,
        /// <summary>
        /// Certificate type is other.
        /// </summary>
        /// <Remarks>
        /// </Remarks>
        Other = 3
    }
}
