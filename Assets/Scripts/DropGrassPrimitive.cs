/*using UnityEngine;
using UnityEditor;
using System.Collections;

public class DropGrassPrimitive : MonoBehaviour
{
    public int numberOfObjects = 100;
    public GameObject[] objectsToPlace;

    public void Generate()
    {
        if (objectsToPlace == null || objectsToPlace.Length <= 0 || numberOfObjects <= 0)
        {
            Debug.LogError("One of the input parameters is invalid. Confirm that the \"Objects To Place\" field has at least one valid object and the \"Number of Objects\" value is greater than 0.");
            return;
        }

        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogError("No object selected.");
            return;
        }

        Terrain selectedTerrain = selected.GetComponent<Terrain>();
        if (selectedTerrain == null)
        {
            Debug.LogError("Object selection does not have a Terrain component.");
            return;
        }

        int width = (int)selectedTerrain.terrainData.size.x;
        int length = (int)selectedTerrain.terrainData.size.z;
        int x = (int)selectedTerrain.transform.position.x;
        int z = (int)selectedTerrain.transform.position.z;

        Debug.Log("Placing " + numberOfObjects + " objects of " + objectsToPlace.Length + " types.");

        for (int i = 0; i < numberOfObjects; i++)
        {
            int posx = Random.Range(x, x + width);
            int posz = Random.Range(z, z + length);
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz));
            GameObject newObj = (GameObject)Instantiate(objectsToPlace[i % objectsToPlace.Length], new Vector3(posx, posy + selected.transform.position.y, posz), Quaternion.Euler(0, Random.Range(0, 359), 0));
            newObj.transform.parent = selected.transform;
        }

        Debug.Log("Object placement complete.");
    }
}

[CustomEditor(typeof(DropGrassPrimitive))]
public class DropGrass : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
            ((DropGrassPrimitive)target).Generate();
    }   
}
*/