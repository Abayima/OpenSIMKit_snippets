/*
 * Command.cs: ISO-7816-4 command
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
	public struct Command
	{
		public byte InstructionClass;
		public byte Instruction;
		public byte Parameter1, Parameter2;
		public byte[] Data;
		public int ResponseLength;

		public byte[] ToArray() {
			MemoryStream ms = new MemoryStream();
			ms.WriteByte(InstructionClass);
			ms.WriteByte(Instruction);
			ms.WriteByte(Parameter1);
			ms.WriteByte(Parameter2);
			bool longLc = false;
			if ((Data != null) && (Data.Length > 0)) {
				if ((Data.Length <= 0xFF) && (ResponseLength <= 0xFF)) {
					ms.WriteByte((byte)Data.Length);
				} else {
					if (Data.Length > 0xFFFF)
						throw new InvalidOperationException("Lc greater than FFFF");
					ms.WriteByte(0);
					ms.WriteByte((byte)(Data.Length >> 8));
					ms.WriteByte((byte)Data.Length);
					longLc = true;
				}
				ms.Write(Data, 0, Data.Length);
			}
			if (ResponseLength >= 0) {
				if (longLc) {
					ms.WriteByte((byte)(ResponseLength >> 8));
					ms.WriteByte((byte)ResponseLength);
				} else {
					if (ResponseLength <= 0xFF) {
						ms.WriteByte((byte)ResponseLength);
					} else {
						if (ResponseLength > 0xFFFF)
							throw new InvalidOperationException("Le greater than FFFF");
						ms.WriteByte(0);
						ms.WriteByte((byte)(ResponseLength >> 8));
						ms.WriteByte((byte)ResponseLength);
					}
				}
			}
			return ms.ToArray();
		}
	}
}
