using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BlockObject;

public class ObjectManager : MonoBehaviour
{
    Stack<BlockObject> blockObjectPool;
    [SerializeField] int sizePool;
	[SerializeField] BlockObject templatedBlock;

	public static ObjectManager Instance
	{ get; private set; }

	void Awake()
    {
		Instance = this;

		blockObjectPool = new Stack<BlockObject>();

        for (int i = 0; i < sizePool; i++)
        {
            BlockObject newBlock = GameObject.Instantiate(templatedBlock);
			newBlock.Setup(BlockObject.BlockColor.Orange);
			newBlock.transform.SetParent (this.transform);
			newBlock.transform.localScale = Vector3.one;
			newBlock.transform.localPosition = Vector3.zero;
			blockObjectPool.Push(newBlock);
		}
	}

	public BlockObject GetBlock(/*BlockType type = BlockType.Normal, */bool showActive = true, BlockColor color = BlockColor.None)
	{
		if (color == BlockColor.None)
		{
			var enumValues = System.Enum.GetValues(enumType: typeof(BlockColor));
			color = (BlockColor) enumValues.GetValue(Random.Range(0, enumValues.Length-1));
		}

		var block = blockObjectPool.Pop();

		block.gameObject.SetActive(showActive);
		block.Setup(color);

		return block;
	}

	public void ReturnBlock(BlockObject used)
	{
		used.transform.SetParent(this.transform);
		used.transform.localPosition = Vector3.zero;
		used.gameObject.SetActive(false);
		blockObjectPool.Push(used);
	}
}
