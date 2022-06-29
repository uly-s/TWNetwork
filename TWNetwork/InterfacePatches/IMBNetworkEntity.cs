using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace TWNetwork.InterfacePatches
{
    public abstract class IMBNetworkEntity
    {
        private BinaryWriter Writer = null;
        private MemoryStream StreamForWriter = null;
        private MemoryStream StreamForReader = null;
        private BinaryReader Reader = null;
		protected MethodInfo HandleNetworkPacket = null;
		public static IMBNetworkEntity Entity { get; protected set; }
		protected void OnReceivePacket(byte[] packet)
		{
			StreamForReader = new MemoryStream(packet);
			Reader = new BinaryReader(StreamForReader);
		}
        protected void BeginModuleEvent()
        {
            StreamForWriter = new MemoryStream();
            Writer = new BinaryWriter(StreamForWriter);
        }
        protected void EndModuleEvent()
        {
            Writer.Close();
            StreamForWriter.Close();
            Writer = null;
            StreamForWriter = null;
        }
		protected byte[] GetBuffer()
		{
			return StreamForWriter.GetBuffer();
		}
		internal bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output)
		{
			try
			{
				output = Reader.ReadInt32();
				if (output < compressionInfo.GetMinimumValue() || output > compressionInfo.GetMaximumValue())
					return false;
				return true;
			}
			catch (Exception)
			{
				output = 0;
				return false;
			}
		}
		internal bool ReadUintFromPacket(ref CompressionInfo.UnsignedInteger compressionInfo, out uint output)
		{
			try
			{
				output = Reader.ReadUInt32();
				if (output < compressionInfo.GetMinimumValue() || output > compressionInfo.GetMaximumValue())
					return false;
				return true;
			}
			catch (Exception)
			{
				output = 0;
				return false;
			}
		}

		internal bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output)
		{
			try
			{
				output = Reader.ReadInt64();
				if (output < compressionInfo.GetMinimumValue() || output > compressionInfo.GetMaximumValue())
					return false;
				return true;
			}
			catch (Exception)
			{
				output = 0;
				return false;
			}
		}
		internal bool ReadUlongFromPacket(ref CompressionInfo.UnsignedLongInteger compressionInfo, out ulong output)
		{
			try
			{
				output = Reader.ReadUInt64();
				if (output < compressionInfo.GetMinimumValue() || output > compressionInfo.GetMaximumValue())
					return false;
				return true;
			}
			catch (Exception)
			{
				output = 0;
				return false;
			}
		}
		internal bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output)
		{
			try
			{
				output = Reader.ReadSingle();
				if (output < compressionInfo.GetMinimumValue() || output > compressionInfo.GetMaximumValue())
					return false;
				return true;
			}
			catch (Exception)
			{
				output = 0;
				return false;
			}
		}
		internal string ReadStringFromPacket(ref bool bufferReadValid)
		{
			if (bufferReadValid)
			{
				try
				{
					return Reader.ReadString();
				}
				catch (Exception)
				{
					bufferReadValid = false;
					return "";
				}
			}
			return "";
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
			if (bufferReadValid)
			{
				try
				{
					return Reader.Read(buffer, offset, bufferCapacity);
				}
				catch (Exception)
				{
					bufferReadValid = false;
					return 0;
				}
			}
			return 0;
		}
		internal void WriteByteArrayToPacket(byte[] value, int offset, int size)
		{
			if (Writer is null)
				return;
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
