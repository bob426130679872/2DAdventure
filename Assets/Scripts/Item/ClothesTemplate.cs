using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Clothes")]
public class ClothesTemplate : ItemTemplate
{
    // --- 裝備部位 ---
    public ClothesType clothesType; // 例如：Head, Body, Legs, Accessory

    // --- 數值加成 ---
    [Header("Stat Bonuses")]
    public float HPBonus;  // 最大生命值加成
    public float attackBonus;     // 攻擊力加成
    public float defenseBonus;    // 防禦力加成
    public float speedBonus;      // 移動速度加成
    public float MPBonus;      
}