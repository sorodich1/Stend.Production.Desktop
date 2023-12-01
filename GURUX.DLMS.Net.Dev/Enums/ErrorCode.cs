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
using System.ComponentModel;
#endif

namespace Gurux.DLMS.Enums
{
    /// <summary>
    ///  Enumerated DLMS error codes.
    /// </summary>
    /// <remarks>
    /// https://www.gurux.fi/Gurux.DLMS.ErrorCodes
    /// </remarks>
    public enum ErrorCode
    {
#if ZIP
        /// <summary>
        /// User aborted.
        /// </summary>
        [Description("�������� �������������")]
#endif
        UserAbortMode = -5,
        /// <summary>
        /// Disconnect Mode.
        /// </summary>
#if ZIP
        [Description("����� ����������")]
#endif
        DisconnectMode = -4,
        /// <summary>
        /// Receive Not Ready.
        /// </summary>
#if ZIP
        [Description("�� ����� � ������")]
#endif
        ReceiveNotReady = -3,
        /// <summary>
        /// Connection is rejected.
        /// </summary>
#if ZIP
        [Description("���������� ���������")]
#endif
        Rejected = -2,
        /// <summary>
        /// Unacceptable frame.
        /// </summary>
#if ZIP
        [Description("������������ ����")]
#endif
        UnacceptableFrame = -1,
        /// <summary>
        /// No error has occurred.
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Access Error : Device reports a hardware fault.
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� �� ���������� ������")]
#endif
        HardwareFault = 1,

        /// <summary>
        /// Access Error : Device reports a temporary failure.
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � ��������� ����")]
#endif
        TemporaryFailure = 2,

        /// <summary>
        /// Access Error : Device reports Read-Write denied.
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� ��������, ��� ������-������ ���������")]
#endif
        ReadWriteDenied = 3,

        /// <summary>
        /// Access Error : Device reports a undefined object
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � �������������� �������")]
#endif
        UndefinedObject = 4,

        /// <summary>
        /// Access Error : Device reports a inconsistent Class or object
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � ��������������� ������ ��� �������")]
#endif
        InconsistentClass = 9,

        /// <summary>
        /// Access Error : Device reports a unavailable object
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � ����������� �������")]
#endif
        UnavailableObject = 11,

        /// <summary>
        /// Access Error : Device reports a unmatched type
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � ����������������� ����")]
#endif
        UnmatchedType = 12,

        /// <summary>
        /// Access Error : Device reports scope of access violated
        /// </summary>
#if ZIP
        [Description("������ �������: ���������� �������� � ��������� ������� �������")]
#endif
        AccessViolated = 13,

        /// <summary>
        /// Access Error : Data Block Unavailable.
        /// </summary>
#if ZIP
        [Description("������ �������: ���� ������ ����������")]
#endif
        DataBlockUnavailable = 14,

        /// <summary>
        /// Access Error : Long Get Or Read Aborted.
        /// </summary>
#if ZIP
        [Description("������ �������: �������� �������� ����������� Get/Read")]
#endif
        LongGetOrReadAborted = 15,

        /// <summary>
        /// Access Error : No Long Get Or Read In Progress.
        /// </summary>
#if ZIP
        [Description("������ �������: ��� �������� ����������� Get/Read")]
#endif
        NoLongGetOrReadInProgress = 16,

        /// <summary>
        /// Access Error : Long Set Or Write Aborted.
        /// </summary>
#if ZIP
        [Description("������ �������: �������� �������� ����������� Set/Write")]
#endif
        LongSetOrWriteAborted = 17,

        /// <summary>
        /// Access Error : No Long Set Or Write In Progress.
        /// </summary>
#if ZIP
        [Description("������ �������:  ��� �������� ����������� Set/Write")]
#endif
        NoLongSetOrWriteInProgress = 18,

        /// <summary>
        /// Access Error : Data Block Number Invalid.
        /// </summary>
#if ZIP
        [Description("������ �������: �������� ����� ����� ������")]
#endif
        DataBlockNumberInvalid = 19,

        /// <summary>
        /// Access Error : Other Reason.
        /// </summary>
#if ZIP
        [Description("������ �������: ������ �������")]
#endif
        OtherReason = 250
    }
}
