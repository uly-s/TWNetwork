namespace TWNetwork.NetworkFiles
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