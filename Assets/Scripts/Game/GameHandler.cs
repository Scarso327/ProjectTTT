using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour {

    SpawnSpot[] spawnSpots;

    public GameObject StandByCam;

    public GameObject UI;
    public GameObject PlayerUI;

    bool spawned = false;
    bool uiSpawn = false;

    // Game Phases in seconds.
    int preparingTime = 30;
    int gamingTime = 600;
    int endingTime = 15;

    int minimumPlayers = 2; // The least amount of players before the game starts.

    // Timing vars.
    float timeLeft = 0;
    public Text Timing; // Temp will be moved to a UI handler.

    // Ratios.
    float traitorRatio = 0.25F;

    // Lists
    // Alive Users
    List<PhotonPlayer> aliveInnocents = new List<PhotonPlayer>();
    List<PhotonPlayer> aliveTraitors = new List<PhotonPlayer>();

    // Dead Users
    List<PhotonPlayer> deadInnocents = new List<PhotonPlayer>();
    List<PhotonPlayer> deadTraitors = new List<PhotonPlayer>();

    public GameObject spectator;
    public GameObject mySpectator;

    void Start () {
        StandByCam.SetActive(true);
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();

        if (!uiSpawn)
        {
            // Spawn UI (Client Side)
            PlayerUI = Instantiate(UI);
        }

        uiSpawn = true;

        UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
        uiHandler.toggleWeapons(false);

        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount >= minimumPlayers)
        {
            nextPhase(true);
        }

        if((int)PhotonNetwork.room.CustomProperties["S"] != 2 && PhotonNetwork.room.PlayerCount >= minimumPlayers)
        {
            SpawnUser();
        }
    }

    // Allows for a mimium before the game actually starts
    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount >= minimumPlayers && timeLeft <= 0)
        {
            nextPhase(true);
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        // Setup as we don't have enough players
        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount < minimumPlayers)
        {
            if (!((int)PhotonNetwork.room.CustomProperties["S"] > 1))
            {
                // End round/Game and move on.
                StandByCam.SetActive(true);

                currentPhase = 0; // Set backup for next round.

                // Delete players
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("PlayerController");

                foreach (GameObject chara in allPlayers)
                {
                    chara.GetComponent<PhotonView>().RPC("destroyThis", PhotonTargets.All, "RoundEnd");
                }

                spawned = false;

                UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));

                // Update UI
                uiHandler.updateStatus("PREPEARING");
                uiHandler.updateSideUI("Undecided");
                uiHandler.closeWin();
                uiHandler.toggleWeapons(false);

                uiHandler.toggleTimer(false);

                // Wipe lists
                aliveInnocents = new List<PhotonPlayer>();
                aliveTraitors = new List<PhotonPlayer>();
                deadInnocents = new List<PhotonPlayer>();
                deadTraitors = new List<PhotonPlayer>();

                // Remove time
                timeLeft = 0;
            } else
            {
                int side = (int)player.CustomProperties["Team"];

                if (side == 1)
                {
                    aliveTraitors.Remove(player);
                    deadTraitors.Add(player);
                }
                else
                {
                    aliveInnocents.Remove(player);
                    deadInnocents.Add(player);
                }

                if (aliveInnocents.Count <= 0)
                {
                    PhotonView.Get(this).RPC("roundWin", PhotonTargets.All, "traitor", "Innocents Eliminated");
                }
                else if (aliveTraitors.Count <= 0)
                {
                    PhotonView.Get(this).RPC("roundWin", PhotonTargets.All, "innocent", "Traitors Eliminated");
                }
            }
        }
    }

    [PunRPC]
    public void SpawnUser()
    {
        if (!spawned)
        {
            StandByCam.SetActive(false);

            if (mySpectator != null)
            {
                mySpectator.SetActive(false);
            }

            if (spawnSpots == null)
            {
                Debug.LogError("No spawn locations found.");
                return;
            }

            SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
            GameObject myPlayerGO = (GameObject)PhotonNetwork.Instantiate("PlayerCharacter", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

            (myPlayerGO.GetComponent("FirstPersonController") as MonoBehaviour).enabled = true;

            Transform PlayerCamera = myPlayerGO.transform.Find("FirstPersonCharacter");

            ((Camera)PlayerCamera.GetComponent("Camera")).enabled = true;
            ((AudioListener)PlayerCamera.GetComponent("AudioListener")).enabled = true;

            myPlayerGO.transform.Find("FirstPersonCharacter").gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            spawned = true;

            // Set our timer var (Would move the script to the UIHandler script but I already made it here so this is quicker and easier for me)
            UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
            Timing = uiHandler.TimerText;

            uiHandler.player = myPlayerGO;

            uiHandler.toggleTimer(true);

            uiHandler.UpdateHealth();

            uiHandler.toggleWeapons(true);
        }
    }

    bool viewPlayers = false;

    private void OnGUI()
    {
        if (viewPlayers)
        {
            GUILayout.Label("Alive Innocents");
            foreach(PhotonPlayer player in aliveInnocents)
            {
                if (GUILayout.Button(player.NickName))
                {
                    GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("PlayerController");

                    foreach (GameObject chara in allPlayers)
                    {
                        if (player == chara.GetComponent<PhotonView>().owner)
                        {
                            chara.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 100);
                        }
                    }
                }
            }
            GUILayout.Label("Alive Traitors");
            foreach (PhotonPlayer player in aliveTraitors)
            {
                if (GUILayout.Button(player.NickName))
                {
                    GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("PlayerController");

                    foreach (GameObject chara in allPlayers)
                    {
                        if (player == chara.GetComponent<PhotonView>().owner)
                        {
                            chara.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 100);
                        }
                    }
                }
            }
            GUILayout.Label("Dead Innocents");
            foreach (PhotonPlayer player in deadInnocents)
            {
                if (GUILayout.Button(player.NickName))
                {

                }
            }
            GUILayout.Label("Dead Traitors");
            foreach (PhotonPlayer player in deadTraitors)
            {
                if (GUILayout.Button(player.NickName))
                {

                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            viewPlayers = true;
        if (Input.GetKeyUp(KeyCode.Tab))
            viewPlayers = false;

        if (timeLeft > 0 && spawned)
        {
            if (PhotonNetwork.isMasterClient)
            {
                timeLeft -= Time.deltaTime;
            }

            string minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            string seconds = (timeLeft % 60).ToString("00");
            
            Timing.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("updateTime", PhotonTargets.All, timeLeft);
        }
    }

    // Timing functions (Only used by the host of the game as then everyone is 100% synced)

    int currentPhase = 0; // 0 - Not Start, 1 - Prepearing, 2 - Playing, 3 - Ending.

    // This function is used to call for the phase timing by the host.
    public void nextPhase (bool timeWin)
    {
        if (PhotonNetwork.isMasterClient && timeLeft <= 0)
        {
            if (currentPhase == 0 && PhotonNetwork.room.PlayerCount >= minimumPlayers)
            {
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("SpawnUser", PhotonTargets.All);

                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "S", 1 } };
                PhotonNetwork.room.SetCustomProperties(hash);

                currentPhase += 1;

                PhotonView.Get(this).RPC("updatePhase", PhotonTargets.All, "PREPEARING"); // Setup UI

                timeLeft = preparingTime;
                StartCoroutine("timePhase");
            } else if (currentPhase == 1 && PhotonNetwork.room.PlayerCount >= minimumPlayers)
            {
                currentPhase += 1;

                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "S", 2 } };
                PhotonNetwork.room.SetCustomProperties(hash);

                // Code for selecting traitors etc.
                selectTraitors();

                PhotonView.Get(this).RPC("updatePhase", PhotonTargets.All, "PLAYING"); // Setup UI

                timeLeft = gamingTime;
                StartCoroutine("timePhase");
            } else if (currentPhase == 2)
            {
                currentPhase += 1;

                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "S", 3 } };
                PhotonNetwork.room.SetCustomProperties(hash);

                if (timeWin)
                {
                    // If it makes it to this point, they innocents win by time!
                    PhotonView.Get(this).RPC("roundWin", PhotonTargets.All, "innocent", "Time Up");
                }

                PhotonView.Get(this).RPC("updatePhase", PhotonTargets.All, "ROUND END"); // Setup UI

                Debug.Log("Yay");

                timeLeft = endingTime;
                Debug.Log(timeLeft);
                StartCoroutine("timePhase");
            } else
            {
                // Next round?
                PhotonView.Get(this).RPC("nextRound", PhotonTargets.All);
            }
        }
    }

    [PunRPC]
    public void nextRound ()
    {
        StandByCam.SetActive(true);

        currentPhase = 0; // Set backup for next round.

        // Delete players
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("PlayerController");

        foreach (GameObject player in allPlayers)
        {
            player.GetComponent<PhotonView>().RPC("destroyThis", PhotonTargets.All, "RoundEnd");
        }

        spawned = false;

        UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));

        // Update UI
        uiHandler.updateStatus("PREPEARING");
        uiHandler.updateSideUI("Undecided");
        uiHandler.closeWin();
        uiHandler.toggleWeapons(false);

        // Wipe lists
        aliveInnocents = new List<PhotonPlayer>();
        aliveTraitors = new List<PhotonPlayer>();
        deadInnocents = new List<PhotonPlayer>();
        deadTraitors = new List<PhotonPlayer>();

        // Remove time
        timeLeft = 0;

        if (PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount >= minimumPlayers && timeLeft <= 0)
        {
            nextPhase(true);
        }
    }

    [PunRPC]
    public void updatePhase(string phase)
    {
        UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
        uiHandler.updateStatus(phase);
    }
    
    // Sync up everyone to the master client
    [PunRPC]
    public void updateTime (float time)
    {
        if (!(PhotonNetwork.isMasterClient))
        {
            timeLeft = time;
        }
    }

    // Times till next call
    IEnumerator timePhase ()
    {
        if (PhotonNetwork.isMasterClient)
        {
            int phaseTime = 0;
            // We have to get phase time using a switch since I can't pass this parameters.
            switch (currentPhase)
            {
                case 1:
                    phaseTime = preparingTime;
                    break;
                case 2:
                    phaseTime = gamingTime;
                    break;
                case 3:
                    phaseTime = endingTime;
                    break;
            }

            yield return new WaitForSeconds(phaseTime);

            if (PhotonNetwork.room.PlayerCount >= minimumPlayers)
            {
                nextPhase(true);
            }
        }
    }

    public void selectTraitors()
    {
        float totalTraitors = Mathf.Floor(traitorRatio * PhotonNetwork.room.PlayerCount);

        if (totalTraitors < 1)
        {
            totalTraitors = 1;
        }

        Debug.Log(totalTraitors);

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (totalTraitors > 0) {
                if (Random.value > .25)
                {
                    // TRAITOR
                    totalTraitors -= 1;

                    PhotonView photonView = PhotonView.Get(this);
                    photonView.RPC("setupTraitor", PhotonTargets.All, player);
                } else
                {
                    // INNOCENT
                    // In the future this will select if they are a detective or not.
                    PhotonView photonView = PhotonView.Get(this);
                    photonView.RPC("setupInnocent", PhotonTargets.All, player);
                }
            } else
            {
                // INNOCENT
                // In the future this will select if they are a detective or not.
                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("setupInnocent", PhotonTargets.All, player);
            }
        }
    }

    [PunRPC]
    public void setupTraitor (PhotonPlayer player)
    {
        // Just to be ran on this users game.
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "Team", 1 } };
        player.SetCustomProperties(hash);

        aliveTraitors.Add(player);

        if (player == PhotonNetwork.player)
        {
            UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
            uiHandler.updateSideUI("Traitor");
        }
    }

    [PunRPC]
    public void setupInnocent (PhotonPlayer player)
    {
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "Team", 2 } };
        player.SetCustomProperties(hash);

        aliveInnocents.Add(player);

        // Just to be ran on this users game.
        if (player == PhotonNetwork.player)
        {
            UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
            uiHandler.updateSideUI("Innocent");
        }
    }

    [PunRPC]
    public void playerDied(PhotonPlayer Player)
    {
        int side = (int)Player.CustomProperties["Team"];

        if(side == 1)
        {
            aliveTraitors.Remove(Player);
            deadTraitors.Add(Player);
        } else
        {
            aliveInnocents.Remove(Player);
            deadInnocents.Add(Player);
        }

        // Setup spectator for dead player.
        if (Player.IsLocal)
        {
            if (mySpectator != null)
            {
                mySpectator.SetActive(true);
            }
            else
            {
                mySpectator = Instantiate(spectator, transform.position, transform.rotation);
            }
        }

        if (aliveInnocents.Count <= 0)
        {
            PhotonView.Get(this).RPC("roundWin", PhotonTargets.All, "traitor", "Innocents Eliminated");
        } else if (aliveTraitors.Count <= 0)
        {
            PhotonView.Get(this).RPC("roundWin", PhotonTargets.All, "innocent", "Traitors Eliminated");
        }
    }

    [PunRPC]
    public void roundWin (string sideWon, string note)
    {
        switch(sideWon)
        {
            case "innocent":
                Debug.Log("Innocent Win!");

                UIHandler uiHandler = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
                uiHandler.winDialog("Innocent");
                break;
            case "traitor":
                Debug.Log("Traitor Win!");

                UIHandler uiHandler2 = (UIHandler)PlayerUI.GetComponent(typeof(UIHandler));
                uiHandler2.winDialog("Traitor");
                break;
        }

        if (PhotonNetwork.isMasterClient)
        {
            StopCoroutine("timePhase");
        }

        currentPhase = 2;

        timeLeft = 0;

        nextPhase(false);
    }
}