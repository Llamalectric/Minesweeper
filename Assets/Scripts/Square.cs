using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Square : MonoBehaviour
{
	public enum State
	{
		covered, flagged, uncovered, peeking
	}

	// For 8x8: each is .5 units
	// For 16x16: each is .25 units

	State currState = State.covered;

	public State CurrState { get; private set; }
	public State PrevState { get; private set; }

	SpriteRenderer sr;
	GameLogic logic;
	[SerializeField] InputActionAsset asset;
	InputAction action;
	ButtonControl button, button2;

	[SerializeField]
	Sprite covered, flagged, mine;
	[SerializeField] Sprite[] numbers;

	public bool IsMine { get; set; }
	public int AdjacentMines { get; set; }

	public int[] coords = new int[2];

	// Start is called before the first frame update
	void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		logic = GameObject.Find("GameLogic").GetComponent<GameLogic>();

		action = asset.FindAction("Peek");

		button = (ButtonControl)action.controls[0];
		button2 = (ButtonControl)action.controls[1];
		action.Enable();

	}

	// Update is called once per frame
	void Update()
	{
		// Stop peeking 
		if (button.wasReleasedThisFrame || button2.wasReleasedThisFrame)
		{
			if (CurrState == State.peeking)
			{
				RestoreState();
				logic.StopPeeking(coords);
			}
		}

		if (IsMine)
		{
			AdjacentMines = 0;
			numbers[0] = mine;
		}
	}

	private void OnMouseOver()
	{
		if (!logic.IsGameOver)
		{
			// Left Click
			if (Input.GetMouseButtonDown(0))
			{
				if (CurrState == State.covered)
				{
					logic.RandomizeMines(coords);
					ChangeState(State.uncovered);
				}

			}
			// Right Click
			if (Input.GetMouseButtonDown(1))
			{
				if (CurrState == State.covered)
				{
					ChangeState(State.flagged);
				}
				else if (CurrState == State.flagged)
				{
					logic.RemoveFlag();
					ChangeState(State.covered);
				}
			}
			// Middle Click, start peeking
			if (button.wasPressedThisFrame || button2.wasPressedThisFrame)
			{
				MakeStateBackup();
				ChangeState(State.peeking);
			}
			// Continue peeking
			if (button.isPressed || button2.isPressed)
			{
				logic.Peek(coords);
			}
		}
	}

	public void ChangeState(State s)
	{
		MakeStateBackup();
		CurrState = s;
		switch (CurrState)
		{
			case State.covered:
				sr.sprite = covered;
				break;

				
			case State.uncovered:
				if (IsMine && !logic.IsGameOver)
				{
					logic.GameOver();
				}
				sr.sprite = numbers[AdjacentMines];
				break;


			case State.flagged:
				if (!logic.PlantFlag())
				{
					RestoreState();
					break;
				}
				sr.sprite = flagged;
				break;
		}
	}

	public void MakeStateBackup() { PrevState = CurrState; }

	public void RestoreState() { ChangeState(PrevState); PrevState = CurrState; }
}
