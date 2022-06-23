namespace TWNetwork.Extensions
{
    public class SendableOptions
    {
        public bool SendBlood { get; set; }
        public bool SendSoundEffects { get; set; }
        public SendableOptions()
        {
            SendBlood = true;
            SendSoundEffects = true;
        }
    }
}