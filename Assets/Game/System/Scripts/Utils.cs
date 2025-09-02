using UnityEngine;

public class Utils : MonoBehaviour
{
    public static bool CheckLayerInMask(LayerMask mask, int layer)
    {
        return mask == (mask | (1 << layer));
    }
}