using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace MultiplayerBattle
{
    public static class ProtobufNetHelper
    {
		private static bool IsCalled = false;
		public static void MakeGameNetworkMessagesProtobufSerializable()
		{
			if (!IsCalled)
			{
				var BaseType = RuntimeTypeModel.Default.Add(typeof(GameNetworkMessage));
				BaseType.Add("MessageId");
				List<Type> FromServerMessages = (List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromServer", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				List<Type> FromClientMessages = (List<Type>)typeof(GameNetwork).GetField("_gameNetworkMessageIdsFromClient", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
				int i = 2;
				foreach (Type type in FromServerMessages)
				{
					var MetaType = RuntimeTypeModel.Default.Add(type);
					BaseType.AddSubType(i++, type);
					var Fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => !f.IsLiteral && !f.IsInitOnly);
					foreach (var f in Fields)
					{
						MetaType.Add(f.Name);
						Type t = f.FieldType;
						if (!RuntimeTypeModel.Default.CanSerialize(t))
						{
							Type surrt = Type.GetType($"MultiplayerBattle.Messages.Serializables.{t.Name}Serializer");
							RuntimeTypeModel.Default.Add(t).SetSurrogate(surrt);
						}
					}
				}
				foreach (Type type in FromClientMessages)
				{
					var MetaType = RuntimeTypeModel.Default.Add(type);
					BaseType.AddSubType(i++, type);
					var Fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(f => !f.IsLiteral && !f.IsInitOnly);
					foreach (var f in Fields)
					{
						MetaType.Add(f.Name);
						Type t = f.FieldType;
						if (!RuntimeTypeModel.Default.CanSerialize(t))
						{
							Type surrt = Type.GetType($"MultiplayerBattle.Messages.Serializables.{t.Name}Serializer");
							RuntimeTypeModel.Default.Add(t).SetSurrogate(surrt);
						}
					}
				}
				IsCalled = true;
			}
		}
	}
}
