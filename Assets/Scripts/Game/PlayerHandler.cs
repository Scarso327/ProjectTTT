using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{

    public int Health = 100;

    public void toggleControl(bool toggle)
    {
        UnityStandardAssets.Characters.FirstPerson.FirstPersonController control = this.gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
        control.enabledControls = toggle;
    }

    [PunRPC]
    public void takeDamage(int dmg)
    {
        Health -= dmg;

        if(Health <= 0)
        {
            destroyThis("killed");
        }
    }

    [PunRPC]
    public void destroyThis(string reason)
    {
        if (reason == "killed")
        {
            if (GetComponent<PhotonView>().instantiationId == 0)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (GetComponent<PhotonView>().isMine)
                {
                    PhotonNetwork.Destroy(this.gameObject);

                    // Tell the whole game we are killed dead.
                    GameObject scriptObj = GameObject.FindGameObjectWithTag("SceneObject");
                    GameHandler tmp = (GameHandler)scriptObj.GetComponent(typeof(GameHandler));
                    tmp.playerDied(this.gameObject.GetComponent<PhotonView>().owner);
                }
            }
        } else if (reason == "RoundEnd")
        {
            if (GetComponent<PhotonView>().isMine)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}