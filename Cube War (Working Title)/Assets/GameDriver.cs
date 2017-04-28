using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDriver : MonoBehaviour {
    public static GameDriver gameDriver;
    public Player p1;
    public Player p2;
    public int playerPointsTesting = 50; // Change how this is done later once we know how to determine this.
    public bool checkingCubeMovement = false;
    public GameObject cubeSelected;
    public List<GameObject> cubesInPlay;
    public List<GameObject> menuInterfaceObjects;
    public List<GameObject> setupInterfaceObjects;
    public List<GameObject> gameOverInterfaceObjects;
    public List<GameObject> turnInterfaceObjects;
    public List<GameObject> pointInterfaceObjects;
    public GameObject pointSlider1;
    public GameObject pointSlider2;
    public GameObject pointText1;
    public GameObject pointText2;
    public List<GameObject> hoverInfoInterfaceObjects;
    public GameObject setupInterfaceHide;
    public GameObject pointInterfaceHide;
    public GameObject hoverInfoInterfaceHide;
    public bool menuVis = false;
    public bool setupVis = true;
    public bool gameOverVis = false;
    public bool turnVis = true;
    public bool pointVis = true;
    public bool hoverInfoVis = false;
    public bool hoverInfoVisLock = false;



    public void Awake()
    {
        gameDriver = this;
        beginGameSetup();
    }

    public void FixedUpdate()
    {
        if (checkingCubeMovement)
        {
			if (checkCubeMovement ()) {
				gameDriver.checkingCubeMovement = false;
				StateMachine.passTurn ();
			}
        }
    }

    public void beginGameSetup()
    {
        p1 = new Player(1, playerPointsTesting);
        p2 = new Player(2, playerPointsTesting);
        StateMachine.activate();
        pointSlider1.GetComponent<Slider>().maxValue = p1.pointsAvailable;
        pointSlider2.GetComponent<Slider>().maxValue = p2.pointsAvailable;
        foreach(GameObject obj in hoverInfoInterfaceObjects)
        {
            obj.SetActive(true);
            if (obj.GetComponent<HoverInfoInterface>() != null) obj.GetComponent<HoverInfoInterface>().Start();
        }
        
        StateMachine.setupPhase();
        foreach (GameObject obj in hoverInfoInterfaceObjects)
        {
            obj.SetActive(false);
        }
        StateMachine.initiateTurns();
        updatePointInterface();
    }

    public int addPlayerPoints(int player, int points)
    {
        switch (player)
        {
            case 1:
                p1.currentPoints += points;
                return p1.currentPoints;
            case 2:
                p2.currentPoints += points;
                return p2.currentPoints;
            default:
                return -1;
        }
    }

    public int getPlayerPoints()
    {
        if (StateMachine.currentTurn() == 1)
        {
            return p1.points;
        }
        else if (StateMachine.currentTurn() == 2)
        {
            return p2.points;
        }
        else
        {
            print("Tried to get player's points, it didn't work.");
            return -1;
        }
    }

    public int getMaxPlayerPoints()
    {
        if (StateMachine.currentTurn() == 1)
        {
            return p1.pointsAvailable;
        }
        else if (StateMachine.currentTurn() == 2)
        {
            return p2.pointsAvailable;
        }
        else
        {
            print("Tried to get player's points, it didn't work.");
            return -1;
        }
    }

    public int getPlayerPointsRemaining()
    {
        if (StateMachine.currentTurn() == 1)
        {
            return (p1.pointsAvailable - p1.points);
        }
        else if (StateMachine.currentTurn() == 2)
        {
            return (p2.pointsAvailable - p2.points);
        }
        else
        {
            print("Tried to get player's points, it didn't work.");
            return -1;
        }
    }

    public void placingCube(GameObject toPlace)
    {
        cubeSelected = toPlace;
        cubeSelected.GetComponent<Cube>().SetToPlacing();
        StateMachine.isPlacingCube = true;

    }

    public static void activateTurnInterface()
    {
        foreach (GameObject obj in gameDriver.turnInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }

    public static void endSetup()
    {
        GameDriver.hidePointHider();
        GameDriver.hideSetupHider();
        GameDriver.hideSetupInterface();
        GameDriver.hidePointInterface();
        GameDriver.startBattle();
    }

    public static void startBattle()
    {
        StateMachine.battlePhase();
        StateMachine.initiateTurns();
    }

    public void startGameOver(int winner)
    {
		StateMachine.gameOverPhase();
        showGameOverInterface();
        foreach (GameObject obj in gameOverInterfaceObjects)
        {
            if (obj.name.Equals("GameOverWinner"))
            {
                obj.GetComponent<Text>().text = ("Player " + winner + " wins!");
            }
        }
		print ("Game Over! Player " + StateMachine.currentTurn () + " Wins!");
    }








    //////////////////Static stuff for calling easily from outside//////////////////////////////////


    //
    //Call this from the cube when it is placed
    //
    public static void placedCube()
    {
        if (gameDriver.addPlayerPoints(StateMachine.currentTurn(), gameDriver.cubeSelected.GetComponent<UnitClass>().cost) == -1)
            print("Something went wrong with the player point counts!");
        gameDriver.cubesInPlay.Add(gameDriver.cubeSelected);
        if (gameDriver.cubeSelected.GetComponent<UnitClass>().unitClass == className.King)
        {
            if (StateMachine.currentTurn() == 1) StateMachine.p1KingPlaced();
            else if (StateMachine.currentTurn() == 2) StateMachine.p2KingPlaced();
            else print("That king was placed on no particular player's turn!");
        }
        gameDriver.cubeSelected = null;
        StateMachine.isPlacingCube = false;
        GameDriver.updatePointInterface();
        StateMachine.passTurn();
    }




    //
    //Call this from the cube when placement is cancelled
    //
    public static void cancelPlaceCube()
    {
        GameObject.Destroy(gameDriver.cubeSelected);
        gameDriver.cubeSelected = null;
        StateMachine.isPlacingCube = false;
    }

    //
    //Call this on turn change to clear the flick count of cubes for the next turn
    //
    public static void clearFlicks()
    {
        foreach (GameObject c in gameDriver.cubesInPlay)
        {
            if (StateMachine.getPhase() == GamePhase.battle)
            {
                if (c.GetComponent<Cube>().stunned == false)
                {
                    c.GetComponent<Cube>().flicked = false;
                }
                else
                {
                    c.GetComponent<Cube>().stunned = false;
                }
            } //Currently commented out because I don't have the cubes in this project. Uncomment this when they're integrated.
        }
    }

    public static bool checkCubeMovement()
    {
        gameDriver.checkingCubeMovement = true;
        bool allStopped = true;
        foreach (GameObject c in gameDriver.cubesInPlay)
        {
			if (!c.GetComponent<Rigidbody> ().IsSleeping ()) {
				allStopped = false;
			}
        }
        if (allStopped == true)
        {
            return true; //All are stopped, it can call for next turn.
        }
		return false; //just so it doesn't yell at me. Changing it later.
    }

    public static void removeCubeFromPlay(GameObject obj)
    {
        gameDriver.cubesInPlay.Remove(obj);
		if (StateMachine.gamePhase == GamePhase.setup) {
			if (obj.GetComponent<Cube> ().playState == PlayState.idle) {
				gameDriver.addPlayerPoints (obj.GetComponent<UnitClass>().owner, -1 * obj.GetComponent<UnitClass> ().cost);
			}
			StateMachine.isPlacingCube = false;
			if (StateMachine.currentTurn () != obj.GetComponent<UnitClass> ().owner) {
				StateMachine.passTurn ();
			}
			updatePointInterface ();
		}
		else if (obj.GetComponent<UnitClass>().unitClass.Equals(className.King))
        {
            if (obj.GetComponent<UnitClass>().owner == 1)
            {
                gameDriver.startGameOver(2);
            }
            else
            {
                gameDriver.startGameOver(1);
            }
        }
        GameObject.Destroy(obj);
    }







    /// <summary>
    /// /////////////////////////////////////Section for showing and hiding all the interface elements.
    /// </summary>

    public static void showMenuInterface()
    {
        gameDriver.menuVis = true;
        foreach (GameObject obj in gameDriver.menuInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hideMenuInterface()
    {
        gameDriver.menuVis = false;
        foreach (GameObject obj in gameDriver.menuInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showTurnInterface()
    {
        gameDriver.turnVis = true;
        foreach (GameObject obj in gameDriver.turnInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hideTurnInterface()
    {
        gameDriver.turnVis = false;
        foreach (GameObject obj in gameDriver.turnInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showSetupInterface()
    {
        gameDriver.setupVis = true;
        foreach (GameObject obj in gameDriver.setupInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hideSetupInterface()
    {
        gameDriver.setupVis = false;
        foreach (GameObject obj in gameDriver.setupInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showGameOverInterface()
    {
        gameDriver.gameOverVis = true;
        foreach (GameObject obj in gameDriver.gameOverInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hideGameOverInterface()
    {
        gameDriver.gameOverVis = false;
        foreach (GameObject obj in gameDriver.gameOverInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showPointInterface()
    {
        gameDriver.pointVis = true;
        foreach (GameObject obj in gameDriver.pointInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hidePointInterface()
    {
        gameDriver.pointVis = false;
        foreach (GameObject obj in gameDriver.pointInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showHoverInfoInterface()
    {
        if (gameDriver.hoverInfoVisLock != true)
        {
            gameDriver.hoverInfoVis = true;
            foreach (GameObject obj in gameDriver.hoverInfoInterfaceObjects)
            {
                obj.SetActive(true);
            }
        }
    }
    public static void hideHoverInfoInterface()
    {
        gameDriver.hoverInfoVis = false;
        foreach (GameObject obj in gameDriver.hoverInfoInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }
    

    public static void showHoverInfoInterface(bool l)
    {
        gameDriver.hoverInfoVis = true;
        gameDriver.hoverInfoVisLock = false;
        foreach (GameObject obj in gameDriver.hoverInfoInterfaceObjects)
        {
            obj.SetActive(true);
        }
    }
    public static void hideHoverInfoInterface(bool l)
    {
        gameDriver.hoverInfoVis = false;
        gameDriver.hoverInfoVisLock = true;
        foreach (GameObject obj in gameDriver.hoverInfoInterfaceObjects)
        {
            obj.SetActive(false);
        }
    }


    public static void showSetupHider()
    {
        gameDriver.setupInterfaceHide.SetActive(true);
    }
    public static void hideSetupHider()
    {
        gameDriver.setupInterfaceHide.SetActive(false);
    }


    public static void showPointHider()
    {
        gameDriver.pointInterfaceHide.SetActive(true);
    }
    public static void hidePointHider()
    {
        gameDriver.pointInterfaceHide.SetActive(false);
    }


    public static void showHoverInfoHider()
    {
        gameDriver.hoverInfoInterfaceHide.SetActive(true);
    }
    public static void hideHoverInfoHider()
    {
        gameDriver.hoverInfoInterfaceHide.SetActive(false);
    }



    public void toggleInterface(string n)
    {
        switch (n)
        {
            case "setup":
                if (setupVis == true) hideSetupInterface();
                else showSetupInterface();
                break;
            case "turn":
                if (turnVis == true) hideTurnInterface();
                else showTurnInterface();
                break;
            case "point":
                if (pointVis == true) hidePointInterface();
                else showPointInterface();
                break;
            case "gameOver":
                if (gameOverVis == true) hideGameOverInterface();
                else showGameOverInterface();
                break;
            case "menu":
                if (menuVis == true) hideMenuInterface();
                else showMenuInterface();
                break;
            case "hoverInfo":
                if (hoverInfoVis == true) hideHoverInfoInterface(true);
                else showHoverInfoInterface(false);
                break;
        }
    }

    public void showHoverInfoOnMouseover()
    {
        showHoverInfoInterface();
    }
    public void hideHoverInfoOnMouseout()
    {
        hideHoverInfoInterface();
    }


    /// <summary>
    /// /////////////////////////////////////////////////////////////End section for showing and hiding interface elements.
    /// </summary>






    public static void updateTurnInterface()
    {
        foreach (GameObject obj in gameDriver.turnInterfaceObjects)
        {
            if (obj.GetComponent<TurnInterface>() != null) obj.GetComponent<TurnInterface>().updateTurnInterface();
        }
    }

    public static void updatePointInterface()
    {
        int p1Points = gameDriver.p1.points;
        int p2Points = gameDriver.p2.points;
        gameDriver.pointSlider1.GetComponent<Slider>().value = p1Points;
        gameDriver.pointSlider2.GetComponent<Slider>().value = p2Points;
        gameDriver.pointText1.GetComponent<Text>().text = p1Points+"/"+gameDriver.p1.pointsAvailable;
        gameDriver.pointText2.GetComponent<Text>().text = p2Points + "/" + gameDriver.p2.pointsAvailable;
    }

    public static void updateSetupInterface()
    {
        foreach(GameObject obj in gameDriver.setupInterfaceObjects)
        {
            if (obj.GetComponent<SetupInterface>() != null) obj.GetComponent<SetupInterface>().textureButtons(StateMachine.currentTurn());
        }
    }


    //
    //Call this to get a reference to this script
    //
    public static GameDriver getGameDriverRef()
    {
        return gameDriver;
    }

    
}
