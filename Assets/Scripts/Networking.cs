using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Networking : MonoBehaviour {

    private string serverPW;
    private string room;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    // Returns our connection state.
    public bool checkConnection ()
    {
        // Are we connected to photon?
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public string version()
    {
        return "Pre-Alpha V0.0.4";
    }

    public string roomName
    {
        get
        {
            return room;
        }
        set
        {
            room = value;
        }
    }

    public string password
    {
        get
        {
            return serverPW;
        }
        set
        {
            serverPW = value;
        }
    }

    public void OnConnectedToPhoton()
    {
        // After connected to server.
        GameObject tmp = GameObject.FindGameObjectWithTag("ScriptObject");

        if (tmp != null)
        {
            MenuSetup tmp2 = (MenuSetup)tmp.GetComponent(typeof(MenuSetup));
            tmp2.afterConnection();
        }
    }

    public void OnDisconnectedFromPhoton()
    {
        // After disconnecting from server.
        SceneManager.LoadScene("MainMenu");

        GameObject tmp = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup tmp2 = (MenuSetup)tmp.GetComponent(typeof(MenuSetup));
        tmp2.Start();
    }

    public void HostGame(string name, byte players, bool isPublic, int passworded, string password)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = isPublic;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = players;

        // M = Map, 
        // S = Status: 0 - Creating, 1 - Preparing, 2 - Playing, 3 - Loading next round, 
        // P = Passworded, PW = Password.
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "M", "S", "P", "PW" };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "M", "Placeholder" }, { "S", 0 }, { "P", passworded }, { "PW", password } };

        PhotonNetwork.CreateRoom(name, roomOptions, null);
    }

    public void OnCreatedRoom()
    {
        Debug.Log("Created");

        SceneManager.LoadScene("PlaceHolder");
    }

    public void OnJoinedRoom()
    {
       

        /*
         * NEEDS TO BE REVISED AS IT DOES NOT FUCKING WORK
        // Make sure everyone has a unique name!
        int totalMatchs = 0;

        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.NickName == PhotonNetwork.playerName)
            {
                totalMatchs += 1;
            }
        }

        if (totalMatchs >= 1)
        {
            string currentName = PhotonNetwork.playerName;
            PhotonNetwork.playerName = currentName + " " + "(" + (totalMatchs) + ")";
        }
        */
    }

    public void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }
}