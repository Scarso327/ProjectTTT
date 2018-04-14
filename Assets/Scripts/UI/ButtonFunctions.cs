using ExitGames.Demos.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour {

    bool registerwarning;

    public void openRegister()
    {
        registerwarning = true;
    }

    public void openCreateGame()
    {
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup createUI = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));
        if(createUI.CreateGameMaster.activeInHierarchy)
        {
            createUI.CreateGameMaster.SetActive(false);
        } else
        {
            createUI.CreateGameMaster.SetActive(true);

            // Set everything else false.
            createUI.FindGameMaster.SetActive(false);
        }
    }
    
    public void openFindGame()
    {
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup createUI = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));
        if (createUI.FindGameMaster.activeInHierarchy)
        {
            createUI.FindGameMaster.SetActive(false);
        }
        else
        {
            // Setup list?
            ServerListUIController control = createUI.FindGameMaster.GetComponent<ServerListUIController>();
            control.RefreshList();

            createUI.FindGameMaster.SetActive(true);

            // Set everything else false.
            createUI.CreateGameMaster.SetActive(false);
        }
    }

    public void hostGame()
    {
        GameObject menuSetup = GameObject.FindGameObjectWithTag("ScriptObject");


        MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
        GameObject network = menuScript.networkObject;

        Networking networkScript = (Networking)network.GetComponent(typeof(Networking));

        CreateItems items = (CreateItems)menuSetup.GetComponent(typeof(CreateItems));

        string name = items.nameField.text;
        byte players = (byte)(8);
        int passworded = items.Passworded.value;
        string password = items.Password.text;

        // Check values
        if (name == "" || name.Length > 32)
        {
            // No!
            Debug.Log("Name");
        }
        else if (players < 2 || players > 16)
        {
            // No!
            Debug.Log("Players");
        }
        else if (passworded < 0 || passworded > 1)
        {
            // No!
            Debug.Log("Passworded");
        }
        else if (password == "" && passworded == 1)
        {
            // No!
            Debug.Log("Password");
        }
        else
        {
            // Name, Players, Server List, Passworded, Password
            networkScript.HostGame(name, players, true, passworded, password);
        }
    }

    public void joinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    #region QuitGameFunctions
    public void logoutFunction()
    {
        GameObject loginHandler = GameObject.FindGameObjectWithTag("LoginHandler");
        LoginHandler handlerScript = (LoginHandler)loginHandler.GetComponent(typeof(LoginHandler));
        handlerScript.logout();
    }

    public void startQuit()
    {
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup script = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));
        script.quitPanel.SetActive(true);
    }

    public void cancelQuit()
    {
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup script = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));
        script.quitPanel.SetActive(false);
    }

    public void quitGame()
    {
        Application.Quit();
    }
    #endregion

    #region PasswordFunctions

    public void joinGame ()
    {
        GameObject networkObject = GameObject.FindGameObjectWithTag("NetworkObject");
        Networking networkScript = networkObject.GetComponent<Networking>();

        // Get room info that we stored earlier.
        string pw = networkScript.password;
        string room = networkScript.roomName;

        // Get ui elements
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup createUI = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));

        Text status = createUI.PWStatus;
        InputField PWInput = createUI.PWInput;

        // Make sure they entered the correct password.
        if (PWInput.text == pw)
        {
            PhotonNetwork.JoinRoom(room);
        } else
        {
            status.text = "Wrong Password!";
        }
    }

    public void closePassword()
    {
        GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup createUI = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));

        createUI.PasswordMaster.SetActive(false);
    }

    #endregion

    void OnGUI()
    {
        if (registerwarning)
        {
            GUI.contentColor = Color.black;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("To Register you are required to go to our website. Would you like to carry on?");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Yes"))
            {
                Application.OpenURL("http://freeflightinteractive.co.uk/ProjectTTT/");
            }

            if (GUILayout.Button("No"))
            {
                registerwarning = false;
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
