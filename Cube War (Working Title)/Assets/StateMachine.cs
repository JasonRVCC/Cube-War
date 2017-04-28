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

public enum GamePhase
{
    setup,
    battle,
	gameOver
}

public class StateMachine : MonoBehaviour {

    public static GameState state;
    public static Turn turnState;
    public static Turn holdTurn;
    public static GamePhase gamePhase;
	public static bool cubePlace = false;
    public static bool p1Setup = false;
    public static bool p2Setup = false;
    public static bool p1King = false;
    public static bool p2King = false;





    public static void initiateTurns()
    {
        if (state == GameState.active)
        {
            turnState = Turn.player1;
            GameDriver.updateTurnInterface();
        }
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
                    if (getPhase() == GamePhase.battle) GameDriver.clearFlicks();
                    if (getPhase() == GamePhase.setup && p2Setup == false) turnState = Turn.player1;
                    GameDriver.updateTurnInterface();
                    GameDriver.updateSetupInterface();
                    if (getPhase() == GamePhase.setup && p1Setup == false && p2Setup == false) GameDriver.endSetup();
                        break;
                case Turn.player2:
                    turnState = Turn.player1;
                    if (getPhase() == GamePhase.battle) GameDriver.clearFlicks();
                    if (getPhase() == GamePhase.setup && p1Setup == false) turnState = Turn.player2;
                    GameDriver.updateTurnInterface();
                    GameDriver.updateSetupInterface();
                    if (getPhase() == GamePhase.setup && p1Setup == false && p2Setup == false) GameDriver.endSetup();
                    break;
                case Turn.pause:
                    print("Game paused, can't pass turn now!");
                    break;
            }
        }
    }

    public static int currentTurn()
    {
        if (turnState == Turn.player1) return 1;
        else if (turnState == Turn.player2) return 2;
        else if (turnState == Turn.pause && holdTurn == Turn.player1) return 1;
        else if (turnState == Turn.pause && holdTurn == Turn.player2) return 2;
        else
        {
            print("Asked for current turn, but it broke!");
            return 0;
        }
    }

    public static void setupPhase()
    {
        if (state == GameState.active)
        {
            gamePhase = GamePhase.setup;
            p1Setup = true;
            p2Setup = true;
        }
    }

    public static void battlePhase()
    {
        if (state == GameState.active)
        {
            gamePhase = GamePhase.battle;
        }
    }

	public static void gameOverPhase()
	{
		if (state == GameState.active) 
		{
			gamePhase = GamePhase.gameOver;
		}
	}

    public static void pause()
    {
        if (turnState != Turn.pause)
        {
            holdTurn = turnState;
            turnState = Turn.pause;
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            GameDriver.showMenuInterface();
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
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            GameDriver.hideMenuInterface();
        }
        else
        {
            print("Game isn't paused, you can't unpause it!");
        }
    }

    public void unPauseFromButton()
    {
        StateMachine.unPause();
    }

    public static GamePhase getPhase()
    {
        return gamePhase;
    }

    public static GameState getGameState()
    {
        return state;
    }


    public static void endP1Setup()
    {
        p1Setup = false;
    }

    public static void endP2Setup()
    {
        p2Setup = false;
    }

    public static void p1KingPlaced()
    {
        p1King = true;
    }

    public static void p2KingPlaced()
    {
        p2King = true;
    }

    public static void endSetup()
    {
        GameDriver.startBattle();
    }

    public static bool isPlacingCube
    {
        get
        {
            return cubePlace;
        }
        set
        {
            cubePlace = value;
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
