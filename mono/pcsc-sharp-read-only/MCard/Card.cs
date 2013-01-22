/*
 * Card.cs: Memory card
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
using System.Runtime.InteropServices;

namespace PcSc.MCard
{
	public class MemoryCard : MarshalByRefObject, IDisposable
	{

		private IntPtr card;

		public MemoryCard(IntPtr card) {
			this.card = card;
		}

		~MemoryCard() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (card != IntPtr.Zero)
				MCardDisconnect(card, SmartCardDisposition.Unpower);
		}

		public int MemorySize {
			get {
				uint len = (uint)Marshal.SizeOf(typeof(Memory));
				IntPtr ptr = Marshal.AllocHGlobal((int)len);
				try {
					int ret = MCardGetAttrib(card, MemoryCardAttribute.Memory, ptr, ref len);
					if (ret != 0)
						throw new Exception(ret.ToString("x"));
					Memory value = (Memory)Marshal.PtrToStructure(ptr, typeof(Memory));
					return (int)value.Size;
				} finally {
					Marshal.FreeHGlobal(ptr);
				}
			}
		}

		public int ReadMemory(int zone, int offset, byte[] dest, int startIndex, int length) {
			IntPtr ptr = Marshal.AllocHGlobal(length);
			try {
				uint len = (uint)length;
				int ret = MCardReadMemory(card, (byte)zone, (uint)offset, ptr, ref len);
				if (ret != 0)
					throw new Exception(ret.ToString("x"));
				Marshal.Copy(ptr, dest, startIndex, (int)len);
				return (int)len;
			} finally {
				Marshal.FreeHGlobal(ptr);
			}
		}

		public int ReadMemoryByte(int zone, int offset) {
			try {
				uint len = 1;
				byte value;
				int ret = MCardReadMemory(card, (byte)zone, (uint)offset, out value, ref len);
				if (ret != 0)
					return -1;
				return value;
			} catch (Exception) {
				return -1;
			}
		}

		private struct Memory
		{
			public uint Flags;
			public uint Size;
		}

		[DllImport("MCChipdr.dll")]
		private static extern int MCardDisconnect(IntPtr card, SmartCardDisposition disposition);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardGetAttrib(IntPtr card, MemoryCardAttribute attribute, IntPtr valuePtr, ref uint length);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardReadMemory(IntPtr card, byte zone, uint offset, IntPtr buffer, ref uint length);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardReadMemory(IntPtr card, byte zone, uint offset, out byte value, ref uint length);
	}
}
