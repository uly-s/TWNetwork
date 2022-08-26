using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWNetworkHelper;

namespace TWNetworkTests
{
    public class HarmonyPatcherTestClassPatches :HarmonyPatches
    {
		public static int propvalue { get; private set; }
		public static int Count = 0;
		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.StaticPrefixTestMethodWithResult),new Type[] { typeof(bool) },true)]
		public bool StaticPrefixTestMethodWithResult(bool shouldRun)
		{
			Count++;
			Run = shouldRun;
			return false;
		}
		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.InstancePrefixTestMethodWithResult), new Type[] { typeof(bool)},true)]
		public bool InstancePrefixTestMethodWithResult(bool shouldRun)
		{
			Count++;
			Run = shouldRun;
			return true;
		}

		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.StaticPrefixTestMethodWithoutResult),new Type[] { typeof(bool)},true)]
		public void StaticPrefixTestMethodWithoutResult(bool GetInstance)
		{
			if (GetInstance)
			{
				object obj = Instance;
			}
			Count++;
		}

		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.InstancePrefixTestMethodWithoutResult),new Type[] { typeof(bool) },true)]
		public void InstancePrefixTestMethodWithoutResult(bool shouldRun)
		{
			Count++;
			Run = shouldRun;
			((HarmonyPatcherTestClass)Instance).AddTwo();
		}
        [PatchedMethod(typeof(HarmonyPatcherTestClass),nameof(HarmonyPatcherTestClass.StaticPostfixTestMethodWithResult),false)]
		public bool StaticPostfixTestMethodWithResult()
		{
			Count++;
			return false;
		}
		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.StaticPostfixTestMethodWithoutResult), false)]
		public void StaticPostfixTestMethodWithoutResult()
		{
			Count++;
		}
		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.InstancePostfixTestMethodWithResult), false)]
		public bool InstancePostfixTestMethodWithResult()
		{
			Count++;
			return false;
		}
		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.InstancePostfixTestMethodWithoutResult), false)]
		public void InstancePostfixTestMethodWithoutResult()
		{
			((HarmonyPatcherTestClass)Instance).AddTwo();
			Count++;
		}
		[PatchedMethod(typeof(HarmonyPatcherTestClass),nameof(HarmonyPatcherTestClass.Prop),true,MethodType.Setter)]
		private void set_Prop(int value)
		{
			Count++;
			propvalue = value;
		}

		[PatchedMethod(typeof(HarmonyPatcherTestClass), nameof(HarmonyPatcherTestClass.Prop), false, MethodType.Getter)]
		private int get_Prop()
		{
			Count++;
			return propvalue;
		}
	}
}
