using CrewNetwork;
using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PhotonPlayer = Photon.Realtime.Player;

namespace CrewNetwork.Transport {

    public class PhotonNetworkManager : NetworkManager, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback, IInRoomCallbacks, ILobbyCallbacks {

        private readonly LoadBalancingClient photonClient = new LoadBalancingClient();
        private static TypedLobby sqlMainLobby = new TypedLobby("sqlMain", LobbyType.SqlLobby);

        private PhotonClient client;
        private PhotonServer server;

        private Dictionary<string, RoomInfo> m_CachedRoomList = new Dictionary<string, RoomInfo>();
        public IReadOnlyDictionary<string, RoomInfo> cachedRoomsList;

        private List<PacketWriter> packetsToRecycle = new List<PacketWriter>();

        private Dictionary<int, CrewNetPeer_Ref> photonUserIdToCrewPeer = new Dictionary<int, CrewNetPeer_Ref>();

        public event Action<List<TypedLobbyInfo>> OnLobbiesStatisticsEvent;

        private List<TypedLobbyInfo> cachedLobbyStatistics;

        private bool m_Shutdown;

        public override void Init() {
            base.Init();

            ServerPeerRef = CrewNetPeer_Ref.FromID(1);

            cachedRoomsList = m_CachedRoomList;

            client = new PhotonClient(this);
            server = new PhotonServer(this);

            client.OnPacket += MessageListener.OnPacketFromServer;
            server.OnPacket += MessageListener.OnPacketFromClient;

            photonClient.AddCallbackTarget(this);
            photonClient.StateChanged += OnStateChange;
            photonClient.OpResponseReceived += OnOperationResponseReceived;

            photonClient.NickName = Guid.NewGuid().ToString();
            photonClient.LoadBalancingPeer.ReuseEventInstance = true;

            photonClient.ConnectUsingSettings(new AppSettings() {
                AppIdRealtime = "42fba0c7-38bc-4d7e-a9d0-9c3a305f74af",
                FixedRegion = "eu",
                UseNameServer = true,
                EnableLobbyStatistics = true,
            });
        }

        public void CreateGame() {

        }

        public void JoinGame() {
            
        }

        public void QuickMatch() {
            photonClient.OpJoinRandomOrCreateRoom(null, null);
        }

        public void QuickMatch1v1() {
            OpJoinRandomRoomParams joinRandomParams = new OpJoinRandomRoomParams();
            joinRandomParams.ExpectedMaxPlayers = 2;

            EnterRoomParams createRoomParams = new EnterRoomParams();
            createRoomParams.RoomOptions = new RoomOptions();
            createRoomParams.RoomOptions.MaxPlayers = 2;

            photonClient.OpJoinRandomOrCreateRoom(joinRandomParams, createRoomParams);
        }

        public void FindRooms(PhotonSqlFilter filter) {
            photonClient.OpGetGameList(sqlMainLobby, filter);
        }

        public override void Tick() {
            photonClient.Service();

            for (int i = packetsToRecycle.Count - 1; i >= 0; i--) {
                packetsToRecycle[i].Recycle();
                packetsToRecycle.RemoveAt(i);
            }
        }

        public int GetTotalPalyersOnlineCount() {
            if(cachedLobbyStatistics != null && cachedLobbyStatistics.Count > 0) {
                int sum = 0;
                foreach (var lobby in cachedLobbyStatistics) {
                    sum += lobby.PlayerCount;
                }

                return sum;
            } else {
                return 0;
            }
        }

        public override void Shutdown() {
            if(!m_Shutdown) {
                m_Shutdown = true;

                photonClient.Disconnect();
                photonClient.RemoveCallbackTarget(this);

                CrewNetDebug.Log("Deinitalize Photon Master Client");
            }
        }

        ~PhotonNetworkManager() {
            Shutdown();
        }

