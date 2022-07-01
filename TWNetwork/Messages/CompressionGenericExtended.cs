using TaleWorlds.MountAndBlade;

namespace TWNetwork.Messages
{
    public static class CompressionGenericExtended
    {
        public static CompressionInfo.UnsignedInteger EventControlFlagCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);
        public static CompressionInfo.UnsignedInteger MovementFlagCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);
    }
}
