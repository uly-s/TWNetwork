using System;

namespace TWNetworkTests
{
    public class HarmonyPatcherTestClass
    {
        public static int Count = 0;
        public int Prop { get; set; }
        public int Counter { get; private set; }
        private void AddOne()
        {
            Counter += 1;
        }

        public void AddTwo()
        {
            Counter += 2;
        }
        public static bool StaticPrefixTestMethodWithResult(bool shouldRun)
        {
            Count++;
            return true;
        }

        public static void StaticPrefixTestMethodWithoutResult(bool GetInstance)
        {
            Count++;
        }

        public bool InstancePrefixTestMethodWithResult(bool shouldRun)
        {
            Count++;
            return false;
        }

        public void InstancePrefixTestMethodWithoutResult(bool shouldRun)
        {
            AddOne();
            Count++;
        }

        public static bool StaticPostfixTestMethodWithResult()
        {
            Count++;
            return true;
        }

        public static void StaticPostfixTestMethodWithoutResult()
        {
            Count++;
        }

        public bool InstancePostfixTestMethodWithResult()
        {
            Count++;
            return true;
        }

        public void InstancePostfixTestMethodWithoutResult()
        {
            AddOne();
            Count++;
        }
    }
}