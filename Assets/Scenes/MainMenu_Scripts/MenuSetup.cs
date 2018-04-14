using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  Setups all information and buttons depending on information recieved.
///  We will also get it to tell us if Photon networking is working etc.
/// </summary>
public class MenuSetup : MonoBehaviour {

    public GameObject networkObject;
    public GameObject loinObject;

    public GameObject LoadingPanel;
    public Text LoadingTitle;
    public Text LoadingDes;

    public GameObject MainMenuMaster;
    public Text VersionText;
    public Text PlayerText;

    public GameObject LoginMaster;
    public InputField username;
    public InputField pwd;
    public Toggle saveUsername;
    public Text infoField;

    public GameObject quitPanel;

    public GameObject CreateGameMaster;
    public GameObject FindGameMaster;

    public GameObject PasswordMaster;

    public Text PWStatus;
    public InputField PWInput;

    bool loginPhase; // Used to control the action of the enter key!

    // Use this for initialization
    public void Start () {
        loginPhase = false;

        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoJoinLobby = true;

        // If they saved their username, set the input to reflect that.
        if(PlayerPrefs.GetInt("NameSaved") == 1)
        {
            username.text = PlayerPrefs.GetString("Username");
            saveUsername.isOn = true;
        }

        if (MainMenuMaster == null)
        {
            Debug.LogError("MainMenuMaster is null...");
            LoadingTitle.text = "Fatal Error...";
            LoadingDes.text = "An error has occured...";
        }

        if (LoginMaster == null)
        {
            Debug.LogError("LoginMaster is null...");
            LoadingTitle.text = "Fatal Error...";
            LoadingDes.text = "An error has occured...";
        }

        if (quitPanel == null)
        {
            Debug.LogError("quitPanel is null...");
            LoadingTitle.text = "Fatal Error...";
            LoadingDes.text = "An error has occured...";
        }

        MainMenuMaster.SetActive(false);
        LoginMaster.SetActive(false);
        LoadingPanel.SetActive(true);
        quitPanel.SetActive(false);
        CreateGameMaster.SetActive(false);
        FindGameMaster.SetActive(false);
        PasswordMaster.SetActive(false);

        LoadingTitle.text = "Connecting...";

        // Have we found our networking object?
        if (networkObject == null)
        {
            // No? - Find that guy!
            networkObject = GameObject.FindGameObjectWithTag("NetworkObject");
        }

        // Yes? - Call for network check.
        Networking tmp = (Networking)networkObject.GetComponent(typeof(Networking));
        bool connection = tmp.checkConnection();
        string version = tmp.version();

        // Temp Fix
        if (loinObject == null)
        {
            loinObject = GameObject.FindGameObjectWithTag("LoginHandler");
        }

        if (connection)
        {
            // Already connected.
            afterConnection();
        } else
        {
            PhotonNetwork.ConnectUsingSettings(version);
        }
	}

    private void Update()
    {
        // Nit picky thing I have about not being able to press enter to login.
        if(loginPhase && Input.GetKey(KeyCode.Return))
        {
            checkLogin();
        }    
    }

    // Once we have connected, do this.
    public void afterConnection ()
    {
        LoadingTitle.text = "Loading UI...";

        // Time to make sure we actually have all the UI components we require.
        if(MainMenuMaster == null)
        {
            Debug.LogError("MainMenuMaster is null...");
            LoadingTitle.text = "Fatal Error...";
            LoadingDes.text = "An error has occured...";
        } else {
            if (LoginMaster == null)
            {
                Debug.LogError("LoginMaster is null...");
                LoadingTitle.text = "Fatal Error...";
                LoadingDes.text = "An error has occured...";
            }
            else
            {
                LoadingTitle.text = "Loading Login...";
                if (username == null || pwd == null)
                {
                    Debug.LogError("Login Inputs are null...");
                    LoadingTitle.text = "Fatal Error...";
                    LoadingDes.text = "An error has occured...";
                } else
                {
                    if (loinObject != null)
                    {
                        // Check if we need to login or if we are already logged in.
                        if (loinObject.GetComponent<LoginHandler>().loggedIn)
                        {
                            Debug.Log("Already Logged");
                            afterLogin();
                        }
                        else
                        {
                            Debug.Log("Not Logged");
                            loginPhase = true;
                            LoadingPanel.SetActive(false);
                            LoginMaster.SetActive(true);
                        }
                    } else
                    {
                        Debug.LogError("Login Handler can't be found...");
                        LoadingTitle.text = "Fatal Error...";
                        LoadingDes.text = "An error has occured...";
                    }
                }
            }
        }
    }

    public void checkLogin ()
    {
        infoField.text = "";
        LoadingTitle.text = "Signing In...";
        LoadingPanel.SetActive(true);
        LoginMaster.SetActive(false);

        LoginHandler loginH = (LoginHandler)loinObject.GetComponent(typeof(LoginHandler));
        loginH.startLoginRoutine();
    }

    public void failedLogin (string reason)
    {
        LoadingPanel.SetActive(false);
        LoginMaster.SetActive(true);

        switch (reason)
        {
            case "usernameBlank":
                infoField.text = "Username Blank...";
                break;

            case "pwdBlank":
                infoField.text = "Password Blank...";
                break;

            case "wrongInfo":
                infoField.text = "Wrong Username/Password...";
                break;

            default:
                LoadingPanel.SetActive(true);
                LoadingTitle.text = "Fatal Error...";
                LoadingDes.text = "An error has occured... (ID#Login)...";
                LoginMaster.SetActive(false);
                break;
        }
    }

    public void afterLogin ()
    {
        loginPhase = false;
        LoadingTitle.text = "Loading Extras...";

        if (PhotonNetwork.gameVersion != "" || PhotonNetwork.playerName != "")
        {
            VersionText.text = PhotonNetwork.gameVersion;
            PlayerText.text = PhotonNetwork.playerName;

            LoadingPanel.SetActive(false);
            MainMenuMaster.SetActive(true);
        } else
        {
            LoadingTitle.text = "Fatal Error...";
            LoadingDes.text = "An error has occured...";
        }
    }
}