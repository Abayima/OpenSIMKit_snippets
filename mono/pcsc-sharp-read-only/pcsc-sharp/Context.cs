/*
 * Context.cs: Smart card context (SCARDCONTEXT)
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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PcSc
{
	public class SmartCardContext : MarshalByRefObject, IDisposable
	{
		protected IntPtr context;

		public SmartCardContext() {
			int ret = SCardEstablishContext(SmartCardScope.System, IntPtr.Zero, IntPtr.Zero, out context);
			if (ret != 0)
				throw ToException(ret);
		}

		~SmartCardContext() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (context != IntPtr.Zero) {
				try {
					Cancel();
				} catch (Exception) {}
				int ret = SCardReleaseContext(context);
				if (ret != 0)
					throw ToException(ret);
				context = IntPtr.Zero;
			}
		}

		public IntPtr Handle {
			get {
				return context;
			}
		}

		public string[] GetReaders() {
			IntPtr readersPtr = IntPtr.Zero;
			int len = -1;
			int ret;
			try {
				if (Environment.OSVersion.Platform == PlatformID.Unix) {
					ret = SCardListReaders(context, IntPtr.Zero, IntPtr.Zero, ref len);
					if (ret != 0)
						throw ToException(ret);
					readersPtr = Marshal.AllocHGlobal(len * 1);
					ret = SCardListReaders(context, IntPtr.Zero, readersPtr, ref len);
				} else {
					ret = SCardListReaders(context, IntPtr.Zero, out readersPtr, ref len);
				}
				if (ret != 0)
					throw ToException(ret);
				List<string> readers = new List<string>();
				long offset = 0;
				string str;
				while ((str = Marshal.PtrToStringAnsi((IntPtr)((long)readersPtr + offset))) != String.Empty) {
					readers.Add(str);
					offset += (str.Length + 1) * 1;
				}
				return readers.ToArray();
			} finally {
				if (readersPtr != IntPtr.Zero) {
					if (Environment.OSVersion.Platform == PlatformID.Unix) {
						Marshal.FreeHGlobal(readersPtr);
					} else {
						SCardFreeMemory(context, readersPtr);
					}
				}
			}
		}
		
		public void WaitForStatusChange(string reader, SmartCardState currentState, SmartCardState newState) {
			WaitForStatusChange(reader, currentState, newState, uint.MaxValue);
		}
		
		public void WaitForStatusChange(string reader, SmartCardState currentState, SmartCardState newState, long timeout) {
			if ((timeout < 0) || (timeout > uint.MaxValue))
				throw new ArgumentOutOfRangeException("timeout must be a 32-bit unsigned integer value");
			ReaderState readerState = new ReaderState();
			readerState.Reader = reader;
			readerState.UserData = IntPtr.Zero;
			readerState.CurrentState = currentState;
			readerState.EventState = newState;
			readerState.AtrLength = 0;
			int ret = SCardGetStatusChange(context, (uint)timeout, ref readerState, 1);
			if (ret != 0)
				throw ToException(ret);
		}
		
		public void Cancel() {
			int ret = SCardCancel(context);
			if (ret != 0)
				throw ToException(ret);
		}

		public SmartCard Connect(string reader, SmartCardShare shareMode, SmartCardProtocols protocol) {
			IntPtr card;
			SmartCardProtocols activeProtocol;
			int ret = SCardConnect(context, reader, shareMode, protocol, out card, out activeProtocol);
			if (ret != 0)
				throw ToException(ret);
			//Console.WriteLine("Protocol: {0}", activeProtocol);
			return new SmartCard(card);
		}

		internal static Exception ToException(int returnValue) {
			PcscException ex = new PcscException(returnValue);
			switch (returnValue) {
				case 109:
					return new NotSupportedException("The local system does not support smart card redirection", ex);
				default:
				{
					SmartCardErrors value = (SmartCardErrors)returnValue;
					switch (value) {
						case SmartCardErrors.InternalError:
							return new SystemException("PC/SC internal error", ex);
						case SmartCardErrors.Cancelled:
							return new OperationCanceledException("Cancelled", ex);
						case SmartCardErrors.InvalidHandle:
							return new ArgumentException("Invalid handle", ex);
						case SmartCardErrors.InvalidParameter:
							return new ArgumentException("Invalid parameter", ex);
						case SmartCardErrors.InvalidTarget:
							return new SystemException("Invalid target", ex);
						case SmartCardErrors.NoMemory:
							return new OutOfMemoryException("No memory", ex);
						case SmartCardErrors.WaitedTooLong:
							return new TimeoutException("Waited too long", ex);
						case SmartCardErrors.InsufficientBuffer:
							return new InternalBufferOverflowException("Insufficient buffer", ex);
						case SmartCardErrors.UnknownReader:
							return new ArgumentException("Unknown reader", ex);
						case SmartCardErrors.Timeout:
							return new TimeoutException("Timeout", ex);
						case SmartCardErrors.SharingViolation:
							return new SmartCardSharingException(ex);
						case SmartCardErrors.NoSmartcard:
							return new Exception("No smart card", ex);
						case SmartCardErrors.UnknownCard:
							return new Exception("Unknown card", ex);
						case SmartCardErrors.CantDispose:
							return new Exception("Can't dispose", ex);
						case SmartCardErrors.ProtocolMismatch:
							return new IOException("Protocol mismatch", ex);
						case SmartCardErrors.NotReady:
							return new InvalidOperationException("Not ready", ex);
						case SmartCardErrors.InvalidValue:
							return new ArgumentException("Invalid value", ex);
						case SmartCardErrors.SystemCancelled:
							return new Exception("System cancelled", ex);
						case SmartCardErrors.CommError:
							return new IOException("Comm error", ex);
						case SmartCardErrors.UnknownError:
							return new Exception("Unknown error", ex);
						case SmartCardErrors.InvalidAtr:
							return new ArgumentException("Invalid ATR", ex);
						case SmartCardErrors.NotTransacted:
							return new Exception("Not transacted", ex);
						case SmartCardErrors.ReaderUnavailable:
							return new Exception("Reader unavailable", ex);
						case SmartCardErrors.Shutdown:
							return new SystemException("Shutdown", ex);
						case SmartCardErrors.PciTooSmall:
							return new SystemException("PCI too small", ex);
						case SmartCardErrors.ReaderUnsupported:
							return new NotSupportedException("Reader unsupported", ex);
						case SmartCardErrors.DuplicateReader:
							return new ArgumentException("Duplicate reader", ex);
						case SmartCardErrors.CardUnsupported:
							return new NotSupportedException("Card unsupported", ex);
						case SmartCardErrors.NoService:
							return new SystemException("No service", ex);
						case SmartCardErrors.ServiceStopped:
							return new SystemException("Service stopped", ex);
						case SmartCardErrors.Unexpected:
							return new Exception("Unexpected", ex);
						case SmartCardErrors.IccInstallation:
							return new Exception("ICC installation", ex);
						case SmartCardErrors.IccCreateOrder:
							return new Exception("ICC create order", ex);
						case SmartCardErrors.UnsupportedFeature:
							return new NotSupportedException("Unsupported feature", ex);
						case SmartCardErrors.DirNotFound:
							return new DirectoryNotFoundException("Directory not found", ex);
						case SmartCardErrors.FileNotFound:
							return new FileNotFoundException("File not found", ex);
						case SmartCardErrors.NoDir:
							return new InvalidOperationException("No directory", ex);
						case SmartCardErrors.NoFile:
							return new InvalidOperationException("No file", ex);
						case SmartCardErrors.NoAccess:
							return new Exception("No access", ex);
						case SmartCardErrors.WriteTooMany:
							return new Exception("Write too many", ex);
						case SmartCardErrors.BadSeek:
							return new Exception("Bad seek", ex);
						case SmartCardErrors.InvalidChv:
							return new Exception("Invalid CHV", ex);
						case SmartCardErrors.UnknownResMng:
							return new ArgumentException("Unknown resource manager", ex);
						case SmartCardErrors.NoSuchCertificate:
							return new Exception("No such certificate", ex);
						case SmartCardErrors.CertificateUnavailable:
							return new Exception("Certificate unavailable", ex);
						case SmartCardErrors.NoReadersAvailable:
							return new Exception("No readers available", ex);
						case SmartCardErrors.CommDataLost:
							return new IOException("Comm data lost", ex);
						case SmartCardErrors.NoKeyContainer:
							return new Exception("No key container", ex);
						case SmartCardErrors.ServerTooBusy:
							return new SystemException("Server too busy", ex);
						
						case SmartCardErrors.UnsupportedCard:
							return new NotSupportedException("Unsupported card", ex);
						case SmartCardErrors.UnresponsiveCard:
							return new IOException("UnresponsiveCard", ex);
						case SmartCardErrors.UnpoweredCard:
							return new IOException("Unpowered card", ex);
						case SmartCardErrors.ResetCard:
							return new IOException("Reset card", ex);
						case SmartCardErrors.RemovedCard:
							return new IOException("Removed card", ex);
						case SmartCardErrors.SecurityViolation:
							return new System.Security.SecurityException("Security violation", ex);
						case SmartCardErrors.WrongChv:
							return new Exception("Wrong CHV", ex);
						case SmartCardErrors.ChvBlocked:
							return new Exception("CHV blocked", ex);
						case SmartCardErrors.Eof:
							return new IOException("EOF", ex);
						case SmartCardErrors.CancelledByUser:
							return new Exception("Cancelled by user", ex);
						case SmartCardErrors.CardNotAuthenticated:
							return new Exception("Card not authenticated", ex);
						
						default:
							return new Exception("PC/SC error " + value.ToString() + " (" + returnValue.ToString("X") + ")", ex);
					}
				}
			}
		}

		[DllImport("Winscard.dll")]
		private static extern int SCardEstablishContext(SmartCardScope scope, IntPtr reserved1, IntPtr reserved2, out IntPtr context);

		[DllImport("Winscard.dll")]
		private static extern int SCardReleaseContext(IntPtr context);

		[DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
		private static extern int SCardListReaders(IntPtr context, IntPtr groups, out IntPtr readers, ref int length);

		[DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
		private static extern int SCardListReaders(IntPtr context, IntPtr groups, IntPtr readers, ref int length);

		[DllImport("Winscard.dll")]
		private static extern int SCardFreeMemory(IntPtr context, IntPtr ptr);

		[DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
		private static extern int SCardConnect(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string reader, SmartCardShare shareMode, SmartCardProtocols preferredProtocols, out IntPtr card, out SmartCardProtocols activeProtocol);

		[DllImport("Winscard.dll", CharSet = CharSet.Ansi)]
		private static extern int SCardGetStatusChange(IntPtr context, uint timeout, ref ReaderState readerState, uint count);
		
		private struct ReaderState {
			[MarshalAs(UnmanagedType.LPStr)]
			public string Reader;
			public IntPtr UserData;
			public SmartCardState CurrentState;
			public SmartCardState EventState;
			public uint AtrLength;
			public byte atr1;
			public byte atr2;
			public byte atr3;
			public byte atr4;
			public byte atr5;
			public byte atr6;
			public byte atr7;
			public byte atr8;
			public byte atr9;
			public byte atr10;
			public byte atr11;
			public byte atr12;
			public byte atr13;
			public byte atr14;
			public byte atr15;
			public byte atr16;
			public byte atr17;
			public byte atr18;
			public byte atr19;
			public byte atr20;
			public byte atr21;
			public byte atr22;
			public byte atr23;
			public byte atr24;
			public byte atr25;
			public byte atr26;
			public byte atr27;
			public byte atr28;
			public byte atr29;
			public byte atr30;
			public byte atr31;
			public byte atr32;
			public byte atr33;
		}

		[DllImport("Winscard.dll")]
		private static extern int SCardCancel(IntPtr context);
	}
}
