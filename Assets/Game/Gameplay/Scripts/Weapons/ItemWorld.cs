using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    public void SetData(ItemData data)
    {

    }

    public void Get()
    {
        gameObject.SetActive(true);
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }
}
