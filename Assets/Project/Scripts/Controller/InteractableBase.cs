using UnityEngine;

public abstract class InteractableBase : ConfiguredMonoBehaviour
{
    public virtual void OnHoverEnter() { }
    public virtual void OnHoverExit() { }

    // Bắt buộc khai báo đúng tên này để các class con override
    public abstract void Interact(GameObject interactorGO);
}