using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemViewer : MonoBehaviour
{
    private Image img;
    [SerializeField] private TextMeshProUGUI count;
    Color emptyColor = new Color(1, 1, 1, 0.34f);
    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        count.text = "";
    }

    public void UpdateCount(int cnt)
    {
        if (cnt <= 0)
        {
            count.text = "";
            img.color = emptyColor;
        }
        else
        {
            count.text = cnt.ToString();
            img.color = Color.white;
        }
    }
}
