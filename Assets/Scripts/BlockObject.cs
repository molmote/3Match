using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject : MonoBehaviour
{
    [SerializeField] Sprite _sprite;
    enum BlockType
    {
        Normal
    }

    enum BlockColor
	{
		Blue = 0,
        Green,
        Red,
        Yellow,
        Purple,
        Orange
    }

	[SerializeField] BlockColor color;
	[SerializeField] BlockType type;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
