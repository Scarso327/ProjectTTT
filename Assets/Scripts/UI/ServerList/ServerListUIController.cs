using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ExitGames.Client.Photon;

namespace ExitGames.Demos.UI
{
    public class ServerListUIController : MonoBehaviour
    {

        public GameObject UIList;

        public GameObject ServerItemPrefab;

        Dictionary<string, ServerItem> _items = new Dictionary<string, ServerItem>();
        
        /// <summary>
        /// Updates the UI listing, it creates the necessary items not yet listed, update existing items and remove unused entries
        /// </summary>

        private void Awake()
        {
            RefreshList();
        }

        public void UpdateUI()
        {
            List<string> processedIDS = new List<string>();
            //Debug.Log(PhotonNetwork.countOfRooms);
            // update existing items and add new items
            foreach (RoomInfo room in PhotonNetwork.GetRoomList())
            {

                if (_items.ContainsKey(room.Name)) // update
                {
                    _items[room.Name].RefreshData(room);
                    processedIDS.Add(room.Name);

                }
                else
                { // create new
                    GameObject _item = (GameObject)Instantiate(ServerItemPrefab);
                    _item.transform.SetParent(UIList.transform);

                    ServerItem _serverItem = _item.GetComponent<ServerItem>();
                    _items.Add(room.Name, _serverItem);
                    _items[room.Name].RefreshData(room);

                    _serverItem.AnimateRevealItem();

                    processedIDS.Add(room.Name);
                }
            }

            // now deal with items that needs to be deleted.
            // work in reverse so that we can delete entries without worries.
            foreach (var _item in _items.Reverse())
            {
                if (!processedIDS.Contains(_item.Key))
                {
                    _items[_item.Key].AnimateRemoveItem();
                    _items.Remove(_item.Key);
                }
            }

        }

        /// <summary>
        /// Cleans up list to prevent memory leak.
        /// </summary>
        public void CleanUpList()
        {
            _items = new Dictionary<string, ServerItem>();
            foreach (Transform child in UIList.transform)
            {
                Destroy(child.gameObject);
            }
        }

        public void RefreshList()
        {
            CleanUpList();
            UpdateUI();
        }

        public void OnReceivedRoomListUpdate()
        {
            CleanUpList();
            UpdateUI();
        }

        public void OnCreatedRoom()
        {
            CleanUpList();
            UpdateUI();
        }
    }
}