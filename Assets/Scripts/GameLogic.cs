using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    [SerializeField]
    GameObject SquarePrefab;

    GameObject[,] Board = new GameObject[10, 10];
    
    // Start is called before the first frame update
    void Start()
    {
        // Populate board
        for (int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                Board[i, j] = Instantiate(SquarePrefab, new Vector2(-4.5f + i, 4.5f - j), Quaternion.identity);
                Board[i, j].transform.localScale = new Vector2(.4f, .4f);
                Board[i, j].name = i.ToString() + ", " + j.ToString();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
