using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class ItemModel
{
    List<int> _items;
    public List<Action<int>> UpdateItems;
    public ItemModel(int cnt)
    {
        _items = new List<int>(cnt);
        UpdateItems = new List<Action<int>>(cnt);
    
        for (int i = 0; i < cnt; i++)
        {
            _items.Add(0);
        }
    }

    public void GetItem(int idx)
    {
        _items[idx]++;
        UpdateItems[idx]?.Invoke(_items[idx]);
    }

    public bool UseItem(int idx)
    {
        if (_items[idx] <= 0) return false;
        _items[idx]--;
        UpdateItems[idx]?.Invoke(_items[idx]);
        return true;
    }
}
