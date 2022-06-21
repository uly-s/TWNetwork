using System;

namespace TWNetworkTests
{
    public static class GameNetwork
    {
        public static int Count { get; private set; } = 0;
        public static void BeginModuleEventAsClient()
        {
            Count++;
        }

        public static void EndModuleEventAsClient()
        {
            Count++;
        }

        public static void BeginModuleEventAsClientUnreliable()
        {
            Count++;
        }

        public static void EndModuleEventAsClientUnreliable()
        {
            Count++;
        }

        public static int GetOne()
        {
            Count++;
            return 1;
        }
    }
}