        public void SendAsServer(PacketWriter packet, CrewNetPeer_Ref peer) {
            CrewNetPeer_Ref localPeerRef = PhotonActorID_ToCrewNetPeer(photonClient.LocalPlayer.ActorNumber);

            if(peer == localPeerRef) {
                (IntPtr intPtr, int length) = packet.ToIntPtr();

                PacketReader packetReader = PacketReader.Get(intPtr, length);

                client.OnPacket?.Invoke(packetReader);

                packetReader.Recycle();
                packet.Recycle();

                return;
            }

            byte eventCode = 1;

            ArraySegment<byte> data = packet.ToArraySegment();
            RaiseEventOptions eventOptions = RaiseEventOptions.Default;

            int[] targetActors = new int[] {
                CrewNetPeer_ToPhotonActorID(peer)
            };

            eventOptions.TargetActors = targetActors;

            photonClient.OpRaiseEvent(eventCode, data, eventOptions, packet.sendOption.ToPhotonSendOption());

            packetsToRecycle.Add(packet);
            if(packet.sendOption.HasFlag(SendOption.Instant)) {
                Tick();
            }
        }

        public void SendAsClient(PacketWriter packet) {
            if(IsServer) {
                (IntPtr intPtr, int length) = packet.ToIntPtr();

                CrewNetPeer_Ref localPeerRef = PhotonActorID_ToCrewNetPeer(photonClient.LocalPlayer.ActorNumber);
                PacketReader packetReader = PacketReader.Get(intPtr, length);

                server.OnPacket?.Invoke(localPeerRef, packetReader);

                packetReader.Recycle();
                packet.Recycle();

                return;
            }

            byte eventCode = 1;

            ArraySegment<byte> data = packet.ToArraySegment();
            RaiseEventOptions eventOptions = RaiseEventOptions.Default;

            int[] targetActors = new int[] {
                photonClient.CurrentRoom.MasterClientId
            };

            eventOptions.TargetActors = targetActors;

            photonClient.OpRaiseEvent(eventCode, data, eventOptions, packet.sendOption.ToPhotonSendOption());

            packetsToRecycle.Add(packet);
            if (packet.sendOption.HasFlag(SendOption.Instant)) {
                Tick();
            }
        }

        private void Server_AddPlayerPeer(PhotonPlayer player) {
            CrewNetPeer_Ref crewNetPeer = CrewNetPeer_Ref.FromID((ushort)player.ActorNumber);
            photonUserIdToCrewPeer.Add(player.ActorNumber, crewNetPeer);
            server.OnPeerConnected?.Invoke(crewNetPeer);
        }

        private void Server_RemovePlayerPeer(PhotonPlayer player) {
            CrewNetPeer_Ref crewNetPeer = PhotonActorID_ToCrewNetPeer(player.ActorNumber);
            photonUserIdToCrewPeer.Remove(player.ActorNumber);
            server.OnPeerDisconnected?.Invoke(crewNetPeer);
        }

        private int CrewNetPeer_ToPhotonActorID(CrewNetPeer_Ref peer) {
            foreach (KeyValuePair<int,CrewNetPeer_Ref> pair in photonUserIdToCrewPeer) {
                if(pair.Value.UID == peer.UID) {
                    return pair.Key;
                }
            }

            return -1;
        }

        private CrewNetPeer_Ref PhotonActorID_ToCrewNetPeer(int actorID) {
            foreach (KeyValuePair<int, CrewNetPeer_Ref> pair in photonUserIdToCrewPeer) {
                if (pair.Key == actorID) {
                    return pair.Value;
                }
            }

            return default(CrewNetPeer_Ref);
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList) {
            for (int i = 0; i < roomList.Count; i++) {
                RoomInfo info = roomList[i];
                if (info.RemovedFromList) {
                    if (m_CachedRoomList.ContainsKey(info.Name)) {
                        m_CachedRoomList.Remove(info.Name);
                    }
                } else {
                    m_CachedRoomList[info.Name] = info;
                }
            }
        }

