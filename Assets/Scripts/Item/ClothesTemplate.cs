using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Clothes")]
public class ClothesTemplate : ItemTemplate
{
    
    public ClothesType clothesType; 
    
    [Header("Stat Bonuses")]
    public float HPBonus;
    public float attackBonus;
    public float defenseBonus;
    public float speedBonus;
    public float MPBonus;

    [Header("能力說明")]
    [TextArea(2,5)] public string abilityText;
    [TextArea(2,5)] public string abilityTextEN;
}