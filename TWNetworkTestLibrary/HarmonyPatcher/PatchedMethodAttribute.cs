using System;
using System.Reflection;

namespace TWNetworkPatcher
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchedMethodAttribute : Attribute
    {
        public MethodInfo Method { get; private set; }
        public bool IsPrefix { get; private set; }
        public PatchedMethodAttribute(Type type,string methodName,Type[] types,bool IsPrefix)
        {
            Method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,null,types,null);
            this.IsPrefix = IsPrefix;
        }
        public PatchedMethodAttribute(Type type,string methodName,bool IsPrefix)
        {
            Method = type.GetMethod(methodName,BindingFlags.Instance |BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type,string assemblyIncludedTypeName, string methodName, Type[] types, bool IsPrefix)
        {
            Method = type.Assembly.GetType(assemblyIncludedTypeName).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, types, null);
            this.IsPrefix = IsPrefix;
        }
        public PatchedMethodAttribute(Type type, string assemblyIncludedTypeName,string methodName, bool IsPrefix)
        {
            Method = type.Assembly.GetType(assemblyIncludedTypeName).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type, string assemblyIncludedTypeName, string methodName,Type[] types, string[] typenames, bool IsPrefix)
        {
            if (types.Length != typenames.Length)
                throw new InvalidOperationException();
            Type [] alltypes = new Type[types.Length];
            for (int i = 0; i < alltypes.Length; i++)
            {
                if (types[i] != null)
                {
                    alltypes[i] = types[i];
                }
                else if (typenames[i]!=null)
                {
                    alltypes[i] = type.Assembly.GetType(typenames[i]);
                }
            }
            Method = type.Assembly.GetType(assemblyIncludedTypeName).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,null,alltypes,null);
            this.IsPrefix = IsPrefix;
        }
    }
}