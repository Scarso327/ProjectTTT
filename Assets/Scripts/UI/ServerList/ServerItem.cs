using System;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace ExitGames.Demos.UI
{
    public class ServerItem : MonoBehaviour
    {
        public Text PlayerNameText;
        public Text SlotText;
        public Text StatusText;
        public Text MapText;

        public GameObject Locked;
        public GameObject Unlocked;

        public float AnimationSpeed = 5f;

        LayoutElement _layoutElement;
        CanvasGroup _canvasGroup;

        float preferredHeight;
        float animatedTargetHeight;
        float animatedTargetAlpha;

        void awake()
        {
            if (_layoutElement == null)
            {
                _layoutElement = GetComponent<LayoutElement>();
                preferredHeight = _layoutElement.preferredHeight;

                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        public void RefreshData(RoomInfo roomInfo)
        {
            this.gameObject.SetActive(true);
            PlayerNameText.text = roomInfo.Name;

            string passworded = roomInfo.CustomProperties["P"].ToString();

            if(passworded == "1")
            {
                Locked.SetActive(true);
                Unlocked.SetActive(false);
            } else
            {
                Locked.SetActive(false);
                Unlocked.SetActive(true);
            }

            SlotText.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;

            string currentMap = roomInfo.CustomProperties["M"].ToString();

            MapText.text = currentMap;

            string currentStatus = roomInfo.CustomProperties["S"].ToString();

            switch (currentStatus)
            {
                case "0":
                    StatusText.text = "Creating";
                    break;
                case "1":
                    StatusText.text = "Prepearing";
                    break;
                case "2":
                    StatusText.text = "Playing";
                    break;
                case "3":
                    StatusText.text = "Ending";
                    break;
                default:
                    StatusText.enabled = false;
                    break;
            }

            Button b = this.gameObject.GetComponent<Button>();

            if (b == null)
            {
                Debug.LogError(b + " does not have a button script!");
            }
            else
            {
                b.onClick.AddListener(
                    delegate {
                        string password = roomInfo.CustomProperties["PW"].ToString();

                        if (passworded == "1")
                        {
                            // Setup the ask password stuff.
                            GameObject networkObject = GameObject.FindGameObjectWithTag("NetworkObject");
                            
                            if(networkObject != null)
                            {
                                // Tell our networking script to remember these as we need them later.
                                Networking networkScript = networkObject.GetComponent<Networking>();
                                networkScript.password = password;
                                networkScript.roomName = roomInfo.Name;

                                // Enable the input for the password.
                                GameObject scriptObject = GameObject.FindGameObjectWithTag("ScriptObject");
                                MenuSetup createUI = (MenuSetup)scriptObject.GetComponent(typeof(MenuSetup));

                                createUI.PasswordMaster.SetActive(true);
                        } else
                            {
                                Debug.LogError("Network Object is missing. (Called by ServerItem: " + b + ")");
                            }
                        } else
                        {
                            PhotonNetwork.JoinRoom(roomInfo.Name);
                        }
                    }
                );
            }
        }

        public void AnimateRevealItem()
        {
            if (this.transform.parent.gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateRevealItem_cr());
                transform.localScale = new Vector3(1, 1, 1);
            }

        }

        IEnumerator AnimateRevealItem_cr()
        {
            if (_layoutElement == null) awake();

            animatedTargetHeight = preferredHeight;

            _layoutElement.preferredHeight = 0;
            _canvasGroup.alpha = 0f;

            yield return new WaitForEndOfFrame();

            while (true)
            {
                _layoutElement.preferredHeight = Mathf.Lerp(_layoutElement.preferredHeight, animatedTargetHeight, AnimationSpeed * Time.deltaTime);

                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, AnimationSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();

                if (_layoutElement.preferredHeight >= animatedTargetHeight) break;
            }
        }

        public void AnimateRemoveItem()
        {
            if (this.transform.parent.gameObject.activeInHierarchy)
            {
                StartCoroutine(AnimateRemoveItem_cr());
            }
        }

        IEnumerator AnimateRemoveItem_cr()
        {
            if (_layoutElement == null) awake();

            animatedTargetHeight = 0;

            yield return new WaitForEndOfFrame();

            while (true)
            {
                _layoutElement.preferredHeight = Mathf.Lerp(_layoutElement.preferredHeight, animatedTargetHeight, 5f * Time.deltaTime);
                _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, AnimationSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();

                if (_layoutElement.preferredHeight <= animatedTargetHeight) break;
            }

            Destroy(this.gameObject);
        }
    }
}