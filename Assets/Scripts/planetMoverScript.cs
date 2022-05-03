using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planetMoverScript : MonoBehaviour
{
    //moving variables
    //MoveIn will be the values fed in from the gesture monitor, slowingspeed will be at what rate the planet slows when no longer being fed data, moveThrehshold will be thevalue move in must be above to avoid slowing
    //moveSpeed will be the speed the planet moves at when moveIn is above moveThresh
    //moveDireciton will be 1 or -1 and used to reverse direciton based off of input action
    public float moveIn, slowingSpeed, moveThreshold, moveSpeed, moveDirection;

    //moving speed is the speed the planet is moving at
    private float movingSpeed;
    private Vector3 lastpos;


    // Update is called once per frame
    void Update()
    {

        if (moveIn > moveThreshold * Time.deltaTime) 
        {
            movingSpeed = (moveSpeed * moveDirection) * Time.deltaTime;
        }
        else if(movingSpeed != 0)
        {
            movingSpeed -= (slowingSpeed * moveDirection) * Time.deltaTime;

            switch(moveDirection)
            {
                //moving away
                case 1:

                    if(movingSpeed < 0)
                    {
                        movingSpeed = 0;
                    }

                    break;

                //moving towards
                case -1:

                    if (movingSpeed > 0)
                    {
                        movingSpeed = 0;
                    }

                    break;

                default:

                    break;
            }
        }

        if(movingSpeed != 0)
        {
            transform.position += new Vector3(0.0f, 0.0f, movingSpeed);
        }

        lastpos = transform.position;
    }
}
