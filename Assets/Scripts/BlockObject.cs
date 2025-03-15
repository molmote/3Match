using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour
{
	[SerializeField] Sprite[] sprites;
	[SerializeField] SpriteRenderer spRenderer;
	public enum BlockType
    {
        Normal
    }

	public enum BlockColor
	{
		Blue = 0,
        Green,
        Red,
        Yellow,
        Purple,
        Orange,
        None
    }

	[SerializeField] BlockColor color;
	[SerializeField] BlockType type;

    public BlockColor Color { 
        get { return color; }
    }

    public Color Alpha
    {
		get { return spRenderer.color; }
		set { spRenderer.color = value; }
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool Check(BlockObject a, BlockObject b)
    {
        return a.Color == b.Color;
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }

    public void Setup(//BlockType type,
                      BlockColor color)
    {
        // this.type = type;
        this.color = color;

        /*int spriteName = color switch
        {
            BlockColor.Blue => "blue_normal",
            BlockColor.Green => "green_normal",
            BlockColor.Red => "red_normal",
			BlockColor.Yellow => "yellow_normal",
			BlockColor.Purple => "purple_normal"
			BlockColor.Orange => "orange_normal",

        };*/

		spRenderer.sprite = sprites[(int)color];
	}
}
