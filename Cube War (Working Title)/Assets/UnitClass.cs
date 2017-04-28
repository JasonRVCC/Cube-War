using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass : MonoBehaviour
{

    public className uC;
    public classType uT;
    public float att;
    public float def;
    public int owningPlayer;
    public int pointCost;
    public Texture[] unitTexture;


    public void unitSetup(className cn, classType ct, float a, float d, int pl, int p, Texture[] tex)
    {
        unitTexture = new Texture[2];
        uC = cn;
        uT = ct;
        att = a;
        def = d;
        owningPlayer = pl;
        pointCost = p;
        texture[0] = tex[0];
        texture[1] = tex[1];
        if (owningPlayer == 1) TextureManager.applyTexture(this.gameObject, texture[0]);
        if (owningPlayer == 2) TextureManager.applyTexture(this.gameObject, texture[1]);
    }

    public void unitSetup(UnitClass copy)
    {
        unitTexture = new Texture[2];
        uC = copy.unitClass;
        uT = copy.unitType;
        att = copy.attack;
        def = copy.defense;
        owningPlayer = copy.owner;
        pointCost = copy.cost;
        texture[0] = copy.texture[0];
        texture[1] = copy.texture[1];
        if (owningPlayer == 1) TextureManager.applyTexture(this.gameObject, texture[0]);
        if (owningPlayer == 2) TextureManager.applyTexture(this.gameObject, texture[1]);
    }

    public className unitClass
    {
        get
        {
            return uC;
        }
        set
        {
            uC = value;
        }
    }

    public classType unitType
    {
        get
        {
            return uT;
        }
        set
        {
            uT = value;
        }
    }

    public float attack
    {
        get
        {
            return att;
        }
        set
        {
            att = value;
        }
    }

    public float defense
    {
        get
        {
            return def;
        }
        set
        {
            def = value;
        }
    }

    public int owner
    {
        get
        {
            return owningPlayer;
        }
        set
        {
            owningPlayer = value;
        }
    }

    public int cost
    {
        get
        {
            return pointCost;
        }
        set
        {
            pointCost = value;
        }
    }


    public Texture[] texture
    {
        get
        {
            return unitTexture;
        }
        set
        {
            unitTexture[0] = value[0];
            unitTexture[1] = value[1];
        }
    }

    public void startAttack()
    {
		this.gameObject.GetComponent<Rigidbody> ().mass= 2.0f + (attack * 2);
    }

    public void startDefense()
    {
		this.gameObject.GetComponent<Rigidbody> ().mass = def;
    }
}