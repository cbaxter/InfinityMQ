using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using InfinityMQ.Serialization.Serializers;

namespace InfinityMQ.Serialization
{
    internal class MessageWriter
    {
        private readonly FrameWriter frameWriter;
        private readonly ISerializeMessages serializer;
        private readonly Dictionary<Type, Frame> typeHeaders = new Dictionary<Type, Frame>();

        public MessageWriter(FrameWriter frameWriter)
            : this(frameWriter, new JsonMessageSerializer())
        { }

        public MessageWriter(FrameWriter frameWriter, ISerializeMessages serializer) //TODO: overload with bufferSize param (can pass in something like Socket.SendBufferSize etc; default 8192.
        {
            Verify.NotNull(frameWriter, "frameWriter");
            Verify.NotNull(serializer, "serializer");

            this.frameWriter = frameWriter;
            this.serializer = serializer;
        }

        public void Write(Object message)
        {
            if (message == null)
                return;

            WriteMessageHeaders(message);
            WriteMessageBody(message);
        }

        private void WriteMessageHeaders(Object message)
        {
            this.frameWriter.Write(GetOrCreateTypeFrame(message));
        }

        private Frame GetOrCreateTypeFrame(Object message)
        {
            Frame typeFrame;
            Type type = message.GetType();

            if (this.typeHeaders.TryGetValue(type, out typeFrame))
                return typeFrame;

            var assemblyQualifiedName = type.AssemblyQualifiedName ?? String.Empty;
            var versionlessAssemblyQualifiedName = Regex.Replace(assemblyQualifiedName, @", Version=\d+.\d+.\d+.\d+,", ",");

            this.typeHeaders[type] = typeFrame = new Frame(FrameFlags.Header | FrameFlags.More, Encoding.UTF8.GetBytes(versionlessAssemblyQualifiedName));

            return typeFrame;
        }

        private void WriteMessageBody(Object message)
        {
            using (var frameStream = new FramedOutputStream(this.frameWriter, 8192)) //TODO: see above
                this.serializer.Serialize(message, frameStream);
        }

        #region FramedOutputStream

        private class FramedOutputStream : Stream
        {
            private readonly FrameWriter frameWriter;
            private readonly Int32 bufferSize;
            private Int32 bufferOffset;
            private Byte[] frameBuffer;

            public FramedOutputStream(FrameWriter frameWriter, Int32 bufferSize)
            {
                Verify.NotNull(frameWriter, "frameWriter");

                this.frameWriter = frameWriter;
                this.bufferSize = bufferSize;
                this.frameBuffer = new Byte[bufferSize];
            }

            public override void Flush()
            {
                this.frameWriter.Write(new Frame(FrameFlags.None, new ArraySegment<Byte>(this.frameBuffer, 0, this.bufferOffset)));
                this.frameBuffer = new Byte[this.bufferSize];
                this.bufferOffset = 0;
            }

            public override Int64 Seek(Int64 offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(Int64 value)
            {
                throw new NotSupportedException();
            }

            public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
            {
                throw new NotSupportedException();
            }

            public override void Write(Byte[] buffer, Int32 offset, Int32 count)
            {
                do
                {
                    var availableSpace = this.bufferSize - this.bufferOffset;
                    var bytesToConsume = Math.Min(availableSpace, count);

                    Buffer.BlockCopy(buffer, offset, this.frameBuffer, this.bufferOffset, bytesToConsume);

                    this.bufferOffset += bytesToConsume;
                    offset += bytesToConsume;
                    count -= bytesToConsume;

                    if (this.bufferOffset < this.bufferSize)
                        continue;

                    this.frameWriter.Write(new Frame(FrameFlags.More, this.frameBuffer));
                    this.frameBuffer = new Byte[this.bufferSize];
                    this.bufferOffset = 0;
                } while (count > 0);
            }

            public override Boolean CanRead
            {
                get { return false; }
            }

            public override Boolean CanSeek
            {
                get { return false; }
            }

            public override Boolean CanWrite
            {
                get { return true; }
            }

            public override Int64 Length
            {
                get { throw new NotSupportedException(); }
            }

            public override Int64 Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }
        }

        #endregion
    }
}
