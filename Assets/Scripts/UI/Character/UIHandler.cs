using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public Image sidePanel;
    public Text sideText;

    public Text StatusText;
    public Text TimerText;
    GameObject TimerControl;

    public GameObject WinningPanel;
    public Image WinnerPanel;
    public Text WinnerText;

    public GameObject EscapeMenu;

    public GameObject player;

    public GameObject WeaponsPanel;
    public Text Health;
    public Text Weapon;
    public Text AmmoText;
    public Image healthBar;

    private void Start()
    {
        sidePanel.color = Color.grey;
        sideText.text = "UNDECIDED";

        WinningPanel.SetActive(false);
        EscapeMenu.SetActive(false);

        TimerControl = TimerText.gameObject;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Esacpe Menu key
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            // If the win panel is closed then we do pause menu stuff else close the win menu.
            if (!(WinnerPanel.gameObject.activeInHierarchy))
            {
                if (EscapeMenu.activeInHierarchy)
                {
                    EscapeMenu.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    
                    if (player != null)
                    {
                        player.GetComponent<PlayerHandler>().toggleControl(true);
                    }
                }
                else
                {
                    EscapeMenu.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    if (player != null)
                    {
                        player.GetComponent<PlayerHandler>().toggleControl(false);
                    }
                }
            } else
            {
                WinningPanel.SetActive(false);
                if (player != null)
                {
                    player.GetComponent<PlayerHandler>().toggleControl(true);
                }
            }
        }
    }

    public void closeWin()
    {
        WinningPanel.SetActive(false);
    }

    public void winDialog(string side)
    {
        switch (side)
        {
            case "Innocent":
                WinningPanel.SetActive(true);

                WinnerPanel.color = Color.green;
                WinnerText.text = "Innocents Wins";
                break;
            case "Traitor":
                WinningPanel.SetActive(true);

                WinnerPanel.color = Color.red;
                WinnerText.text = "Traitors Wins";
                break;
        }

        if (player != null)
        {
            player.GetComponent<PlayerHandler>().toggleControl(false);
        }
    }

    public void updateSideUI (string side)
    {
        switch(side)
        {
            case "Innocent":
                sidePanel.color = Color.green;
                sideText.text = "Innocent";
                break;

            case "Traitor":
                sidePanel.color = Color.red;
                sideText.text = "Traitor";
                break;

            case "Undecided":
                sidePanel.color = Color.grey;
                sideText.text = "UNDECIDED";
                break;

            default:
                Debug.LogError("Side Update Error (UI)");
                break;
        }
    }

    public void updateStatus (string status)
    {
        StatusText.text = status;
    }

    public void toggleTimer(bool toggle)
    {
        if (TimerControl != null)
        {
            TimerControl.SetActive(toggle);
        }
    }

    public void UpdateHealth()
    {
        int health = player.GetComponent<PlayerHandler>().Health;

        Health.text = health + "%";
        healthBar.fillAmount = (health / 100);
    }

    public void toggleWeapons(bool toggle)
    {
        if (WeaponsPanel != null)
        {
            WeaponsPanel.SetActive(toggle);
        }
    }

    #region ButtonFuctions

    public void closeEsacpeMenu()
    {
        if (player != null)
        {
            player.GetComponent<PlayerHandler>().toggleControl(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        EscapeMenu.SetActive(false);
    }

    public void leavegame()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
}
