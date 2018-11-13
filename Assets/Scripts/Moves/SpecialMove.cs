using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*To make a special move
  Make a script, like DodgeDash and give it a Type. In playerController check for the type and then add the move component to the player
     */

//The abstract class that all the moves will draw from 
public abstract class SpecialMove : MonoBehaviour {

    public PlayerController controller; //references the controller of the player doing the move 

    public SpecialMove() {}

    public SpecialMove(PlayerController controller) 
    {
        this.controller = controller;
    }

    public abstract void SetData(PlayerController controller, SpecialMoveData data); //used so derived classes of SpecialMove can have derivations of SpecialMoveData

    public abstract void Activate(); //starts the special move

}
