using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
        
    }

    private void Update()
    {
        if (targetObject == null)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player == null)
            {
                return;
            }

            targetObject = player.transform;
        }

        Vector4[] playerPositions = new Vector4[1];
        playerPositions[0] = targetObject.position;

        Shader.SetGlobalVectorArray("_PlayerPositions", playerPositions);
        Shader.SetGlobalInt("_PlayerCount", 1);
        Shader.SetGlobalFloat("_CutoutSize", 2.0f);
        Shader.SetGlobalFloat("_FalloffSize", 1.0f);
        return;

        /*Vector2 cutoutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Material[] materials = hitObjects[i].transform.GetComponent<Renderer>().materials;

            for (int m = 0; m < materials.Length; ++m)
            {
                
            }
        }*/
    }
}
