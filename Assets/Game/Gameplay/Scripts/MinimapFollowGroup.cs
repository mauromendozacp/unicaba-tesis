using Unity.Cinemachine;
using UnityEngine;

public class MinimapFollowGroup : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;
    public float height = 50f;       // Altura del minimapa
    public float smooth = 5f;        // Suavizado del movimiento

    private Vector3 velocity;

    void LateUpdate()
    {
        if (targetGroup == null) return;

        // Obtiene el punto medio de los jugadores
        Vector3 center = targetGroup.transform.position;

        // También podés usar el bounding box:
        // Vector3 center = targetGroup.BoundingBox.center;

        // Posiciona la cámara del minimapa sobre ese punto
        Vector3 desiredPos = center + Vector3.up * height;

        // Movimiento suave
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, 1f / smooth);

        // Asegura que mire hacia abajo
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
