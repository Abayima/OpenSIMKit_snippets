/*
 * Errors.cs: PC/SC errors
 * 
 * Copyright (c) 2006-2008 Andreas Faerber
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.IO;

namespace PcSc
{
	internal enum SmartCardErrors : uint {
		Success                = 0,

		InternalError          = 0x80100001,
		Cancelled              = 0x80100002,
		InvalidHandle          = 0x80100003,
		InvalidParameter       = 0x80100004,
		InvalidTarget          = 0x80100005,
		NoMemory               = 0x80100006,
		WaitedTooLong          = 0x80100007,
		InsufficientBuffer     = 0x80100008,
		UnknownReader          = 0x80100009,
		Timeout                = 0x8010000A,
		SharingViolation       = 0x8010000B,
		NoSmartcard            = 0x8010000C,
		UnknownCard            = 0x8010000D,
		CantDispose            = 0x8010000E,
		ProtocolMismatch       = 0x8010000F,
		NotReady               = 0x80100010,
		InvalidValue           = 0x80100011,
		SystemCancelled        = 0x80100012,
		CommError              = 0x80100013,
		UnknownError           = 0x80100014,
		InvalidAtr             = 0x80100015,
		NotTransacted          = 0x80100016,
		ReaderUnavailable      = 0x80100017,
		Shutdown               = 0x80100018,
		PciTooSmall            = 0x80100019,
		ReaderUnsupported      = 0x8010001A,
		DuplicateReader        = 0x8010001B,
		CardUnsupported        = 0x8010001C,
		NoService              = 0x8010001D,
		ServiceStopped         = 0x8010001E,
		Unexpected             = 0x8010001F,
		IccInstallation        = 0x80100020,
		IccCreateOrder         = 0x80100021,
		UnsupportedFeature     = 0x80100022,
		DirNotFound            = 0x80100023,
		FileNotFound           = 0x80100024,
		NoDir                  = 0x80100025,
		NoFile                 = 0x80100026,
		NoAccess               = 0x80100027,
		WriteTooMany           = 0x80100028,
		BadSeek                = 0x80100029,
		InvalidChv             = 0x8010002A,
		UnknownResMng          = 0x8010002B,
		NoSuchCertificate      = 0x8010002C,
		CertificateUnavailable = 0x8010002D,
		NoReadersAvailable     = 0x8010002E,
		CommDataLost           = 0x8010002F,
		NoKeyContainer         = 0x80100030,
		ServerTooBusy          = 0x80100031,

		UnsupportedCard        = 0x80100065,
		UnresponsiveCard       = 0x80100066,
		UnpoweredCard          = 0x80100067,
		ResetCard              = 0x80100068,
		RemovedCard            = 0x80100069,
		SecurityViolation      = 0x8010006A,
		WrongChv               = 0x8010006B,
		ChvBlocked             = 0x8010006C,
		Eof                    = 0x8010006D,
		CancelledByUser        = 0x8010006E,
		CardNotAuthenticated   = 0x8010006F
	}
	
	public class PcscException : System.ComponentModel.Win32Exception {
		public PcscException(int error) : base(error) {
		}
		
		internal PcscException(SmartCardErrors error) : this((int)error) {
		}
	}
	
	public class SmartCardSharingException : IOException {
		public SmartCardSharingException(PcscException innerException) : base("A sharing violation occurred.", innerException) {
		}
	}
}
