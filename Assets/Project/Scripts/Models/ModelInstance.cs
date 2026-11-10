using UnityEngine;
using System;
public class PlayerModel
{
    public int id;
    public string modelName;
    public int experience;
    public int level;
    public string image;
    public int gold;
    public int diamond;

}
public class ItemModel
{
    public int id;
    public string itemName;
    public string itemType;
    public int levelRequirement;
    public int price;
    public string description;
    public string pathPrefab;
}
// public class InventoryModel
// {
//     public System.Guid id;
//     public System.Guid playerId;
// }
public class ItemOfInventoryModel
{
    public int id;
    // public int inventoryId;
    public int itemId;
    public int playerId;
    public int quantity;
}

public class ItemOnGridModel
{
    public int id;
    public int cellX;
    public int cellZ;
    public int itemId;
    public int playerId;
    public float[] position = new float[3];
    public float[] rotation = new float[4];
    public float[] scale = new float[3];
}
