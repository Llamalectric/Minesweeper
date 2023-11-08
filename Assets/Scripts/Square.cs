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
	State prevState;

	public State CurrState { get; private set; }

	SpriteRenderer sr;
	GameLogic logic;
	[SerializeField] InputActionAsset asset;
	InputAction action;
	ButtonControl button;

	[SerializeField]
	Sprite uncovered, covered, flagged, mine;

	public bool IsMine { get; set; }

	public int[] coords = new int[2];

	// Start is called before the first frame update
	void Start()
    {
        sr = GetComponent<SpriteRenderer>();
		logic = GameObject.Find("GameLogic").GetComponent<GameLogic>();

		action = asset.FindAction("Peek");

		button = (ButtonControl)action.controls[0];
		action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
		// Stop peeking 
		if (button.wasReleasedThisFrame)
		{
			if (CurrState == State.peeking)
			{
				RestoreState();
				logic.StopPeeking(coords);
			}
		}

		if (IsMine)
		{
			uncovered = mine;
		}
    }

	private void OnMouseOver()
	{
		// Left Click
		if (Input.GetMouseButtonDown(0))
		{
			if (CurrState == State.covered)
			{
				logic.RandomizeMines(coords);
				ChangeState(State.uncovered);
			}
			if (IsMine)
			{
				logic.GameOver();
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
				ChangeState(State.covered);
			}
		}
		// Middle Click, start peeking
		if (button.wasPressedThisFrame)
		{
			MakeStateBackup();
			ChangeState(State.peeking);
		}
		// Continue peeking
		if (button.isPressed)
		{
			logic.Peek(coords);
		}
	}

	public void ChangeState(State s)
    {
        CurrState = s;
        if (CurrState == State.covered)
        {
            sr.sprite = covered;
        } 
        else if (CurrState == State.uncovered)
        {
            sr.sprite = uncovered;
		}
        else if(CurrState == State.flagged)
        {
            sr.sprite = flagged;
        }
    }

    public void MakeStateBackup() { prevState = CurrState; }

    public void RestoreState() { ChangeState(prevState); prevState = CurrState; }
}