        #region Events

        private void OnStateChange(ClientState arg1, ClientState arg2) {
            CrewNetDebug.Log(arg1 + " -> " + arg2);
        }

        private void OnOperationResponseReceived(OperationResponse response) {
            CrewNetDebug.Log("OnOperationResponseReceived: " + response.ToStringFull());

            if (response.OperationCode == OperationCode.GetGameList) {
                CrewNetDebug.Log("GetGameList: " + response.ToStringFull());
            }
        }

        #endregion

        #region IConnectionCallbacks

        void IConnectionCallbacks.OnConnectedToMaster() {
            CrewNetDebug.Log($"OnConnectedToMaster Server: {this.photonClient.LoadBalancingPeer.ServerIpAddress} \n UserID:{photonClient.UserId} Nickname: {photonClient.NickName}");
        }

        void IConnectionCallbacks.OnConnected() {
            CrewNetDebug.Log("OnConnected");
        }

        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause) {
            CrewNetDebug.LogError($"OnDisconnected, cause: { cause }");
        }

        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler) {
            CrewNetDebug.Log("OnRegionListReceived");
        }

        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data) {
            CrewNetDebug.Log("OnCustomAuthenticationResponse");
        }

        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage) {
            CrewNetDebug.Log("OnCustomAuthenticationFailed");
        }

        #endregion

        #region IMatchmakingCallbacks

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList) {
            CrewNetDebug.Log(friendList.ToPrettyString("OnFriendListUpdate", true));
        }

        void IMatchmakingCallbacks.OnCreatedRoom() {
            if(role == NetworkRole.None) {
                role = NetworkRole.Host;
            }

            CrewNetDebug.Log($"Created room! ID:{photonClient.CurrentRoom.ToStringFull()}");
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message) {
            CrewNetDebug.LogError($"Create room failed");
        }

        void IMatchmakingCallbacks.OnJoinedRoom() {
            if(photonClient.CurrentRoom.MasterClientId == photonClient.LocalPlayer.ActorNumber) {
                role = NetworkRole.Host;
            }

            if(role == NetworkRole.None) {
                role = NetworkRole.Client;
            }

            if (IsServer) {
                Server_AddPlayerPeer(photonClient.LocalPlayer);
            }

            client.OnConnected?.Invoke();

            CrewNetDebug.Log($"Joined room! ID:{photonClient.CurrentRoom.ToStringFull()}");
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message) {
            CrewNetDebug.LogError($"Join room failed. ErrorCode: {returnCode} Message: {message}");
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message) {
            CrewNetDebug.LogError($"Join RANDOM failed. ErrorCode: {returnCode} Message: {message}");
        }

        void IMatchmakingCallbacks.OnLeftRoom() {
            client.OnDisconnected?.Invoke();

            CrewNetDebug.LogError($"OnLeftRoom");
        }

        #endregion

        #region IOnEventCallback

        void IOnEventCallback.OnEvent(EventData photonEvent) {
            CrewNetDebug.Log("OnEvent: " + photonEvent.ToStringFull());

            switch(photonEvent.Code) {
                case 1:
                    byte[] data = (byte[])photonEvent.CustomData;

                    CrewNetDebug.Log(data.ToPrettyString("bytes: ", false, false, x => x + "_"));

                    PacketReader packet = PacketReader.Get(data);

                    if(role == NetworkRole.Client) {
                        client.OnPacket?.Invoke(packet);
                    } else {
                        CrewNetPeer_Ref peer = PhotonActorID_ToCrewNetPeer(photonEvent.Sender);
                        server.OnPacket?.Invoke(peer, packet);
                    }

                    packet.Recycle();
                    break;
            }            
        }

        #endregion

        #region IInRoomCallbacks

        void IInRoomCallbacks.OnPlayerEnteredRoom(PhotonPlayer newPlayer) {                                  

            if (IsServer) {
                Server_AddPlayerPeer(newPlayer);
            }

            CrewNetDebug.Log("OnPlayerEnteredRoom: " + newPlayer.ToStringFull());
        }

        void IInRoomCallbacks.OnPlayerLeftRoom(PhotonPlayer otherPlayer) {           

            if(IsServer) {
                Server_RemovePlayerPeer(otherPlayer);
            }

            CrewNetDebug.Log("OnPlayerLeftRoom: " + otherPlayer.ToStringFull());
        }

        void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) {

        }

        void IInRoomCallbacks.OnPlayerPropertiesUpdate(PhotonPlayer targetPlayer, Hashtable changedProps) {

        }

        void IInRoomCallbacks.OnMasterClientSwitched(PhotonPlayer newMasterClient) {

        }

        #endregion

        #region ILobbyCallbacks

        void ILobbyCallbacks.OnJoinedLobby() {
            m_CachedRoomList.Clear();

            CrewNetDebug.Log("OnJoinedLobby " + photonClient.CurrentLobby.ToString());
        }

        void ILobbyCallbacks.OnLeftLobby() {
            m_CachedRoomList.Clear();

            CrewNetDebug.Log("OnLeftLobby");
        }

        void ILobbyCallbacks.OnRoomListUpdate(List<RoomInfo> roomList) {
            UpdateCachedRoomList(roomList);

            CrewNetDebug.Log(roomList.ToPrettyString("OnRoomListUpdate", true, true, x => x.ToStringFull()));
        }

        void ILobbyCallbacks.OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) {
            cachedLobbyStatistics = lobbyStatistics;

            OnLobbiesStatisticsEvent?.Invoke(lobbyStatistics);

            CrewNetDebug.Log(lobbyStatistics.ToPrettyString("OnLobbyStatisticsUpdate", true));
        }

        #endregion

    }

}

