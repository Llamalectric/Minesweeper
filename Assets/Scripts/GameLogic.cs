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
            { column - 1, row - 1 }, { column, row - 1 }, { column + 1, row - 1},
            { column - 1, row     }, /*row,     column */ { column + 1, row    },
            { column - 1, row + 1}, {column, row + 1}, {column + 1, row + 1    }
        };
        // Edge cases (pun intended)
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (neighbors[i, j] < 0) { neighbors[i, j] = 0; }
                if (neighbors[i, j] > 9) { neighbors[i, j] = 9; }
            }
        }
        
        /*if (column == 0) 
        {
            neighbors[0, 0] = neighbors[3, 0] = neighbors[5, 0] = column;
        }
        if (column == 9)
        {
            neighbors[2, 0] = neighbors[4, 0] = neighbors[7, 0] = column;
        }
        if (row == 0)
        {
            neighbors[0, 1] = neighbors[1, 1] = neighbors[2, 1] = row;
        }
        if (row == 9)
        {
            neighbors[5, 1] = neighbors[6, 1] = neighbors[7, 1] = row;
        }*/
        return neighbors;
    }
}
