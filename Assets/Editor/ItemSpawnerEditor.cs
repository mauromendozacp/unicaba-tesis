using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ItemSpawner))]
public class ItemSpawnerEditor : Editor
{
    private int selectedIndex = 0;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var spawner = (ItemSpawner)target;
        var spawnSettings = spawner.GetSpawnSettings();

        if (spawnSettings != null && spawnSettings.Count > 0)
        {
            string[] options = spawnSettings.Select(s => s.Name).ToArray();

            if (selectedIndex < 0 || selectedIndex >= options.Length)
                selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Select Object", selectedIndex, options);
        }
        else
        {
            EditorGUILayout.LabelField("No hay items configurados.");
        }
    }

    private void OnSceneGUI()
{
    var spawner = (ItemSpawner)target;
    var spawnSettings = spawner.GetSpawnSettings();

    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

    Handles.color = Color.green;
    foreach (var settings in spawnSettings)
    {
        foreach (var pos in settings.SpawnPositions)
        {
            Handles.SphereHandleCap(0, pos, Quaternion.identity, 0.5f, EventType.Repaint);
        }
    }

    Event e = Event.current;
    if (e.type == EventType.MouseDown && e.button == 0 && e.control)
    {
        // 🔹 Plano en Y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            spawnSettings[selectedIndex].SpawnPositions.Add(hitPoint);
            EditorUtility.SetDirty(spawner);
            Debug.Log($"Agregado punto en {hitPoint}");
        }

        e.Use();
    }


    }
}
