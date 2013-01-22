/*
 * Context.cs: Memory card context
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
	public class MemoryCardContext : MarshalByRefObject, IDisposable
	{
		IntPtr context;

		public MemoryCardContext(SmartCardContext context, string reader) {
			uint version;
			int ret = MCardInitialize(context.Handle, reader, out this.context, out version);
			if (ret != 0)
				throw new Exception(ret.ToString("x"));
		}

		~MemoryCardContext() {
			Dispose(false);
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing) {
			if (context != IntPtr.Zero)
				MCardShutdown(context);
		}

		public MemoryCard Connect() {
			IntPtr card;
			int ret = MCardConnect(context, MemoryCardConnectMode.Intelligent, MemoryCardType.Unknown, out card);
			if (ret != 0)
				throw new Exception(ret.ToString("x"));
			return new MemoryCard(card);
		}

		public void WaitForState(SmartCardState state, int timeout) {
			SmartCardState retState;
			int ret = MCardWaitForCardState(context, state, out retState, (uint)timeout);
			if (ret != 0)
				throw new Exception(ret.ToString("X"));
		}

		[DllImport("MCChipdr.dll")]
		private static extern int MCardInitialize(IntPtr context, [MarshalAs(UnmanagedType.LPStr)] string reader, out IntPtr mContext, out uint version);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardShutdown(IntPtr context);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardConnect(IntPtr context, MemoryCardConnectMode mode, MemoryCardType type, out IntPtr handle);

		[DllImport("MCChipdr.dll")]
		private static extern int MCardWaitForCardState(IntPtr context, SmartCardState expectedCardState, out SmartCardState cardState, uint timeout);

	}
}
