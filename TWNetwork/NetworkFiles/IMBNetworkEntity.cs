using System;
using System.IO;
using System.Reflection;
using System.Threading;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.NetworkFiles
{
    public abstract class IMBNetworkEntity
    {
        private BinaryWriter Writer = null;
        private MemoryStream StreamForWriter = null;
        private MemoryStream StreamForReader = null;
        private BinaryReader Reader = null;
		private readonly object ReaderObject = new object();
		private readonly object WriterObject = new object();
		protected MethodInfo HandleNetworkPacket = null;
		public static IMBNetworkEntity Entity { get; protected set; }
		private void OnReceivePacketBegin(byte[] packet)
		{
			StreamForReader = new MemoryStream(packet);
			Reader = new BinaryReader(StreamForReader);
		}

		private void OnReceivePacketEnd()
		{
			Reader.Close();
			StreamForReader.Close();
			Reader = null;
			StreamForReader = null;
		}

		/// <summary>
		/// This method should be called from the client or the server, when a GameNetworkMessage is received.
		/// </summary>
		/// <param name="packet">The packet in a byte array.</param>
		protected void HandleNetworkPacketAsEntity(byte[] packet, object[] args)
		{
			lock (ReaderObject) 
			{
				OnReceivePacketBegin(packet);
				while (StreamForReader.Position < packet.Length)
				{
					HandleNetworkPacket?.Invoke(null, args);
				}
				OnReceivePacketEnd();
			}
		}
        protected void BeginModuleEvent()
        {
			Monitor.Enter(WriterObject);
            StreamForWriter = new MemoryStream();
            Writer = new BinaryWriter(StreamForWriter);
        }

        protected void EndModuleEvent()
        {
            Writer.Close();
            StreamForWriter.Close();
            Writer = null;
            StreamForWriter = null;
			Monitor.Exit(WriterObject);
        }

		protected byte[] GetBuffer()
		{
			return StreamForWriter.ToArray();
		}
		internal bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output)
		{
			output = Reader.ReadInt32();
			return output >= compressionInfo.GetMinimumValue() && output <= compressionInfo.GetMaximumValue();
		}
		internal bool ReadUintFromPacket(ref CompressionInfo.UnsignedInteger compressionInfo, out uint output)
		{
			output = Reader.ReadUInt32();
			return output >= compressionInfo.GetMinimumValue() && output <= compressionInfo.GetMaximumValue();
		}

		internal bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output)
		{
			output = Reader.ReadInt64();
			return output >= compressionInfo.GetMinimumValue() && output <= compressionInfo.GetMaximumValue();
		}
		internal bool ReadUlongFromPacket(ref CompressionInfo.UnsignedLongInteger compressionInfo, out ulong output)
		{
			output = Reader.ReadUInt64();
			return output >= compressionInfo.GetMinimumValue() && output <= compressionInfo.GetMaximumValue();
		}
		internal bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output)
		{
			output = Reader.ReadSingle();
			return output >= compressionInfo.GetMinimumValue() && output <= compressionInfo.GetMaximumValue();
		}
		internal string ReadStringFromPacket(ref bool bufferReadValid)
		{
			return Reader.ReadString();
		}

		internal void WriteIntToPacket(int value, ref TaleWorlds.MountAndBlade.CompressionInfo.Integer compressionInfo)
		{
			if (Writer is null)
				return;
			if (value < compressionInfo.GetMinimumValue() || value > compressionInfo.GetMaximumValue())
				throw new InvalidDataException();
			Writer.Write(value);
		}

		internal void WriteUintToPacket(uint value, ref CompressionInfo.UnsignedInteger compressionInfo)
		{
			if (Writer is null)
				return;
			if (value < compressionInfo.GetMinimumValue() || value > compressionInfo.GetMaximumValue())
				throw new InvalidDataException();
			Writer.Write(value);
		}

		internal void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo)
		{
			if (Writer is null)
				return;
			if (value < compressionInfo.GetMinimumValue() || value > compressionInfo.GetMaximumValue())
				throw new InvalidDataException();
			Writer.Write(value);
		}
		internal void WriteUlongToPacket(ulong value, ref CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			if (Writer is null)
				return;
			if (value < compressionInfo.GetMinimumValue() || value > compressionInfo.GetMaximumValue())
				throw new InvalidDataException();
			Writer.Write(value);
		}
		internal void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo)
		{
			if (Writer is null)
				return;
			if (value < compressionInfo.GetMinimumValue() || value > compressionInfo.GetMaximumValue())
				throw new InvalidDataException();
			Writer.Write(value);
		}
		internal void WriteStringToPacket(string value)
		{
			if (Writer is null)
				return;
			if (value is null)
				throw new InvalidDataException();
			Writer.Write(value);
		}

		internal int ReadByteArrayFromPacket(byte[] buffer, int offset, int bufferCapacity, ref bool bufferReadValid)
		{
			int size = Reader.ReadInt32();
			Reader.Read(buffer, offset, size);
			return size;
		}
		internal void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			if (Writer is null)
				return;
			Writer.Write(size);
			Writer.Write(value, offset, size);
		}
	}

	internal static class CompressionInfoExtensions
	{
		internal static ulong GetMinimumValue(this CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			return (ulong)compressionInfo.GetType().GetField("minimumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static ulong GetMaximumValue(this CompressionInfo.UnsignedLongInteger compressionInfo)
		{
			return (ulong)compressionInfo.GetType().GetField("maximumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static long GetMinimumValue(this CompressionInfo.LongInteger compressionInfo)
		{
			return (long)compressionInfo.GetType().GetField("minimumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static long GetMaximumValue(this CompressionInfo.LongInteger compressionInfo)
		{
			return (long)compressionInfo.GetType().GetField("maximumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static uint GetMinimumValue(this CompressionInfo.UnsignedInteger compressionInfo)
		{
			return (uint)compressionInfo.GetType().GetField("minimumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static uint GetMaximumValue(this CompressionInfo.UnsignedInteger compressionInfo)
		{
			return (uint)compressionInfo.GetType().GetField("maximumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}

		internal static int GetMinimumValue(this CompressionInfo.Integer compressionInfo)
		{
			return (int)compressionInfo.GetType().GetField("minimumValue", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(compressionInfo);
		}
	}
}
