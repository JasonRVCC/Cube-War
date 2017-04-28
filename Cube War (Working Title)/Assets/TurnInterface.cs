using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnInterface : MonoBehaviour {
    public Image turnImage;
    public Sprite p1Sprite;
    public Sprite p2Sprite;

    public void Awake()
    {
        turnImage = GetComponent<Image>();
    }

    public void updateTurnInterface()
    {
        if (StateMachine.getGameState() == GameState.active)
        {
            
            if (StateMachine.currentTurn() == 1)
            {
                TextureManager.applySprite(this.gameObject, p1Sprite);

            }
            else if (StateMachine.currentTurn() == 2)
            {
                TextureManager.applySprite(this.gameObject, p2Sprite);
            }
        }
    }
}
