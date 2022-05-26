namespace TWNetwork
{
    public interface IGameNetworkClient: IGameNetworkEntity
    {
        void BeginModuleEventAsClient();
        void BeginModuleEventAsClientUnreliable();
        void EndModuleEventAsClient();
        void EndModuleEventAsClientUnreliable();
    }
}