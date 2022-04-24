using CrewNetwork;
using CrewNetwork.Transport.ENetTransport;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CrewNetwork.Transport {

    public class ENetNetworkManager : NetworkManager {        

        private ENetClient enetClient;
        private ENetServer enetServer;

        public override void Init() {
            base.Init();

            enetClient = new ENetClient();
            enetServer = new ENetServer();

            ServerPeerRef = CrewNetPeer_Ref.FromID(0);
        }

        public override void Tick() {
            if (role.HasFlag(NetworkRole.Client)) {
                if (enetClient != null) {
                    enetClient.Tick();
                }
            }

            if (role.HasFlag(NetworkRole.Server)) {
                if (enetServer != null) {
                    enetServer.Tick();
                }
            }
        }

        public void StartServer() {
            if (role == NetworkRole.None) {
                role = NetworkRole.Server;
            }

            enetServer.Init(new ServerStartConfig() {
                port = 7777,
                maxClients = 8
            });

            enetServer.Start();
            enetServer.OnPacket += MessageListener.OnPacketFromClient;
        }

        public void StartHost() {
            role = NetworkRole.Host;

            StartServer();
            StartClient();
        }

        public void StartClient() {
            if (role == NetworkRole.None) {
                role = NetworkRole.Client;
            }

            enetClient.Init(new ClientConnectionRequest() {
                ip = "127.0.0.1",
                port = 7777
            });

            enetClient.Connect();
            enetClient.OnPacket += MessageListener.OnPacketFromServer;
        }

        public override void Shutdown() {
            if (enetServer != null) {
                enetServer.Shutdown();
            }

            if (enetClient != null) {
                enetClient.Shutdown();
            }
        }

    }

}