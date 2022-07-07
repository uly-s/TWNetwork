using System;
using System.Collections.Generic;
using System.Reflection;

namespace TWNetworkPatcher
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchedMethodAttribute : Attribute
    {
        private static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public MethodInfo Method { get; private set; }
        public bool IsPrefix { get; private set; }
        public PatchedMethodAttribute(Type type,string methodName,Type[] types,bool IsPrefix)
        {
            Method = type.GetMethod(methodName, Flags,null,types,null);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type, string methodName, string[] typenames, bool IsPrefix)
        {
            Type[] types = new Type[typenames.Length];
            for(int i=0;i < typenames.Length;i++)
            {
                types[i] = Type.GetType(typenames[i]);
            }
            Method = type.GetMethod(methodName, Flags, null, types, null);
            this.IsPrefix = IsPrefix;
        }
        public PatchedMethodAttribute(Type type,string methodName,bool IsPrefix)
        {
            Method = type.GetMethod(methodName, Flags);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type,string fieldname, string methodName, Type[] types, bool IsPrefix)
        {
            Method = type.GetField(fieldname, Flags).FieldType.GetMethod(methodName, Flags, null, types, null);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type, string fieldname, string methodName, Type[] types,bool[] IsReferenceOrOutTypes, bool IsPrefix)
        {
            if (types.Length != IsReferenceOrOutTypes.Length)
                throw new InvalidOperationException();
            for (int i = 0; i < types.Length; i++)
            {
                if (IsReferenceOrOutTypes[i])
                    types[i] = types[i].MakeByRefType();
            }
            Method = type.GetField(fieldname, Flags).FieldType.GetMethod(methodName, Flags, null,types, null);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type, string fieldname, string methodName, bool IsPrefix)
        {
            Method = type.GetField(fieldname, Flags).FieldType.GetMethod(methodName, Flags);
            this.IsPrefix = IsPrefix;
        }
    }
}