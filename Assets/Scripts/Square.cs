using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    enum States
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

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        }
        // Middle Click, start peeking
        if (Input.GetMouseButtonDown(2))
        {
            prevState = state;
            ChangeState(States.peeking);
        }
        // Stop peeking 
        if (Input.GetMouseButtonUp(2))
        {
            if (state == States.peeking)
            {
                ChangeState(prevState);
            }
        }
	}

	private void ChangeState(States s)
    {
        Debug.Log(s.ToString());
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
}
