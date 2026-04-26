using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JunkGroundGenerator : MonoBehaviour
{
    [SerializeField] GameObject[] groundsJunk;
    [SerializeField] Transform parent;
    [SerializeField] Tilemap[] tilemapLayer;
    [SerializeField] Vector2 offset;
    [SerializeField] float chanseGenerate;
    [SerializeField] List<GameObject> generatedJunks = new List<GameObject>();

    private void Start()
    {
        Generate();
    }
    public void Generate()
    {
        Clear();
        foreach (var tilemap in tilemapLayer)
        {
            foreach (Vector3Int vector3Int in tilemap.cellBounds.allPositionsWithin)
            {
                if(Random.Range(0, 100) > chanseGenerate) { continue; }
                Vector3Int position = new Vector3Int(vector3Int.x, vector3Int.y, vector3Int.z);
                TileBase tile = tilemap.GetTile(position);
                if (tile != null)
                {
                    Vector3 tilepos = tilemap.CellToWorld(position);
                    Vector3 randomOffset = new Vector2(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y));
                    generatedJunks.Add(Instantiate(groundsJunk[Random.Range(0, groundsJunk.Length)], tilepos + randomOffset, Quaternion.identity, parent));
                }
            }
        }
    }

    internal void Clear()
    {
        foreach (var item in generatedJunks)
        {
            DestroyImmediate(item);
        }
        generatedJunks.Clear();
    }
}
