using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterProfile
{
    public int schemaVersion = 1;
    public string characterAddressKey = string.Empty;
    public List<InventoryItemData> inventory = new List<InventoryItemData>();
    public List<int> ownedMaterialIds = new List<int>();
    public List<int> ownedSkillIds = new List<int>();
    public string equippedWeaponId = "sword";
    public int equippedMaterialId;
    public Color equippedMaterialColor = new Color32(52, 152, 219, 255);
    public List<int> equippedSkillIds = new List<int>();
}

[Serializable]
public class InventoryItemData
{
    public int itemId;
    public int quantity;
}