public interface ITransportServer : ITickable {
    event Action<CrewNetPeer_Ref> OnPeerConnected;
    event Action<CrewNetPeer_Ref> OnPeerDisconnected;
    event Action<CrewNetPeer_Ref, PacketReader> OnPacket;

    void Start();
    void Send(PacketWriter packet, CrewNetPeer_Ref peer);
    void SendInstant(PacketWriter packet, CrewNetPeer_Ref peer);
    void Disconnect(CrewNetPeer_Ref peer);
    void Shutdown();
}

public interface ITransportClient : ITickable {

    event Action OnConnected;
    event Action OnDisconnected;
    event Action<PacketReader> OnPacket;

    void Connect();
    void Send(PacketWriter packet);
    void Disconnect();
    void Shutdown();
}

public class PhotonSqlFilter {

    private string str;
    private int writtenFieldsCount;

    public PhotonSqlFilter() {
        str = "";
        writtenFieldsCount = 0;
    }

    public PhotonSqlFilter Between(string fieldInd, int min, int max) {
        if(writtenFieldsCount != 0) {
            str += " AND ";
        }

        str += $"{fieldInd} BETWEEN {min} AND {max}";
        writtenFieldsCount++;
        return this;
    }

    public PhotonSqlFilter Equal(string fieldInd, object value) {
        if (writtenFieldsCount != 0) {
            str += " AND ";
        }

        str += $"{fieldInd} = '{value}'";
        writtenFieldsCount++;
        return this;
    }

    public override string ToString() {
        return str;
    }

    public static implicit operator string(PhotonSqlFilter filter) => filter.str;

}

