using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class PlaceHolder : MonoBehaviour
{
    [SerializeField] int col;
    [SerializeField] int row;
    [SerializeField] BlockObject currentBlock;

	public BlockObject Block
	{
		get { return currentBlock; }
	}

	public enum HolderState
    {
        Empty,
        Moving,
        Filled
    }

    public HolderState state;
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

    public void SetBlock(BlockObject block, bool changeParent = true)
    {
        currentBlock = block;
        if(changeParent)
        {
			block.transform.SetParent(transform);
			block.transform.localPosition = Vector3.zero;
		}			
    }

	public void ClearBlock()
	{
        currentBlock.Clear();

		ObjectManager.Instance.ReturnBlock(currentBlock);

		currentBlock = null;
	}
}
