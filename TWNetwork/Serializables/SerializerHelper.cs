using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.ObjectSystem;

namespace MultiplayerBattle.Messages.Serializables
{
    public static class SerializerHelper
    {
        public static MBObjectBase GetObjectFromRef(uint reference)
            => (reference > 0U) ?MBObjectManager.Instance.GetObject(new MBGUID(reference)):null;
        public static uint GetReferenceFromObject(MBObjectBase Object)
            => (Object != null) ? Object.Id.InternalValue : 0U;
    }
}
