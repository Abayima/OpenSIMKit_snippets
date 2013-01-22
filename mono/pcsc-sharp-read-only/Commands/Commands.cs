/*
 * Commands.cs: Some ISO-7816-4 commands
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

namespace PcSc.Iso7816_4
{
	public class Commands
	{
		public static Stream Select(SmartCard card, SelectType selectType, byte[] file, SelectFileOccurrence fileOccurrence, SelectFileControlInformation fileControlInformation, int expectedLength) {
			Command cmd = new Command();
			cmd.Instruction = 0xA4;
			cmd.Parameter1 = (byte)selectType;
			cmd.Parameter2 = (byte)((byte)fileOccurrence | (byte)fileControlInformation);
			cmd.Data = file;
			cmd.ResponseLength = expectedLength;
			byte[] command = cmd.ToArray();
			byte[] buffer = new byte[256];
			//Console.Write("Sending command {0}...", BitConverter.ToString(command));
			int len = card.Transmit(command, buffer);
			//Console.WriteLine(BitConverter.ToString(buffer, 0, len));
			CommandStatus status = GetStatus(buffer, len);
			if (!status.IsNormal())
				throw status.ToException();
			Stream result = new MemoryStream();
			result.Write(buffer, 0, len - 2);
			while (status.Sw1 == 0x61) {
				throw new NotImplementedException("Reading of remaining length not implemented");
			}
			return result;
		}

		public static Stream ReadBinary(SmartCard card, int offset, int expectedLength) {
			if ((offset < 0) || (offset > 0x7FFF))
				throw new ArgumentOutOfRangeException("offset");
			Command cmd = new Command();
			cmd.Instruction = 0xB0;
			cmd.Parameter1 = (byte)((offset >> 8) & 0x7F);
			cmd.Parameter2 = (byte)offset;
			cmd.ResponseLength = expectedLength;
			byte[] command = cmd.ToArray();
			byte[] buffer = new byte[expectedLength + 2];
			//Console.Write("Sending command {0}...", BitConverter.ToString(command));
			int len = card.Transmit(command, buffer);
			//Console.WriteLine(BitConverter.ToString(buffer, 0, len));
			CommandStatus status = GetStatus(buffer, len);
			if (!status.IsNormal())
				throw status.ToException();
			Stream result = new MemoryStream();
			result.Write(buffer, 0, len - 2);
			while (status.Sw1 == 0x61) {
				throw new NotImplementedException("Reading of remaining length not implemented");
			}
			result.Position = 0;
			return result;
		}

		public static Stream ReadBinary(SmartCard card, int fileId, int offset, int expectedLength) {
			if ((fileId < 0) || (fileId > 0x1F))
				throw new ArgumentOutOfRangeException("id");
			if ((offset < 0) || (offset > 0xFF))
				throw new ArgumentOutOfRangeException("offset");
			Command cmd = new Command();
			cmd.Instruction = 0xB0;
			cmd.Parameter1 = (byte)((1 << 7) | (fileId & 0x1F));
			cmd.Parameter2 = (byte)offset;
			cmd.ResponseLength = (expectedLength <= 0) ? 0xFF : expectedLength;
			byte[] command = cmd.ToArray();
			byte[] buffer = new byte[((expectedLength <= 0) ? 256 : expectedLength) + 2];
			//Console.Write("Sending command {0}...", BitConverter.ToString(command));
			int len = card.Transmit(command, buffer);
			//Console.WriteLine(BitConverter.ToString(buffer, 0, len));
			CommandStatus status = GetStatus(buffer, len);
			if (!status.IsNormal())
				throw status.ToException();
			Stream result = new MemoryStream();
			result.Write(buffer, 0, len - 2);
			while (status.Sw1 == 0x61) {
				if (buffer.Length < status.Sw2 + 2)
					buffer = new byte[status.Sw2 + 2];
				cmd = new Command();
				cmd.Instruction = 0xC0;
				cmd.ResponseLength = status.Sw2;
				command = cmd.ToArray();
				len = card.Transmit(command, buffer);
				status = GetStatus(buffer, len);
				if (!status.IsNormal())
					throw status.ToException();
				result.Write(buffer, 0, len - 2);
			}
			result.Position = 0;
			return result;
		}

		public static CommandStatus GetStatus(byte[] buffer, int length) {
			return GetStatus(buffer, 0, length);
		}

		public static CommandStatus GetStatus(byte[] buffer, int startIndex, int length) {
			return new CommandStatus(buffer[startIndex + length - 2], buffer[startIndex + length - 1]);
		}
	}

	public enum SelectType
	{
		// by file identifier
		Identifier = 0,
		ChildDFIdentifier = 1,
		EFIdentifierUnderCurrentDF = 2,
		ParentDF = 3,
		// by name
		DFName = 4,
		// by path
		PathFromMF = 8,
		PathFromCurrentDF = 9
	}

	public enum SelectFileOccurrence
	{
		FirstOrOnly = 0,
		Last        = 1,
		Next        = 2,
		Previous    = 3
	}

	public enum SelectFileControlInformation
	{
		Fci         = 0,
		Fcp         = 4,
		Fmd         = 8,
		Proprietary = 12
	}
}
