using CrewNetwork;
using CrewNetwork.Transport;
using MicroCrew.Utils;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BeanNetwork : Singleton<BeanNetwork> {

    public event Action<bool> OnInternetConnectionChanged;

    public GameServer gameServer;
    public GameClient gameClient;

    private Timer checkInternetConnectionTimer;
    private bool isConnectedToInternet = true;

    public PhotonNetworkManager photonNetworkManager { get; private set; }

    private UIW_SearchingOnlineGame searchGameWindow;

    public override void Init() {
        checkInternetConnectionTimer = new Timer(1f, true, true);

        MessageListener.InitPacketListeners(gameClient, gameServer);

        Debug.Log("[BeanNetwork] Init photon manager");

        photonNetworkManager = new PhotonNetworkManager();
        photonNetworkManager.Init();

        gameServer.Init();
        gameClient.Init();        
    }

    protected override void Shutdown() {
        photonNetworkManager.Shutdown();
    }

    public void InternalUpdate() {
        if(checkInternetConnectionTimer) {
            checkInternetConnectionTimer.AddFromNow();
            CheckInternetConnection();
        }

        photonNetworkManager.Tick();
    }

    public void InternalFixedUpdate() {

    }

    public void FindMatch() {
        searchGameWindow = (UIW_SearchingOnlineGame)UIWindowManager.CreateWindow(new UIWData_SearchOnlineGame());

        photonNetworkManager.QuickMatch();
    }

    private void CheckInternetConnection() {
        StartCoroutine(CheckInternet_Coroutine("https://google.com"));
    }
    
    private IEnumerator CheckInternet_Coroutine(string url) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url)) {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    if(isConnectedToInternet) {
                        OnInternetConnectionChanged?.Invoke(false);
                        isConnectedToInternet = false;
                    }
                    break;
                case UnityWebRequest.Result.Success:
                    if (!isConnectedToInternet) {
                        OnInternetConnectionChanged?.Invoke(true);
                        isConnectedToInternet = true;
                    }
                    break;
            }
        }
    }

}
