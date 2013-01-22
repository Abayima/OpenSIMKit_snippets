/*
 * CommandStatus.cs: ISO-7816-4 command status
 * 
 * Copyright (c) 2007-2008 Andreas Faerber
 * 
 * All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification,
 * are permitted provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright notice,
 *    this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 *    this list of conditions and the following disclaimer in the documentation
 *    and/or other materials provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PcSc
{
	public struct CommandStatus
	{
		public CommandStatus(byte sw1, byte sw2) {
			this.Sw1 = sw1;
			this.Sw2 = sw2;
		}

		public byte Sw1, Sw2;

		public bool IsNormal() {
			return ((Sw1 == 0x90) && (Sw2 == 0x00)) ||
			        (Sw1 == 0x61);
		}

		public int GetRemainingLength() {
			if (Sw1 != 0x61)
				throw new InvalidOperationException();
			return Sw2;
		}

		public Exception ToException() {
			switch (Sw1) {
				case 0x90:
					if (Sw2 == 0)
						return null;
					break;
				case 0x62:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x81:
							return new IOException("Part of returned data may be corrupt");
						case 0x82:
							return new IOException("End of file or record reached before reading Le bytes");
						case 0x83:
							return new IOException("Selected file invalidated");
						case 0x84:
							return new IOException("File control information not formatted according to 5.6");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x63:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x81:
							return new IOException("File filled up by the last write");
						//case 0xCx:
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x64:
					switch (Sw2) {
						case 0x00:
							return new IOException("Execution error");
						case 0x01:
							return new IOException("Immediate reply required by the card");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x65:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x81:
							return new IOException("Memory failure");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x66:
					switch (Sw2) {
						default:
							return new IOException(String.Format("Security-related issue {0:X} {1:X}", Sw1, Sw2));
					}
				case 0x67:
					switch (Sw2) {
						case 0x00:
							return new IOException("Wrong length");
						default:
							break;
					}
					break;
				case 0x68:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x81:
							return new IOException("Logical channel not supported");
						case 0x82:
							return new IOException("Secure messaging not supported");
						case 0x83:
							return new IOException("Final chained command expected");
						case 0x84:
							return new IOException("Command chaining not supported");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x69:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x81:
							return new IOException("Command incompatible with file structure");
						case 0x82:
							return new IOException("Security status not satisfied");
						case 0x83:
							return new IOException("Authentication method blocked");
						case 0x84:
							return new IOException("Referenced data invalidated");
						case 0x85:
							return new IOException("Conditions of use not satisfied");
						case 0x86:
							return new IOException("Command not allowed (no current EF)");
						case 0x87:
							return new IOException("Expected secure messaging data objects missing");
						case 0x88:
							return new IOException("Incorrect secure messaging data objects");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x6A:
					switch (Sw2) {
						case 0x00:
							return new IOException("No information given");
						case 0x80:
							return new IOException("Incorrect parameters in the command data field");
						case 0x81:
							return new IOException("Function not supported");
						case 0x82:
							return new FileNotFoundException();
						case 0x83:
							return new IOException("Record not found");
						case 0x84:
							return new IOException("Not enough memory space in the file");
						case 0x85:
							return new IOException("Lc inconsistent with TLV structure");
						case 0x86:
							return new IOException("Incorrect parameters P1-P2");
						case 0x87:
							return new IOException("Lc inconsistent with parameters P1-P2");
						case 0x88:
							return new IOException("Referenced data not found");
						case 0x89:
							return new IOException("File already exists");
						case 0x8A:
							return new IOException("DF name already exists");
						default:
							return new IOException(String.Format("{0:X} {1:X}", Sw1, Sw2));
					}
				case 0x6B:
					switch (Sw2) {
						case 0x00:
							return new IOException("Wrong parameters P1-P2");
						default:
							break;
					}
					break;
				case 0x6C:
					return new IOException(String.Format("Wrong length Le. Expected value: {0}", Sw2));
				case 0x6D:
					switch (Sw2) {
						case 0x00:
							return new IOException("Instruction code not supported or invalid");
						default:
							break;
					}
					break;
				case 0x6E:
					switch (Sw2) {
						case 0x00:
							return new IOException("Class not supported");
						default:
							break;
					}
					break;
				case 0x6F:
					switch (Sw2) {
						case 0x00:
							return new IOException("No precise diagnosis");
						default:
							break;
					}
					break;
				default:
					break;
			}
			return new IOException(String.Format("Unexpected status {0:X} {1:X}", Sw1, Sw2));
		}
	}
}
