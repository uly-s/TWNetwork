using HarmonyLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TWNetworkPatcher
{
	public static class HarmonyPatcher
	{
		private static Harmony harmony = new Harmony("TWNetwork.HarmonyPatcher");
		private static bool PatchAlreadyApplied = false;
		private static ConcurrentDictionary<MethodInfo, (PatchedMethodAttribute Attribute,MethodInfo Method)> methods = new ConcurrentDictionary<MethodInfo, (PatchedMethodAttribute Attribute,MethodInfo Method)>();
		private static IEnumerable<MethodBase> StaticPrefixMethodWithoutResultPatcherMethods => methods.Keys.Where(m => m.ReturnType == typeof(void) && methods[m].Attribute.IsPrefix && m.IsStatic);
		private static IEnumerable<MethodBase> StaticPrefixMethodWithResultPatcherMethods => methods.Keys.Where(m => m.ReturnType != typeof(void) && methods[m].Attribute.IsPrefix && m.IsStatic);
		private static IEnumerable<MethodBase> StaticPostfixMethodWithoutResultPatcherMethods => methods.Keys.Where(m => m.ReturnType == typeof(void) && !methods[m].Attribute.IsPrefix && m.IsStatic);
		private static IEnumerable<MethodBase> StaticPostfixMethodWithResultPatcherMethods => methods.Keys.Where(m => m.ReturnType != typeof(void) && !methods[m].Attribute.IsPrefix && m.IsStatic);
		private static IEnumerable<MethodBase> InstancePrefixMethodWithoutResultPatcherMethods => methods.Keys.Where(m => m.ReturnType == typeof(void) && methods[m].Attribute.IsPrefix && !m.IsStatic);
		private static IEnumerable<MethodBase> InstancePrefixMethodWithResultPatcherMethods => methods.Keys.Where(m => m.ReturnType != typeof(void) && methods[m].Attribute.IsPrefix && !m.IsStatic);
		private static IEnumerable<MethodBase> InstancePostfixMethodWithoutResultPatcherMethods => methods.Keys.Where(m => m.ReturnType == typeof(void) && !methods[m].Attribute.IsPrefix && !m.IsStatic);
		private static IEnumerable<MethodBase> InstancePostfixMethodWithResultPatcherMethods => methods.Keys.Where(m => m.ReturnType != typeof(void) && !methods[m].Attribute.IsPrefix && !m.IsStatic);
		public static void ApplyPatches() 
		{
			if (PatchAlreadyApplied)
				return;
			if (StaticPrefixMethodWithoutResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(StaticPrefixMethodWithoutResultPatcher)).Patch();
			}
			if (StaticPrefixMethodWithResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(StaticPrefixMethodWithResultPatcher)).Patch();
			}
			if (StaticPostfixMethodWithoutResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(StaticPostfixMethodWithoutResultPatcher)).Patch();
			}
			if (StaticPostfixMethodWithResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(StaticPostfixMethodWithResultPatcher)).Patch();
			}
			if (InstancePrefixMethodWithoutResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(InstancePrefixMethodWithoutResultPatcher)).Patch();
			}
			if (InstancePrefixMethodWithResultPatcherMethods.Count() > 0)
			{
				var list = InstancePrefixMethodWithResultPatcherMethods.ToList();
				harmony.CreateClassProcessor(typeof(InstancePrefixMethodWithResultPatcher)).Patch();
			}
			if (InstancePostfixMethodWithoutResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(InstancePostfixMethodWithoutResultPatcher)).Patch();
			}
			if (InstancePostfixMethodWithResultPatcherMethods.Count() > 0)
			{
				harmony.CreateClassProcessor(typeof(InstancePostfixMethodWithResultPatcher)).Patch();
			}
			PatchAlreadyApplied = true;
		}
		static HarmonyPatcher()
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach(Type type in assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(HarmonyPatches)))) { 
				var PatchableMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).ToList();
					foreach (var method in PatchableMethods)
					{
						var attributes = method.GetCustomAttributes<PatchedMethodAttribute>().ToList();
						if (method.IsStatic && attributes.Count > 0)
							throw new StaticPatcherMethodException($"The method called {method.Name} in {method.DeclaringType.Name} class has PatchedMethod attribute, but is static, which is not allowed.");
						foreach (var attribute in attributes)
						{
							if (attribute.Method.Name == method.Name)
							{
								if (attribute.Method.ReturnType != method.ReturnType)
								{ 
									throw new NotSameReturnTypeException($"General method: The official method has {attribute.Method.ReturnType.Name} return type, the patcher method has {method.ReturnType.Name} return type."); 
								}
								if(attribute.Method.GetParameters().Length != method.GetParameters().Length)
								{
									throw new NotGoodAmountOfParametersException("The amount of parameters the patcher method has are not the same amount of the original method.");
								}
								for (int i = 0; i < attribute.Method.GetParameters().Length; i++)
								{
									if (attribute.Method.GetParameters()[i].ParameterType != method.GetParameters()[i].ParameterType)
									{
										throw new ParametersNotSameTypeException($"The {i + 1}. Parameter is different from each other in the two methods. Official: {attribute.Method.GetParameters()[i].ParameterType.Name} - Patcher: {method.GetParameters()[i].ParameterType.Name}");
									}
								}
								methods.TryAdd(attribute.Method, (attribute,method));
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
		private class StaticPrefixMethodWithoutResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return StaticPrefixMethodWithoutResultPatcherMethods;
			}

			static bool Prefix(object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod,methods[(MethodInfo)__originalMethod].Method, __args);
				return Result.Run;
			}
		}

		[HarmonyPatch]
		private class StaticPrefixMethodWithResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return StaticPrefixMethodWithResultPatcherMethods;
			}

			static bool Prefix(out object __result,object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args);
				__result = Result.Result;
				return Result.Run;
			}
		}

		[HarmonyPatch]
		private class StaticPostfixMethodWithoutResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return StaticPostfixMethodWithoutResultPatcherMethods;
			}

			static void Postfix(object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args);
			}
		}

		[HarmonyPatch]
		private class StaticPostfixMethodWithResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return StaticPostfixMethodWithResultPatcherMethods;
			}

			static void Postfix(out object __result, object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args);
				__result = Result.Result;
			}
		}

		[HarmonyPatch]
		private class InstancePrefixMethodWithoutResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return InstancePrefixMethodWithoutResultPatcherMethods;
			}

			static bool Prefix(object __instance ,object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args,__instance);
				return Result.Run;
			}
		}

		[HarmonyPatch]
		private class InstancePrefixMethodWithResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return InstancePrefixMethodWithResultPatcherMethods;
			}

			static bool Prefix(object __instance, out object __result, object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args,__instance);
				__result = Result.Result;
				return Result.Run;
			}
		}

		[HarmonyPatch]
		private class InstancePostfixMethodWithoutResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return InstancePostfixMethodWithoutResultPatcherMethods;
			}

			static void Postfix(object __instance, object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args,__instance);
			}
		}

		[HarmonyPatch]
		private class InstancePostfixMethodWithResultPatcher
		{
			static IEnumerable<MethodBase> TargetMethods()
			{
				return InstancePostfixMethodWithResultPatcherMethods;
			}

			static void Postfix(object __instance, out object __result, object[] __args, MethodBase __originalMethod)
			{
				HarmonyPatches Result = HarmonyPatches.InvokeMethod((MethodInfo)__originalMethod, methods[(MethodInfo)__originalMethod].Method, __args,__instance);
				__result = Result.Result;
			}
		}
	}
}
