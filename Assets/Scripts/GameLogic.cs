using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameLogic : MonoBehaviour
{

    [SerializeField]
    GameObject SquarePrefab;

    [SerializeField]
    Sprite peekSprite;

    [SerializeField]
    Vector2 dimensions = new Vector2(8, 8);

    GameObject[,] Board;
    List<Square> Mines;

    bool isPeeking = false;
    bool hasRandomizedMines = false;
    public bool IsGameOver { get; set; }
    
    // Start is called before the first frame update
    void Start()
    {
        Mines = new();
        IsGameOver = false;
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
        // Check for victory
        if (HasWon() && hasRandomizedMines && !IsGameOver)
        {
            Debug.Log("You win!");
            IsGameOver = true;
        }
    }

    public void Peek(int[] coords)
    {
        if (isPeeking) return;
        isPeeking = true;
        List<Square> neighbors = FindNeighbors(coords[0], coords[1]);
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].CurrState == Square.State.covered)
            {
				Board[neighbors[i].coords[0], neighbors[i].coords[1]].GetComponent<SpriteRenderer>().sprite = peekSprite;
			}
        }
    }

    public void StopPeeking(int[] coords) 
    {
		isPeeking = false;
        List<Square> neighbors = FindNeighbors(coords[0], coords[1]);
        for (int i = 0; i < neighbors.Count; i++)
        {
            if (neighbors[i].CurrState == Square.State.covered)
            {
                neighbors[i].ChangeState(Square.State.covered);
            }
		}
	}

    private List<Square> FindNeighbors(int column, int row)
    {
        List<Square> neighbors = new();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (column + i < 0 || column + i > dimensions.y - 1 || row + j < 0 || row + j > dimensions.x - 1) { continue; }
                if (i == 0 && j == 0) { continue; }
                neighbors.Add(Board[column + i, row + j].GetComponent<Square>());
            }
        }
        
        return neighbors;
    }

    public void GameOver()
    {
        // TODO: Handle game over
        Debug.Log("Game Over!");
        IsGameOver = true;
        for (int i = 0; i < Mines.Count; i++)
        {
            Mines[i].ChangeState(Square.State.uncovered);
        }
    }

    public void RandomizeMines(int[] coords)
    {
        int randomy, randomx;
		randomy = Random.Range(0, 8);
		randomx = Random.Range(0, 8);
        List<Square> neighbors;
		if (!hasRandomizedMines)
        {
            hasRandomizedMines = true;
            for (int i = 0; i < 10; i++)
            {
                
                while (Board[randomy, randomx].GetComponent<Square>().IsMine || (coords != null && randomy == coords[0] && randomx == coords[1]))
                {
                    randomy = Random.Range(0, 8);
                    randomx = Random.Range(0, 8);
                }
                Board[randomy, randomx].GetComponent<Square>().IsMine = true;
                Mines.Add(Board[randomy, randomx].GetComponent<Square>());
                
                // Update count for adjacent squares
                neighbors = FindNeighbors(randomy, randomx);
                for (int n = 0; n < neighbors.Count; n++)
                {
                    neighbors[n].AdjacentMines++;
                }

			}

        }
    }

    private bool HasWon()
    {
        bool hasWon = true;
        foreach (Square square in Mines)
        {
            if (square.CurrState != Square.State.flagged)
            {
                hasWon = false;
            }
        }
        return hasWon;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
