using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Turn
{
    player1,
    player2,
    idle,
    pause
}

public enum GameState
{
    active,
    inactive
}

public class StateMachine : MonoBehaviour {

    public static GameState state;
    public static Turn turnState;
    public static Turn holdTurn;

	public static void initiateTurns()
    {
        if(state == GameState.active) turnState = Turn.player1;
    }

    public static void passTurn()
    {
        if (state == GameState.active)
        {
            switch (turnState)
            {
                case Turn.idle:
                case Turn.player1:
                    turnState = Turn.player2;
                    break;
                case Turn.player2:
                    turnState = Turn.player1;
                    break;
                case Turn.pause:
                    print("Game paused, can't pass turn now!");
                    break;
            }
        }
    }

    public static void pause()
    {
        if (turnState != Turn.pause)
        {
            holdTurn = turnState;
            turnState = Turn.pause;
        }
        else
        {
            print("Can't pause, game is already paused!");
        }
    }

    public static void unPause()
    {
        if (turnState == Turn.pause)
        {
            turnState = holdTurn;
            holdTurn = Turn.idle;
        }
        else
        {
            print("Game isn't paused, you can't unpause it!");
        }
    }


    public static void activate()
    {
        state = GameState.active;
    }

    public static void deactivate()
    {
        state = GameState.inactive;
    }

}
