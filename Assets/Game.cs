using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField]
    Vector2Int boardSize = new Vector2Int(5, 5);

    [SerializeField]
    Gameboard board = default;

    void Awake()
    {
        board.Initialize(boardSize);
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
