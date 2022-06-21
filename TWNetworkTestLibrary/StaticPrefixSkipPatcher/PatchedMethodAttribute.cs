using System;
using System.Reflection;

namespace TWNetworkPatcher
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PatchedMethodAttribute : Attribute
    {
        public MethodInfo Method { get; private set; }
        public PatchedMethodAttribute(Type type,string methodName,Type[] types)
        {
            Method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,null,types,null);
        }
        public PatchedMethodAttribute(Type type,string methodName)
        {
            Method = type.GetMethod(methodName,BindingFlags.Instance |BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}