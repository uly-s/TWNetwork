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
    public class ActionIndexCacheSerializer
    {
        private static ConstructorInfo ActionIndexCacheCtr = typeof(ActionIndexCache).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,null,new Type[] { typeof(int) },null);
        [ProtoMember(1)]
        public int Index { get; set; }

        public ActionIndexCacheSerializer(ActionIndexCache actionIndexCache)
        {
            Index = actionIndexCache.Index;
        }

        public ActionIndexCacheSerializer() { }

        public static implicit operator ActionIndexCacheSerializer(ActionIndexCache actionIndexCache)
        {
            return new ActionIndexCacheSerializer(actionIndexCache);
        }

        public static implicit operator ActionIndexCache(ActionIndexCacheSerializer serializer)
        {
            return  (ActionIndexCache)ActionIndexCacheCtr.Invoke(new object[] { serializer.Index });
        }
    }
}
