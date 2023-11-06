using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Square : MonoBehaviour
{
    public enum States
    {
        covered, flagged, uncovered, peeking
    }

    // For 8x8: each is .5 units
    // For 16x16: each is .25 units

    States state = States.covered;
    States prevState;

    SpriteRenderer sr;
	[SerializeField] InputActionAsset asset;
	InputAction action;
	ButtonControl button;

    [SerializeField]
    Sprite uncovered, covered, flagged;

    public int[] coords = new int[2];

	public States State() => state;

	// Start is called before the first frame update
	void Start()
    {
        sr = GetComponent<SpriteRenderer>();

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
			if (state == States.peeking)
			{
				RestoreState();
				SendMessageUpwards("StopPeeking", coords);
			}
		}
    }

	private void OnMouseOver()
	{
		// Left Click
		if (Input.GetMouseButton(0))
		{
			if (state == States.covered)
			{
				ChangeState(States.uncovered);
			}
		}
		// Right Click
		if (Input.GetMouseButtonDown(1))
		{
			if (state == States.covered)
			{
				ChangeState(States.flagged);
			}
			else if (state == States.flagged)
			{
				ChangeState(States.covered);
			}
		}
		// Middle Click, start peeking
		if (button.wasPressedThisFrame)
		{
			MakeStateBackup();
			ChangeState(States.peeking);
		}
		// Continue peeking
		if (button.isPressed)
		{
			gameObject.SendMessageUpwards("Peek", coords);
		}
	}

	public void ChangeState(States s)
    {
        state = s;
        if (state == States.covered)
        {
            sr.sprite = covered;
        } 
        else if (state == States.uncovered)
        {
            sr.sprite = uncovered;
		}
        else if(state == States.flagged)
        {
            sr.sprite = flagged;
        }
    }

    public void MakeStateBackup() { prevState = state; }

    public void RestoreState() { ChangeState(prevState); prevState = state; }
}
