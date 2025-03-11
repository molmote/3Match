using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AdaptivePerformance;
using static BlockObject;
using static Unity.Collections.AllocatorManager;

public class Board : MonoBehaviour
{
    Dictionary<(int, int), PlaceHolder> blockHolders = new Dictionary<(int, int), PlaceHolder>();
    List<List<BlockObject>> objectList = new List<List<BlockObject>>();
	//[SerializeField] List<int[]> defaultColors = new List<int[]>();
	[SerializeField] List<PlaceHolderRow> defaultSetup;
	[SerializeField] Dictionary<(int,int), PlaceHolder> blocks2Clear = new Dictionary<(int, int), PlaceHolder>();
	[SerializeField] Transform positionReplenish;

	public static Board Instance
	{ get; private set; }

    void Awake()
    {
        Instance = this;
    }

	// Start is called before the first frame update
	void Start()
    {
        var holders = GetComponentsInChildren<PlaceHolder>();

        for (int i = 0; i < 7; i++) 
        { 
            List<BlockObject> col = new List<BlockObject>();
            for(int j = 0; j < 6; j++)
            {
                col.Add(null);
            }

            objectList.Add(col);
        }


		foreach (var holder in holders) 
        {
            if (blockHolders.ContainsKey((holder.Col, holder.Row)))
            {
				Debug.Log($"{holder.Col}, {holder.Row} = duplicated");
			}
            blockHolders[(holder.Col, holder.Row)] = holder;

            int x = holder.Col + 3;

			var block = ObjectManager.Instance.GetBlock(defaultSetup[x].GetColor(holder.Row));
            holder.SetBlock(block); 
            block.gameObject.SetActive(true);
            objectList[x][holder.Row] = block;

            Debug.Log($"{holder.Col}, {holder.Row} = {defaultSetup[x].GetColor(holder.Row)}");
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsMatchAfterSwap(PlaceHolder a, PlaceHolder b)
    {
        bool ret = FindMatch(a);
        ret &= FindMatch(b);

        return ret;
    }

    public bool FindMatch(PlaceHolder a)
    {
        int left = a.Col - 2 >= -3 ? a.Col : -3;
        int right = a.Col + 2 < 3 ? a.Col : 3;

        int top = a.Row - 2 >= 0 ? a.Col : 0;
        int bot = a.Row + 2 < 6 ? a.Col : 5;

        for(int i = left; i < right; i++)
        {
            for (int j = top; j < bot; j++)
			{
                var block = blockHolders[(i, j)];

			}
		}
		return true;
	}

    public bool CheckMatch3(PlaceHolder a)
    {
        return true;
    }

    public bool CheckMatch4(PlaceHolder a)
    {
        int x = a.Col;// + 3;
		int left = x- 1 >= -3 ? x-1 : -3;
		int right = x + 1 < 4 ? x+1 : 3;

		int top = a.Row - 1 >= 0 ? a.Row-1 : 0;
		int bot = a.Row + 1 < 5 ? a.Row+1 : 4;

        bool leftTop = CheckMatch4Internal(left, top);
        bool leftBot = CheckMatch4Internal(left, top+1);
		bool rightTop = CheckMatch4Internal(left+1, top);
		bool rightBot = CheckMatch4Internal(left+1, top+1);

		return leftTop || leftBot || rightTop || rightBot;
    }

    public bool CheckMatch4Internal(int left, int top)
	{
        List<PlaceHolder> clears = new List<PlaceHolder>();

		if (!blockHolders.ContainsKey((left, top)))
        {
            return false;
        }

        BlockColor prevColor = blockHolders[(left,top)].Block.Color;// objectList[left][top].Color;

		for (int i = left; i < left + 2; i++)
		{
			for (int j = top; j < top + 2; j++)
			{
                //if (i > 6 || i < 0 || j > )

                if (blockHolders.ContainsKey((i, j)))
                {
					if (blockHolders[(i, j)].Block.Color != prevColor)
						return false;

					clears.Add(blockHolders[(i, j)]);
				}
				else
                {
					return false;
				}
			}
        }

        foreach (var elem in clears)
        {
            blocks2Clear[(elem.Col, elem.Row)] = elem;
		}		

		return true;
	}

    public bool SwapBlock(PlaceHolder a, PlaceHolder b)
    {
        if (a != b)
        {
            BlockObject tmp = a.Block;

			//objectList[i][j] = b.Block;
			//objectList[i][j] = b.Block;

			a.SetBlock(b.Block);
            b.SetBlock(tmp);
		}
        return true;
    }

    public void ClearBlock()
    {
        foreach (var block in blocks2Clear.Values)
        {
            block.ClearBlock();
        }

        int newBlocksSize = blocks2Clear.Count;
		blocks2Clear.Clear();

        FallLogic();
		PopuplateBlocks(newBlocksSize);
	}

    public enum BoardState
    {
        Play,
        Fall,
    }

    private int movingBlocks = 0;
    public bool IsMoving()
    {
        return movingBlocks > 0;
	}

    public void FallLogic()
    {
        //List<BlockObject>

        foreach(var block in blockHolders.Values)
		{
			int fallLength = 0;
			for (int i = block.Row+1; i < 6; i++)
            {
                if (block.Block != null && 
                    blockHolders.ContainsKey((block.Col, i)) && 
                    blockHolders[(block.Col, i)].Block == null)
                {
                    fallLength++;
				}
			}            

            if (fallLength > 0)
            {
				if (blockHolders.ContainsKey((block.Col, block.Row + fallLength)))
                {
					var destination = blockHolders[(block.Col, block.Row + fallLength)];
					var diff = (destination.transform.localPosition - block.transform.localPosition);

					blocks2Clear[(block.Col, block.Row)] = block;

					movingBlocks++;
					blockHolders[(block.Col, block.Row + fallLength)].SetBlock(block.Block, true);
					blockHolders[(block.Col, block.Row)] = null;
					block.Block.transform.DOMoveY(destination.transform.position.y, fallLength / 2.0f).OnComplete(() =>
					{
						movingBlocks--;
						MyLogger.Log($"Moving Finished, moved {fallLength} unit there are still {movingBlocks} blocks moving");
					});
				}

                else
                {
					MyLogger.Log($"Moving Failed to move {block.Col},{block.Row+fallLength} block");
				}
			}

            }

    }

    public void PopuplateBlocks(int newBlocksSize)
    {
        var block00 = blockHolders[(0, 0)];

        foreach (var destination in blocks2Clear.Values)
        {
            var block = ObjectManager.Instance.GetBlock();

            block.transform.DOMoveY(block00.transform.position.y, 0.3f).OnComplete(() =>
            {
                block.transform.DOMove(destination.transform.position, 1.0f).OnComplete(() =>
                {
                    movingBlocks--;
                    MyLogger.Log($"New Blocks Moving Finished, moved {0} unit there are still {movingBlocks} blocks moving");

                    blockHolders[(destination.Col, destination.Row)].SetBlock(block, true);
                });
            });
        }

        blocks2Clear.Clear();

	}
}
