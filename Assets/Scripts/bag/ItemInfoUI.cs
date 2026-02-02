using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemInfoUI : MonoBehaviour
{
    
    public Image icon;
    public Text nameText;
    public Text descriptionText;

    public void ShowInfo(ItemTemplate itemTemplate)
    {

        if (itemTemplate == null) return;        
        icon.sprite = itemTemplate.icon;
        nameText.text = itemTemplate.displayName;
        descriptionText.text = itemTemplate.description;
    }
}
