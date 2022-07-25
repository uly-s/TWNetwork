using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TWNetworkTestMod
{
	public class GameNetworkHandler : IGameNetworkHandler
	{
		void IGameNetworkHandler.OnNewPlayerConnect(PlayerConnectionInfo playerConnectionInfo, NetworkCommunicator networkPeer)
		{
			if (networkPeer != null)
			{
				GameManagerBase.Current.OnPlayerConnect(networkPeer.VirtualPlayer);
			}
		}

		void IGameNetworkHandler.OnInitialize()
		{
			MultiplayerGameTypes.Initialize();
		}

		void IGameNetworkHandler.OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
		{
			if (Mission.Current != null)
			{
				using (List<MissionBehavior>.Enumerator enumerator = Mission.Current.MissionBehaviors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionNetwork missionNetwork;
						if ((missionNetwork = (enumerator.Current as MissionNetwork)) != null)
						{
							missionNetwork.OnPlayerConnectedToServer(networkPeer);
						}
					}
				}
			}
		}

		void IGameNetworkHandler.OnDisconnectedFromServer()
		{
			if (Mission.Current != null)
			{
				Mission.Current.EndMission();
			}
		}

		void IGameNetworkHandler.OnPlayerDisconnectedFromServer(NetworkCommunicator networkPeer)
		{
			GameManagerBase.Current.OnPlayerDisconnect(networkPeer.VirtualPlayer);
			if (Mission.Current != null)
			{
				using (List<MissionBehavior>.Enumerator enumerator = Mission.Current.MissionBehaviors.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MissionNetwork missionNetwork;
						if ((missionNetwork = (enumerator.Current as MissionNetwork)) != null)
						{
							missionNetwork.OnPlayerDisconnectedFromServer(networkPeer);
						}
					}
				}
			}
		}

		void IGameNetworkHandler.OnStartMultiplayer()
		{
			GameNetwork.AddNetworkComponent<TWNetworkComponent>();
			GameManagerBase.Current.OnGameNetworkBegin();
		}

		// Token: 0x06002731 RID: 10033 RVA: 0x0009717E File Offset: 0x0009537E
		void IGameNetworkHandler.OnEndMultiplayer()
		{
			GameManagerBase.Current.OnGameNetworkEnd();
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<TWNetworkComponent>());
		}

		// Token: 0x06002732 RID: 10034 RVA: 0x000971A8 File Offset: 0x000953A8
		void IGameNetworkHandler.OnStartReplay()
		{
			GameNetwork.AddNetworkComponent<TWNetworkComponent>();
		}

		void IGameNetworkHandler.OnEndReplay()
		{
			GameNetwork.DestroyComponent(GameNetwork.GetNetworkComponent<TWNetworkComponent>());
		}

		void IGameNetworkHandler.OnHandleConsoleCommand(string command)
		{
		
		}
	}
}
