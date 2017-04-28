using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextureManager : MonoBehaviour {

    public static void applyTexture(GameObject obj, Texture tex)
    {
        if (tex != null)
            obj.GetComponent<Renderer>().material.SetTexture(Shader.PropertyToID("_MainTex"), tex);
        else
            print("Can't apply texture, the texture you passed in is null!");
	}



    
    public static void applySprite(GameObject obj, Texture tex)
    {
        if (tex != null)
        {
            obj.GetComponent<Image>().sprite = Sprite.Create((Texture2D)tex, new Rect(new Vector2(0, 0), new Vector2(tex.width, tex.height)), new Vector2(0.5f, 0.5f));
        }
        else
            print("Can't apply sprite, the texture you passed in is null!");
    }

    public static void applySprite(GameObject obj, Sprite tex)
    {
        if (tex != null)
        {
            obj.GetComponent<Image>().sprite = tex;
        }
        else
            print("Can't apply sprite, the texture you passed in is null!");
    }
}
