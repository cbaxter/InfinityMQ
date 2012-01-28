using System;
using System.Threading;

namespace InfinityMQ.Performance.Benchmarks
{
    internal class ByteCounter
    {
        private readonly Int64 expectedBytes;
        private Int64 clientBytesReceived;
        private Int64 serverBytesReceived;
        private Int64 clientBytesSent;
        private Int64 serverBytesSent;

        public Boolean AllClientBytesSent { get { return Interlocked.Read(ref this.clientBytesSent) == expectedBytes; } }
        public Boolean AllServerBytesSent { get { return Interlocked.Read(ref this.serverBytesSent) == expectedBytes; } }
        public Boolean AllClientBytesReceived { get { return Interlocked.Read(ref this.clientBytesReceived) == expectedBytes; } }
        public Boolean AllServerBytesReceived { get { return Interlocked.Read(ref this.serverBytesReceived) == expectedBytes; } }

        public ByteCounter(Int64 expectedBytes)
        {
            this.expectedBytes = expectedBytes;
        }

        public void CaptureClientBytesReceived(Int64 bytesReceived)
        {
            Interlocked.Add(ref this.clientBytesReceived, bytesReceived);
        }

        public void CaptureClientBytesSent(Int64 bytesSent)
        {
            Interlocked.Add(ref this.clientBytesSent, bytesSent);
        }

        public void CaptureServerBytesReceived(Int64 bytesReceived)
        {
            Interlocked.Add(ref this.serverBytesReceived, bytesReceived);
        }

        public void CaptureServerBytesSent(Int64 bytesSent)
        {
            Interlocked.Add(ref this.serverBytesSent, bytesSent);
        }
    }
}
