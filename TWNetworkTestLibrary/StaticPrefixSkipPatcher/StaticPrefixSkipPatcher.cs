using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TWNetworkPatcher
{
	public static class StaticPrefixSkipPatcher
	{
		private static ConcurrentDictionary<MethodInfo, MethodInfo> methods = new ConcurrentDictionary<MethodInfo, MethodInfo>();
		public static void ApplyPatches() { new Harmony("TWNetwork.StaticPatcher").PatchAll(); }
		static StaticPrefixSkipPatcher()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes()) { 
				var PatchableMethods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).ToList();
					foreach (var method in PatchableMethods)
					{
						var attributes = method.GetCustomAttributes<PatchedMethodAttribute>().ToList();
						foreach (var attribute in attributes)
						{
							if (attribute.Method.Name == method.Name)
							{
								if (attribute.Method.ReturnType != method.ReturnType)
									throw new NotSameReturnTypeException($"The official method has {attribute.Method.ReturnType.Name} return type, the patcher method has {method.ReturnType.Name} return type.");
								if (attribute.Method.GetParameters().Length != method.GetParameters().Length)
									throw new NotSameAmountOfParametersException("The amount of parameters the official method and the patcher method has ar different.");
								for (int i = 0; i < attribute.Method.GetParameters().Length; i++)
								{
									if (attribute.Method.GetParameters()[i].ParameterType != method.GetParameters()[i].ParameterType)
									{
										throw new ParametersNotSameTypeException($"The {i + 1}. Parameter is different from each other in the two methods. Official: {attribute.Method.GetParameters()[i].ParameterType.Name} - Patcher: {method.GetParameters()[i].ParameterType.Name}");
									}
								}
								methods.TryAdd(attribute.Method, method);
							}
							else
							{
								throw new NameIsNotEqualException($"The official method and the patcher method has different names. {attribute.Method.Name} - {method.Name}");
							}
						}
					}
				}
			}
		}

		
		[HarmonyPatch]
		private class StaticMethodWithoutResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return methods.Keys.Where(m => m.ReturnType == typeof(void));
			}

			static bool Prefix(object[] __args, MethodBase __originalMethod)
			{
				var Method = methods[(MethodInfo)__originalMethod];
				Method.Invoke(null, __args);
				return false;
			}
		}

		[HarmonyPatch]
		private class StaticMethodWithResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return methods.Keys.Where(m => m.ReturnType != typeof(void));
			}

			static bool Prefix(out object __result,object[] __args, MethodBase __originalMethod)
			{
				var Method = methods[(MethodInfo)__originalMethod];
				__result = Method.Invoke(null, __args);
				return false;
			}
		}
	}
}
