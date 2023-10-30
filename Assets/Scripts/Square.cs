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


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        // Left Click
        if (Input.GetMouseButtonDown(0))
        {
            if (state == States.covered)
            {
                ChangeState(States.uncovered);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (state == States.covered)
            {
                ChangeState(States.flagged);
            }
        }
        if (Input.GetMouseButtonDown(2))
        {
            prevState = state;
            ChangeState(States.peeking);
        }
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
    }
}
