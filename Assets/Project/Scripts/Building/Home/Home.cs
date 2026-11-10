using UnityEngine;

public class House : GridObject
{
    protected override void OnClick()
    {
        Debug.Log($"Mở menu thông tin của ngôi nhà: {objectName}");
    }

    protected override void OnDoubleClick()
    {
        Debug.Log($"Mở giao diện nâng cấp nhanh cho: {objectName}");
    }
}