/*

public void CreateSqlRoom() {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new Hashtable { { PhotonGameParams.PropKey_GameMode, 50 } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { PhotonGameParams.PropKey_GameMode };

        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomOptions = roomOptions;
        enterRoomParams.Lobby = sqlMainLobby;

        client.OpCreateRoom(enterRoomParams);
    }

    public void FindRooms(PhotonSqlFilter filter) {
        client.OpGetGameList(sqlMainLobby, filter);        
    }

    public void JoinSimple() {

        client.OpJoinRandomOrCreateRoom(new OpJoinRandomRoomParams() {
            ExpectedMaxPlayers = CrewNetCommon.MAX_PLAYERS_COUNT
        }, 
        new EnterRoomParams() {
            RoomOptions = new RoomOptions() {
                MaxPlayers = CrewNetCommon.MAX_PLAYERS_COUNT,
            }
        });
    }

    public void CreateRoom(int map) {
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomName = null;

        RoomOptions roomOptions = enterRoomParams.RoomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = CrewNetCommon.MAX_PLAYERS_COUNT;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {
            CrewNetCommon.ROOM_KEY_MAP,
            CrewNetCommon.ROOM_KEY_GAME_UID
        };

        Hashtable roomProperties = new Hashtable();
        roomProperties[CrewNetCommon.ROOM_KEY_MAP] = map;
        roomProperties[CrewNetCommon.ROOM_KEY_GAME_UID] = CrewNetUtils.GeneratePrettyRoomUID(6);

        roomOptions.CustomRoomProperties = roomProperties;

        if(!client.OpJoinRandomOrCreateRoom(new OpJoinRandomRoomParams() {
            ExpectedCustomRoomProperties = roomProperties,
            ExpectedMaxPlayers = CrewNetCommon.MAX_PLAYERS_COUNT,
            TypedLobby = TypedLobby.Default
        }, enterRoomParams)) {
            OnPhotonError?.Invoke("[PhotonMasterClient] OpCreateRoom failed");
        }
    }

    public void CreateOrJoinRandomRoom() {

    }

    public void JoinRoom(string roomUID, int map) {
        EnterRoomParams enterRoomParams = new EnterRoomParams();
        enterRoomParams.RoomName = null;
        enterRoomParams.RoomOptions = new RoomOptions();        

        Hashtable roomProperties = new Hashtable();
        roomProperties["mymap"] = map;
        roomProperties["roomUID"] = roomUID;

        enterRoomParams.RoomOptions.CustomRoomPropertiesForLobby = new string[] {
            CrewNetCommon.ROOM_KEY_MAP,
            CrewNetCommon.ROOM_KEY_GAME_UID
        };
        enterRoomParams.RoomOptions.CustomRoomProperties = roomProperties;

        if(!client.OpJoinRandomRoom(new OpJoinRandomRoomParams() {
            ExpectedCustomRoomProperties = roomProperties,
            ExpectedMaxPlayers = CrewNetCommon.MAX_PLAYERS_COUNT,
            TypedLobby = TypedLobby.Default,
        })) {
            OnPhotonError?.Invoke("[PhotonMasterClient] OpJoinRoom failed");
        }
    }

*/

public class PhotonGameParams {
    public byte maxPlayers;
    public GameMode? gameMode = null;
    public GameMap? gameMap = null;

    public string[] GetCustomPropertiesForLobby() {
        List<string> props = new List<string>();
        if (gameMode != null) {
            props.Add(PropKey_GameMode);
        }
        if (gameMap != null) {
            props.Add(PropKey_GameMap);
        }
        return props.ToArray();
    }

    public Hashtable GetRoomProperties() {
        var table = new Hashtable();
        if (gameMode != null) {
            table.Add(PropKey_GameMode, (int)gameMode);
        }
        if (gameMap != null) {
            table.Add(PropKey_GameMap, (int)gameMap);
        }

        if (gameMode == null && gameMap == null) {
            return null;
        } else {
            return table;
        }
    }

    public const string PropKey_GameMode = "C0";
    public const string PropKey_GameMap = "C1";

    public enum GameMode {
        None,
        PVP_1v1,
        PVP_2v2,
        Deathmatch
    }

    public enum GameMap {
        None,
        Forest,
        Desert
    }

}