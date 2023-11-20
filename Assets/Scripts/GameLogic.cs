using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameLogic : MonoBehaviour
{

	[SerializeField]
	GameObject SquarePrefab;

	[SerializeField]
	Sprite peekSprite, dead_smiley, peek_smiley, win_smiley;

	[SerializeField]
	Vector2 dimensions = new(8, 8);

	[SerializeField] int numMines = 10;

	[SerializeField]
	TMPro.TMP_Text flagCounter, stopwatch;

	int numFlags;
	float time;

	GameObject[,] Board;
	List<Square> Mines;

	bool isPeeking = false, hasRandomizedMines = false;
	public bool IsGameOver { get; set; }

	// Start is called before the first frame update
	void Start()
	{
		numFlags = numMines;
		Mines = new();
		IsGameOver = false;
		Board = new GameObject[(int)dimensions.x, (int)dimensions.y];
		// Populate board
		for (int i = 0; i < dimensions.y; i++)
		{
			for (int j = 0; j < dimensions.x; j++)
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
			GameObject.FindGameObjectWithTag("smiley").GetComponent<Image>().sprite = win_smiley;
			IsGameOver = true;
		}
		// Update stopwatch
		if (hasRandomizedMines && time <= 999 && !IsGameOver)
		{
			time += Time.deltaTime;
			stopwatch.SetText(String.Format("{0, 0:D3}", (int)time));
		}
	}

	public void Peek(int[] coords)
	{
		if (isPeeking) return;
		isPeeking = true;
		bool uncover = false;
		List<Square> neighbors = FindNeighbors(coords[0], coords[1]);

		if (Board[coords[0], coords[1]].GetComponent<Square>().PrevState == Square.State.uncovered)
		{
			uncover = HasFoundAllMines(coords) && !Board[coords[0], coords[1]].GetComponent<Square>().IsMine;
		}

		if (uncover)
		{
			FloodClear(coords);
		}


		for (int i = 0; i < neighbors.Count; i++)
		{
			if (Board[neighbors[i].coords[0], neighbors[i].coords[1]].GetComponent<Square>().CurrState == Square.State.covered)
			{
				if (!uncover)
				{
					Board[neighbors[i].coords[0], neighbors[i].coords[1]].GetComponent<SpriteRenderer>().sprite = peekSprite;
				}
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
		Debug.Log("Game Over!");
		IsGameOver = true;
		GameObject.FindGameObjectWithTag("smiley").GetComponent<Image>().sprite = dead_smiley;
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
			for (int i = 0; i < numMines; i++)
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

	public bool HasFoundAllMines(int[] coords)
	{
		int foundMines = 0;
		List<Square> neighbors = FindNeighbors(coords[0], coords[1]);
		foreach (Square square in neighbors)
		{
			if (square.CurrState == Square.State.flagged)
			{
				foundMines++;
			}
		}
		return foundMines == Board[coords[0], coords[1]].GetComponent<Square>().AdjacentMines;
	}

	public bool PlantFlag()
	{
		if (numFlags > 0)
		{
			numFlags--;
			flagCounter.SetText(String.Format("{0, 0:D3}", numFlags));
			return true;
		}
		return false;
	}

	public void RemoveFlag()
	{
		numFlags++;
		flagCounter.SetText(String.Format("{0, 0:D3}", numFlags));
	}

	public void FloodClear(int[] coords)
	{
		/*
		 * 
		 * n = getneighbors(coords)
		 * foreach square in n:
		 *	if square should be uncovered:
		 *		uncover
		 *	floodClear(n)
		 *	
		 */
		Stack<int[]> stack = new();
		stack.Push(coords);

		while (stack.Count > 0)
		{
			int[] currCoords = stack.Pop();

			if (Board[currCoords[0], currCoords[1]].GetComponent<Square>().CurrState != Square.State.covered) { continue; }

			if (HasFoundAllMines(currCoords) && !Board[currCoords[0], currCoords[1]].GetComponent<Square>().IsMine)
			{
				Board[currCoords[0], currCoords[1]].GetComponent<Square>().ChangeState(Square.State.uncovered);
				Board[currCoords[0], currCoords[1]].GetComponent<Square>().MakeStateBackup();
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						int newX = currCoords[0] + i;
						int newY = currCoords[1] + j;
						if (newX >= 0 && newX < dimensions.y && newY >= 0 && newY < dimensions.x)
						{
							if (Board[newX, newY].GetComponent<Square>().CurrState == Square.State.covered)
							{
								Board[newX, newY].GetComponent<Square>().ChangeState(Square.State.uncovered);
							}
							stack.Push(new int[] { newX, newY });
						}
					}
				}
			}
		}
	}


}
