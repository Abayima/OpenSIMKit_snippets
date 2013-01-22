/*
 * Card.cs: Smart card (SCARDHANDLE)
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

namespace PcSc
{
	public class SmartCard : MarshalByRefObject, IDisposable
	{

		protected IntPtr card;
		protected SmartCardDisposition dispose_disposition = SmartCardDisposition.Leave;

		internal SmartCard(IntPtr card) {
			this.card = card;
		}

		~SmartCard() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			int ret = SCardDisconnect(card, dispose_disposition);
			if (ret != 0)
				Console.WriteLine(SmartCardContext.ToException(ret).Message);
		}
		
		public SmartCardDisposition Disposition {
			get {
				return dispose_disposition;
			}
			set {
				dispose_disposition = value;
			}
		}
		
		public SmartCardState GetStatus() {
			uint readerLen = 0;
			int ret = SCardGetStatus(card, IntPtr.Zero, ref readerLen, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (ret != 0)
				throw SmartCardContext.ToException(ret);
			IntPtr readerPtr = Marshal.AllocHGlobal((int)readerLen);
			SmartCardState state;
			SmartCardProtocols protocol;
			uint atrLen = 0;
			ret = SCardGetStatus(card, readerPtr, ref readerLen, out state, out protocol, IntPtr.Zero, out atrLen);
			if (ret != 0)
				throw SmartCardContext.ToException(ret);
			return state;
		}

		public int Transmit(byte[] sendBuffer, byte[] receiveBuffer) {
			SmartCardIORequest sendPci = SmartCardIORequest.T1;
			SmartCardIORequest recvPci = SmartCardIORequest.T1;
			uint len = (uint)receiveBuffer.Length;
			IntPtr ptr = Marshal.AllocHGlobal((int)len);
			try {
				int ret = SCardTransmit(card, ref sendPci, sendBuffer, (uint)sendBuffer.Length, ref recvPci, ptr, ref len);
				if (ret != 0)
					throw SmartCardContext.ToException(ret);
				//Console.Write("Transmit received {0} bytes: ", len);
				Marshal.Copy(ptr, receiveBuffer, 0, (int)len);
				//Console.WriteLine(BitConverter.ToString(receiveBuffer, 0, (int)len));
				return (int)len;
			} finally {
				Marshal.FreeHGlobal(ptr);
			}
		}

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardDisconnect(IntPtr card, SmartCardDisposition disposition);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardTransmit(IntPtr card, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest sendPci, [MarshalAs(UnmanagedType.LPArray)] byte[] sendBuffer, uint sendLen, [MarshalAs(UnmanagedType.Struct)] ref SmartCardIORequest recvPci, IntPtr recvBuffer, ref uint recvLen);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, IntPtr state, IntPtr protocol, IntPtr atr, IntPtr atrLength);

		[DllImport("Winscard.dll", CharSet = CharSet.Auto)]
		private static extern int SCardGetStatus(IntPtr card, IntPtr readerName, ref uint readerLength, out SmartCardState state, out SmartCardProtocols protocol, IntPtr atr, out uint atrLength);
	}
}
