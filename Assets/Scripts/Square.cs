using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [SerializeField]
    Sprite uncovered, covered, flagged;

    public int[] coords = new int[2];

	public States State() => state;

	// Start is called before the first frame update
	void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.peeking)
        {
            gameObject.SendMessageUpwards("Peek", coords);
        } 
    }

	private void OnMouseOver()
	{
		// Left Click
        if (Input.GetMouseButtonDown(0))
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
        if (Input.GetMouseButtonDown(2))
        {
            MakeStateBackup();
            ChangeState(States.peeking);
        }
        // Stop peeking 
        if (Input.GetMouseButtonUp(2))
        {
            if (state == States.peeking)
            {
                RestoreState();
                SendMessageUpwards("StopPeeking", coords);
            }
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
