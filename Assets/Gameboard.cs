using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameboard : MonoBehaviour 
{

    Vector2Int size;

    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    public void Initialize (Vector2Int size)
    {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);

        Vector2 offset = new Vector2(
            (size.x - 1) * 0.5f, (size.y - 1) * 0.5f
        );

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                GameTile tile = Instantiate(tilePrefab);
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y
                );
            }
        }
    }

    // Start is called before the first frame update
    void Start() 
    {
        
    }

    // Update is called once per frame
    void Update() 
    {
        
    }
}
