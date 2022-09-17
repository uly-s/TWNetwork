using System;
using System.Reflection;

namespace TWNetworkHelper
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchedMethodAttribute : Attribute
    {
        private static BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public MethodInfo Method { get; private set; }
        public bool IsPrefix { get; private set; }
        public PatchedMethodAttribute(Type type,string methodName,Type[] types,bool IsPrefix)
        {
            Method = type.GetMethod(methodName, Flags, null, types, null);
            this.IsPrefix = IsPrefix;
        }

        public PatchedMethodAttribute(Type type,string methodorpropertyname,bool IsPrefix, MethodType methodType = MethodType.Normal)
        {
            switch (methodType)
            {
                case MethodType.Normal:
                    Method = type.GetMethod(methodorpropertyname, Flags);
                    break;
                case MethodType.Setter:
                    Method = type.GetProperty(methodorpropertyname, Flags).SetMethod;
                    break;
                case MethodType.Getter:
                    Method = type.GetProperty(methodorpropertyname, Flags).GetMethod;
                    break;
            }
            this.IsPrefix = IsPrefix;
        }
    }
}