using TaleWorlds.MountAndBlade.Network.Messages;

namespace TWNetwork
{
    public class GameNetworkClient: IGameNetworkEntity
    {
        public GameNetworkMessage MessageToSend { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void BeginModuleEventAsClient()
        { 
        }
        public void BeginModuleEventAsClientUnreliable()
        {
            
        }
        public void EndModuleEventAsClient()
        {
            
        }
        public void EndModuleEventAsClientUnreliable()
        {
            
        }
        public void InitializeClientSide()
        {
            
        }
        public void TerminateClientSide()
        {
            
        }

    }
}