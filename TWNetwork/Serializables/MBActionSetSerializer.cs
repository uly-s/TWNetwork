using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace MultiplayerBattle.Messages.Serializables
{
    [ProtoContract]
    public class MBActionSetSerializer
    {
        private static ConstructorInfo MBActionSetCtr = typeof(MBActionSet).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[] { typeof(int) }, null);
        [ProtoMember(1)]
        public int Index { get; set; }

        public MBActionSetSerializer(MBActionSet mBActionSet)
        {
            Index = (int)typeof(MBActionSet).GetField("Index",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mBActionSet);
        }

        public MBActionSetSerializer() { }

        public static implicit operator MBActionSetSerializer(MBActionSet mBActionSet)
        {
            return new MBActionSetSerializer(mBActionSet);
        }

        public static implicit operator MBActionSet(MBActionSetSerializer serializer)
        {
            return (MBActionSet)MBActionSetCtr.Invoke(new object[] { serializer.Index });
        }
    }
}
