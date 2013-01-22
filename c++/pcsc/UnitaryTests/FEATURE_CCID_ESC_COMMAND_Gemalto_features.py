#! /usr/bin/env python

"""
#   FEATURE_CCID_ESC_COMMAND.py: Unitary test for FEATURE_CCID_ESC_COMMAND
#   Copyright (C) 2011  Ludovic Rousseau
"""

#   This program is free software; you can redistribute it and/or modify
#   it under the terms of the GNU General Public License as published by
#   the Free Software Foundation; either version 3 of the License, or
#   (at your option) any later version.
#
#   This program is distributed in the hope that it will be useful,
#   but WITHOUT ANY WARRANTY; without even the implied warranty of
#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#   GNU General Public License for more details.
#
#   You should have received a copy of the GNU General Public License along
#   with this program; if not, see <http://www.gnu.org/licenses/>.

from smartcard.System import readers
from smartcard.pcsc.PCSCPart10 import (SCARD_SHARE_DIRECT,
    SCARD_LEAVE_CARD, FEATURE_CCID_ESC_COMMAND, getFeatureRequest, hasFeature)
from smartcard.Exceptions import SmartcardException


def test_bit(value, bit):
    mask = 1 << bit
    return value & mask == mask


def main():
    """ main """
    card_connection = readers()[0].createConnection()
    card_connection.connect(mode=SCARD_SHARE_DIRECT,
        disposition=SCARD_LEAVE_CARD)

    feature_list = getFeatureRequest(card_connection)

    ccid_esc_command = hasFeature(feature_list, FEATURE_CCID_ESC_COMMAND)
    if ccid_esc_command is None:
        raise Exception("The reader does not support FEATURE_CCID_ESC_COMMAND")

    # Proprietary command for Gemalto readers
    # This is implemented by the Gemalto Pinpad v2 and C200 readers
    firmware_features = [0x6A]
    try:
        res = card_connection.control(ccid_esc_command, firmware_features)
    except SmartcardException, ex:
        print "Failed:", ex
        return

    print res
    print "LogicalLCDLineNumber (Logical number of LCD lines):", res[0]
    print "LogicalLCDRowNumber (Logical number of characters per LCD line):", res[1]
    print "LcdInfo:", res[2]
    print "  b0 indicates if scrolling available:", test_bit(res[2], 0)
    print "EntryValidationCondition:", res[3]

    print "VerifyPinStart:", test_bit(res[4], 0)
    print "VerifyPinFinish:", test_bit(res[4], 1)
    print "ModifyPinStart:", test_bit(res[4], 2)
    print "ModifyPinFinish:", test_bit(res[4], 3)
    print "GetKeyPressed:", test_bit(res[4], 4)
    print "VerifyPinDirect:", test_bit(res[4], 5)
    print "ModifyPinDirect:", test_bit(res[4], 6)
    print "Abort:", test_bit(res[4], 7)

    print "GetKey:", test_bit(res[5], 0)
    print "WriteDisplay:", test_bit(res[5], 1)
    print "SetSpeMessage:", test_bit(res[5], 2)
    # bits 3-7 are RFU
    # bytes 6 and 7 are RFU

    print "bTimeOut2:", test_bit(res[8], 0)
    # bits 1-7 are RFU
    # bytes 9, 10 and 11 and RFU

    print "VersionNumber:", res[12]
    print "MinimumPINSize:", res[13]
    print "MaximumPINSize:", res[14]
    print "Firewall:", test_bit(res[15], 0)
    # bits 1-7 are RFU

    # bytes 16-20 are RFU

if __name__ == "__main__":
    main()
