using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupInterface : MonoBehaviour {
    public string targetClass = "King"; // This is what you will set to change what class you get from the button
    public UnitClass unitClass;
    public GameObject cubePrefab;
    public GameObject newUnit;
    public GameDriver driver;
    public ClassLookup classInfo;

    public void Start()
    {
        classInfo = ClassLookup.getClassLookup();
        driver = GameDriver.getGameDriverRef();
        textureButtons(1);
    }



    public void instantiateNewUnit()
    {
        if (StateMachine.turnState != Turn.pause)
        {
			classInfo.Lookup (targetClass);
			print (classInfo.cost);
            if (!StateMachine.isPlacingCube && driver.getPlayerPointsRemaining() > classInfo.cost)
            {
				newUnit = Instantiate(cubePrefab, new Vector3(0,2,0), Quaternion.identity) as GameObject;
				newUnit.GetComponent<Cube> ().SetToPlacing();
				newUnit.GetComponent<UnitClass>().unitSetup(classInfo.Lookup(targetClass, newUnit.GetComponent<UnitClass>()));
                driver.placingCube(newUnit);
            }
        }
    }

    public void instantiateNewUnit(string target)      //An overload in case the interface calls it this way
    {
        if (StateMachine.turnState != Turn.pause)
        {
            classInfo.Lookup(target);
            if (!StateMachine.isPlacingCube && driver.getPlayerPointsRemaining() >= classInfo.cost)
            {
                if (target == "King")
                {
                    if ((StateMachine.currentTurn() == 1 && StateMachine.p1King == true) || (StateMachine.currentTurn() == 2 && StateMachine.p2King == true))
                    {
                        return;
                    }
                }
                newUnit = Instantiate(cubePrefab) as GameObject;
                newUnit.GetComponent<Cube>().SetToPlacing();
                newUnit.GetComponent<UnitClass>().unitSetup(classInfo.Lookup(target, newUnit.GetComponent<UnitClass>()));
                driver.placingCube(newUnit);
            }
        }
    }

    public void endPlayerSetup()
    {
        if (StateMachine.turnState != Turn.pause)
        {
            if (StateMachine.isPlacingCube == false)
            {
                if (StateMachine.currentTurn() == 1 && StateMachine.p1King == true)
                {
                    StateMachine.endP1Setup();
                    StateMachine.passTurn();
                }
                else if (StateMachine.currentTurn() == 2 && StateMachine.p2King == true)
                {
                    StateMachine.endP2Setup();
                    StateMachine.passTurn();
                }
            }
        }
    }


    public void textureButtons(int i)
    {
        foreach (Transform t in transform)
        {
            switch (t.name)
            {
                case "King":
                    classInfo.Lookup("King");
                    break;
                case "Brawler":
                    classInfo.Lookup("Brawler");
                    break;
                case "Sentinel":
                    classInfo.Lookup("Sentinel");
                    break;
                case "Shadow":
                    classInfo.Lookup("Shadow");
                    break;
                case "Grunt":
                    classInfo.Lookup("Grunt");
                    break;
                case "Peasant":
                    classInfo.Lookup("Peasant");
                    break;
                case "Healer":
                    classInfo.Lookup("Healer");
                    break;
                case "Paralyze":
                    classInfo.Lookup("Paralyze");
                    break;
                case "Titan":
                    classInfo.Lookup("Titan");
                    break;
                default:
                    classInfo.Lookup("blank");
                    break;
            }
            if (classInfo.cName != className.Null)
            {
                TextureManager.applySprite(t.gameObject, classInfo.texture[i - 1]);
            }
        }
    }
}
