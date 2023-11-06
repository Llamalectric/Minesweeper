using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    [SerializeField]
    GameObject SquarePrefab;

    [SerializeField]
    Sprite peekSprite;

    [SerializeField]
    Vector2 dimensions = new Vector2(8, 8);

    GameObject[,] Board;

    bool isPeeking = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Board = new GameObject[(int)dimensions.x, (int)dimensions.y];
        // Populate board
        for (int i = 0; i < dimensions.y; i++)
        {
            for(int j = 0; j < dimensions.x; j++)
            {
                Board[i, j] = Instantiate(SquarePrefab, new Vector2(-3.5f + i, 2.5f - j), Quaternion.identity);
                Board[i, j].transform.localScale = new Vector2(.4f, .4f);
                Board[i, j].name = i.ToString() + ", " + j.ToString();
                Board[i, j].GetComponent<Square>().coords[0] = i;
                Board[i, j].GetComponent<Square>().coords[1] = j;
                Board[i, j].transform.parent = transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Peek(int[] coords)
    {
        if (isPeeking) return;
        isPeeking = true;
        int[,] neighbors = FindNeighbors(coords[0], coords[1]);
        for (int i = 0; i < 8; i++)
        {
            if (Board[neighbors[i, 0], neighbors[i, 1]].GetComponent<Square>().State() == Square.States.covered)
            {
                Board[neighbors[i, 0], neighbors[i, 1]].GetComponent<SpriteRenderer>().sprite = peekSprite;
			}
        }
    }

    public void StopPeeking(int[] coords) 
    {
		isPeeking = false;
        int[,] neighbors = FindNeighbors(coords[0], coords[1]);
        for (int i = 0; i < 8; i++)
        {
            if (Board[neighbors[i, 0], neighbors[i, 1]].GetComponent<Square>().State() == Square.States.covered)
            {
                Board[neighbors[i, 0], neighbors[i, 1]].GetComponent<Square>().ChangeState(Square.States.covered);
            }
		}
	}

    private int[,] FindNeighbors(int column, int row)
    {
        int[,] neighbors = {
            { column - 1, row - 1 }, { column, row - 1 }, { column + 1, row - 1 },
            { column - 1, row     }, /*column, row     */ { column + 1, row     },
            { column - 1, row + 1 }, { column, row + 1 }, { column + 1, row + 1 }
        };
        // Edge cases (pun intended)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (neighbors[i, j] < 0) { neighbors[i, j] = 0; }
                if (neighbors[i, j] > 7) { neighbors[i, j] = 7; }
            }
        }
        
        return neighbors;
    }
}
