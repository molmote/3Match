using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolder : MonoBehaviour
{
    [SerializeField] int col;
    [SerializeField] int row;
    [SerializeField] BlockObject currentBlock;

	public BlockObject Block
	{
		get { return currentBlock; }
	}

	enum HolderState
    {
        Empty,
        Moving,
        Filled
    }

    [SerializeField] HolderState state;
	public bool visited;

	public int Col
    {
        get
        {
            return col;
        }
    }

    public int Row
    {
        get
        {
            return row;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        visited = false;
	}

    public void SetBlock(BlockObject block)
    {
        currentBlock = block;
        block.transform.SetParent(transform);
        block.transform.localPosition = Vector3.zero;
    }
        
}
