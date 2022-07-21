using HarmonyLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;
using TWNetworkPatcher;

namespace TWNetworkTests
{
    [TestClass]
    public class ClassMultiplePatchTest
    {
        [TestInitialize]
        public void TestInit()
        {
            HarmonyPatcher.ApplyPatches();
        }
        [TestCleanup]
        public void TestCleanUp()
        {
            HarmonyPatcherTestClass.Count = 0; HarmonyPatcherTestClassPatches.Count = 0;
        }

        [TestMethod]
        public void TestStaticPrefixWithResultPatch()
        {
            bool result = HarmonyPatcherTestClass.StaticPrefixTestMethodWithResult(false);
            Assert.IsTrue(!result && HarmonyPatcherTestClassPatches.Count == 1 && HarmonyPatcherTestClass.Count == 0);
            result = HarmonyPatcherTestClass.StaticPrefixTestMethodWithResult(true);
            Assert.IsTrue(result && HarmonyPatcherTestClassPatches.Count == 2 && HarmonyPatcherTestClass.Count == 1);
        }

        [TestMethod]
        public void TestStaticPrefixWithoutResultPatch()
        {
            HarmonyPatcherTestClass.StaticPrefixTestMethodWithoutResult(false);
            Assert.IsTrue(HarmonyPatcherTestClassPatches.Count == 1 && HarmonyPatcherTestClass.Count == 0);
            Assert.ThrowsException<NotInstanceMethodException>(()=> HarmonyPatcherTestClass.StaticPrefixTestMethodWithoutResult(true));
        }

        [TestMethod]
        public void TestInstancePrefixWithoutResultPatch()
        {
            HarmonyPatcherTestClass tester = new HarmonyPatcherTestClass();
            tester.InstancePrefixTestMethodWithoutResult(false);
            Assert.IsTrue(tester.Counter == 2 && HarmonyPatcherTestClass.Count == 0 && HarmonyPatcherTestClassPatches.Count == 1);
            tester.InstancePrefixTestMethodWithoutResult(true);
            Assert.IsTrue(tester.Counter == 5 && HarmonyPatcherTestClass.Count == 1 && HarmonyPatcherTestClassPatches.Count == 2);
        }

        [TestMethod]
        public void TestInstancePrefixWithResultPatch()
        {
            HarmonyPatcherTestClass tester = new HarmonyPatcherTestClass();
            bool result = tester.InstancePrefixTestMethodWithResult(false);
            Assert.IsTrue(result && HarmonyPatcherTestClass.Count == 0 && HarmonyPatcherTestClassPatches.Count == 1);
            result = tester.InstancePrefixTestMethodWithResult(true);
            Assert.IsTrue(!result && HarmonyPatcherTestClass.Count == 1 && HarmonyPatcherTestClassPatches.Count == 2);
        }

        [TestMethod]
        public void TestStaticPostfixWithResultPatch()
        {
            bool result = HarmonyPatcherTestClass.StaticPostfixTestMethodWithResult();
            Assert.IsTrue(!result && HarmonyPatcherTestClassPatches.Count == 1 && HarmonyPatcherTestClass.Count == 1);
        }

        [TestMethod]
        public void TestStaticPostfixWithoutResultPatch()
        {
            HarmonyPatcherTestClass.StaticPostfixTestMethodWithoutResult();
            Assert.IsTrue(HarmonyPatcherTestClassPatches.Count == 1 && HarmonyPatcherTestClass.Count == 1);
        }

        [TestMethod]
        public void TestInstancePostfixWithoutResultPatch()
        {
            HarmonyPatcherTestClass tester = new HarmonyPatcherTestClass();
            tester.InstancePostfixTestMethodWithoutResult();
            Assert.IsTrue(tester.Counter == 3 && HarmonyPatcherTestClass.Count == 1 && HarmonyPatcherTestClassPatches.Count == 1);
        }

        [TestMethod]
        public void TestInstancePostfixWithResultPatch()
        {
            HarmonyPatcherTestClass tester = new HarmonyPatcherTestClass();
            bool result = tester.InstancePostfixTestMethodWithResult();
            Assert.IsTrue(!result && HarmonyPatcherTestClass.Count == 1 && HarmonyPatcherTestClassPatches.Count == 1);
        }

        [TestMethod]
        public void TestInterfaceImplementer()
        {
            var testobject = new TestInterfaceImplementation();
            TestInterface obj = (TestInterface)testobject.GetTransparentProxy();
            Assert.IsTrue(testobject.Num1 == -1 && testobject.Num2 == -1);
            obj.Valami(2,4.5);
            Assert.IsTrue(testobject.Num1 == 2 && testobject.Num2 == 4.5);
            Assert.IsTrue(obj.Ez("valami"));
            Assert.IsTrue(testobject.Something == "valami");
            Assert.IsTrue(!obj.Ez(null));
            obj.Number = 2;
            Assert.IsTrue(obj.Number == 2);
        }

        [TestMethod]
        public void TestGetterSetterPatch()
        {
            HarmonyPatcherTestClass tester = new HarmonyPatcherTestClass();
            tester.Prop = 2;
            Assert.IsTrue(HarmonyPatcherTestClassPatches.propvalue == 2 && HarmonyPatcherTestClassPatches.Count == 1);
            int value = tester.Prop;
            Assert.IsTrue(value == 2 &&  value == HarmonyPatcherTestClassPatches.propvalue && HarmonyPatcherTestClassPatches.Count == 2);
        }
    }
}
