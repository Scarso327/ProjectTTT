using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginHandler : MonoBehaviour
{

    public InputField username;
    public InputField pwd;
    public Toggle saveUsername;

    public bool loggedIn;

    string login_url = "https://example.co.uk/checkLogin.php"; // Use HTTPS for crying out loud.
    string hash = "aweqwrey"; // I removed the hash I was using so this will need to be replaced with the matching one in the checkLogin.php

    GameObject menuSetup;

    // Use this for initialization
    void Start()
    {
        loggedIn = false;

        GameObject script = GameObject.FindGameObjectWithTag("ScriptObject");
        MenuSetup scriptC = (MenuSetup)script.GetComponent(typeof(MenuSetup));
        username = scriptC.username;
        pwd = scriptC.pwd;
        saveUsername = scriptC.saveUsername;

        menuSetup = GameObject.FindGameObjectWithTag("ScriptObject");

        hash = hash + menuSetup.GetComponent<MenuSetup>().networkObject.GetComponent<Networking>().version().ToString();

        DontDestroyOnLoad(this.gameObject);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
    }

    public void startLoginRoutine()
    {
        // This allows us to call for the start login from other files
        StartCoroutine(startLogin());
    }

    IEnumerator startLogin()
    {
        if (username.text == "")
        {
            MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
            menuScript.failedLogin("usernameBlank");
        }
        else if (pwd.text == "")
        {
            MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
            menuScript.failedLogin("pwdBlank");
        }
        else
        {
            WWWForm form = new WWWForm();
            form.AddField("uname", username.text);
            form.AddField("pwd", pwd.text);
            form.AddField("hash", hash);

            WWW w = new WWW(login_url, form);

            yield return w;

            if (!string.IsNullOrEmpty(w.error))
            {
                print(w.error);
                MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
                menuScript.failedLogin("fatalError");
            }
            else
            {
                if (w.text == "LoginCorrect")
                {
                    loggedIn = true;

                    PhotonNetwork.playerName = username.text; // Set their multiplayer name.

                    // Check our current scene and execute certain code depending on our current scene.
                    Scene scene = SceneManager.GetActiveScene();
                    if (scene.name == "MainMenu")
                    {
                        MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
                        menuScript.afterLogin();
                    }

                    if (saveUsername.isOn)
                    {
                        PlayerPrefs.SetInt("NameSaved", 1);
                        PlayerPrefs.SetString("Username", username.text);
                    } else
                    {
                        PlayerPrefs.SetInt("NameSaved", 0);
                        PlayerPrefs.SetString("Username", "");
                    }
                }
                else if (w.text == "LoginFailed")
                {
                    MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
                    menuScript.failedLogin("wrongInfo");
                } else {
                    print(w.text);
                    MenuSetup menuScript = (MenuSetup)menuSetup.GetComponent(typeof(MenuSetup));
                    menuScript.failedLogin("fatalError");
                }
                w.Dispose();
            }
        }
    }

    // Checks if we are logged in and on the main menu.
    // Useful for when players return to it from games so they don't have to re login.
    // Also used to get back fields etc.
    public void OnLevelWasLoaded(int level)
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name == "MainMenu")
        {
            // Get back our fields!
            GameObject script = GameObject.FindGameObjectWithTag("ScriptObject");
            MenuSetup scriptC = (MenuSetup)script.GetComponent(typeof(MenuSetup));
            username = scriptC.username;
            pwd = scriptC.pwd;
            saveUsername = scriptC.saveUsername;

            menuSetup = GameObject.FindGameObjectWithTag("ScriptObject");
        }
    }


    // Signs the user out.
    // Clears any data + loads main menu.
    public void logout ()
    {
        PhotonNetwork.playerName = "";

        loggedIn = false;
        SceneManager.LoadScene("MainMenu");
    }
}