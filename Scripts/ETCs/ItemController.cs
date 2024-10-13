using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private List<ItemViewer> _itemViewers;
    ItemModel _itemModel;
    [SerializeField] private Player _player;
    
    
    public Action<int, bool> useItem;
    
    public PhotonView PV;
    
    // Start is called before the first frame update
    void Start()
    {
        _itemModel = new ItemModel(_itemViewers.Count);
        useItem = _player.UseItem;
        
        for (int i = 0; i < _itemViewers.Capacity; i++)
        {
            _itemModel.UpdateItems.Add(_itemViewers[i].UpdateCount);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if(_itemModel.UseItem(0))
                    useItem?.Invoke(20, false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                if(_itemModel.UseItem(1))
                    useItem?.Invoke(60, false);
            }

            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                if(_itemModel.UseItem(2))
                    useItem?.Invoke(50, true);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().PickedEvent += GetItem;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            other.GetComponent<Item>().PickedEvent -= GetItem;
        }
    }
    
    private void GetItem(int itemID)
    {
        if (Input.GetKeyDown(KeyCode.E) && PV.IsMine)
        {
            _itemModel?.GetItem((int)ItemManager.instance.itemDict[itemID].type);
            PV.RPC("GetItemRPC", RpcTarget.AllBuffered, itemID);
        }
    }
    
    [PunRPC]
    public void GetItemRPC(int itemID)
    {
        var _item = ItemManager.instance.itemDict[itemID];
        _item.PickedEvent -= GetItem;
        _item.Picked();
    }
}
