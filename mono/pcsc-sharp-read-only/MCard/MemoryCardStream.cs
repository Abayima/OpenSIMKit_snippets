/*
 * MemoryCardStream.cs: Memory card Stream
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

namespace PcSc.MCard
{
	public class MemoryCardStream : Stream
	{
		protected MemoryCard card;
		protected int zone;
		private long position = 0;

		public MemoryCardStream(MemoryCard card, int zone) {
			this.card = card;
			this.zone = zone;
		}

		public override bool CanRead {
			get {
				return true;
			}
		}

		public override bool CanWrite {
			get {
				return false;
			}
		}

		public override bool CanSeek {
			get {
				return false;
			}
		}

		public override long Length {
			get {
				throw new NotImplementedException();
			}
		}

		public override void SetLength(long value) {
			throw new Exception("The method or operation is not implemented.");
		}

		public override long Position {
			get {
				return position;
			}
			set {
				position = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin) {
			switch (origin) {
				case SeekOrigin.Begin:
					return Position = offset;
				case SeekOrigin.Current:
					return Position += offset;
				default:
					throw new InvalidOperationException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count) {
			try {
				int bytesRead = card.ReadMemory(zone, (int)position, buffer, offset, count);
				Position += bytesRead;
				return bytesRead;
			} catch (Exception ex) {
				throw new IOException(null, ex);
			}
		}

		public override int ReadByte() {
			int ret = card.ReadMemoryByte(zone, (int)position);
			if (ret != -1)
				position++;
			return ret;
		}

		public override void Write(byte[] buffer, int offset, int count) {
			throw new NotImplementedException();
		}

		public override void Flush() {
		}
	}
}
