using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BlockObject;
using static Unity.Collections.AllocatorManager;

public class Board : MonoBehaviour
{
    Dictionary<(int, int), PlaceHolder> blockHolders = new Dictionary<(int, int), PlaceHolder>();
    List<List<BlockObject>> objectList = new List<List<BlockObject>>();
	//[SerializeField] List<int[]> defaultColors = new List<int[]>();
	[SerializeField] List<PlaceHolderRow> defaultSetup;
	[SerializeField] Dictionary<(int,int), PlaceHolder> blocks2Clear = new Dictionary<(int, int), PlaceHolder>();
	[SerializeField] List<PlaceHolder> blocks2Fill;
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

			var block = ObjectManager.Instance.GetBlock(true, defaultSetup[x].GetColor(holder.Row));
            holder.SetBlock(block); 
            block.gameObject.SetActive(true);
            objectList[x][holder.Row] = block;

            Debug.Log($"{holder.Col}, {holder.Row} = {defaultSetup[x].GetColor(holder.Row)}");
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (blocks2Clear.Count > 0)
        {
            if (movingBlocks == 0)
			{
				Debug.Break();
				blocks2Clear.Clear();

				//SwapBlock(newTile, selectedBlock);
                foreach(var elem in blockHolders.Values)
                {
					bool con1 = Board.Instance.CheckMatch4(elem);
					if (con1)
					{
						//if(con1) Board.Instance.CheckMatch4(newTile);
						//if(con2) Board.Instance.CheckMatch4(selectedBlock);
						///Board.Instance.SwapBlock(newTile, selectedBlock);
						MyLogger.Log("CheckMatch4");
					}
					else if (Board.Instance.CheckMatch3(elem))
					{
						MyLogger.Log("CheckMatch3 CheckMatch3");
						//Board.Instance.CheckMatch4(newTile);
						//Board.Instance.CheckMatch4(selectedBlock);
						///Board.Instance.SwapBlock(newTile, selectedBlock);
					}
				}

                ClearBlock();
			}
        }
	}

    /*public bool IsMatchAfterSwap(PlaceHolder a, PlaceHolder b)
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
	}*/

    // check for all 3 possible combinations
    public bool CheckMatch3(PlaceHolder block)
	{
		// same row
		bool a = CheckMatch3Internal(block.Col-1, block.Row) | CheckMatch3Internal(block.Col+1, block.Row);
		// same col
		bool b = CheckMatch3Internal(block.Col, block.Row - 1) | CheckMatch3Internal(block.Col, block.Row + 1);
		// two more cases
		bool c = CheckMatch3Internal(block.Col-1, block.Row-1) | CheckMatch3Internal(block.Col+1, block.Row+1);

        return a | b | c;
    }

    bool CheckMatch3Internal(int x, int y)
	{
		if (!blockHolders.ContainsKey((x, y)) || blocks2Clear.ContainsKey((x,y)) )
		{
			return false;
		}

		bool a = CheckLine(x, y - 1, x, y, x, y + 1); // vertical
		if (x == 0) // edge case where slope is not consistent
        {
            bool bb    = CheckLine(x - 1, y-1, x, y, x + 1, y); // horizontal
			bool cc    = CheckLine(x - 1, y, x, y, x + 1, y-1); // diagonal

			return a | bb | cc;
		}
        else
        {
			bool b = CheckLine(x - 1, y, x, y, x + 1, y); // horizontal
			bool c = CheckLine(x - 1, y - 1, x, y, x + 1, y + 1); // diagonal
			return a | b | c;
		}
	}

    bool CheckLine( int sx, int sy, int mx, int my, int ex, int ey)
	{
        if (sx == 0)
        {
            // 
        }

		if (!blockHolders.ContainsKey((sx, sy)))
        {
            return false;
		}
		List<PlaceHolder> clears = new List<PlaceHolder>();
        clears.Add(blockHolders[(sx, sy)]);

		BlockColor prevColor = blockHolders[(sx,sy)].Block.Color;

		if (!blockHolders.ContainsKey((mx, my)) || prevColor != blockHolders[(mx, my)].Block.Color)
		{
            return false;
        }
		clears.Add(blockHolders[(mx, my)]);

		if (!blockHolders.ContainsKey((ex, ey)) || prevColor != blockHolders[(ex, ey)].Block.Color)
		{
			return false;
		}
		clears.Add(blockHolders[(ex, ey)]);

		foreach (var elem in clears)
		{
            MyLogger.Log($"Adding line combo ({elem.Col}, {elem.Row}) to clear");
			blocks2Clear[(elem.Col, elem.Row)] = elem;
		}

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

			a.SetBlock(b.Block);
            b.SetBlock(tmp);
		}
        return true;
    }

    public void ClearBlock()
	{
		//Debug.Break();
		if (blocks2Clear.Count == 0)
        {
            return;
        }

        foreach (var block in blocks2Clear.Values)
        {
            block.ClearBlock();
        }

        int newBlocksSize = blocks2Clear.Count;
        var blocksSorted = new List<PlaceHolder>();

        int row = 0;
        int prevCol = -3;
        foreach(var block in blocks2Clear.Values.OrderBy(x => x.Col))
        {
            if (prevCol != block.Col)
                row = 0;

			prevCol = block.Col;
			var blockTop = blockHolders[(block.Col, row)];
            blocksSorted.Add(blockTop);
            row++;
		}

		//blocks2Clear.Clear();

        FallLogic();
        var blockSorted2 = blocksSorted.OrderByDescending(x => Mathf.Abs(x.Col) + x.Row).ToList();
		PopuplateBlocks(blockSorted2);
		Debug.Break();
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
				var copyBlock = block.Block;
				if (blockHolders.ContainsKey((block.Col, block.Row + fallLength)))
                {
					var destination = blockHolders[(block.Col, block.Row + fallLength)];
					var diff = (destination.transform.localPosition - block.transform.localPosition);

					// blocks2Clear[(block.Col, block.Row)] = block;

					movingBlocks++;
					// blockHolders[(block.Col, block.Row)] = null;
					block.Block.transform.DOMoveY(destination.transform.position.y, fallLength / 2.0f).OnComplete(() =>
					{
						blockHolders[(block.Col, block.Row + fallLength)].SetBlock(copyBlock, true);
						movingBlocks--;
						MyLogger.Log($"Moving Finished moved {fallLength}, and {block.Col},{block.Row + fallLength} now has {copyBlock.Color} color");
						//MyLogger.Log($"Moving Finished, moved {fallLength} unit there are still {movingBlocks} blocks moving");
					});
				}

                else
                {
					MyLogger.Log($"Moving Failed to move {block.Col},{block.Row+fallLength} block");
				}
			}

            }

    }

    public void PopuplateBlocks(List<PlaceHolder> list2Fill)
    {
        var block00 = blockHolders[(0, 0)];

        float interval = 0f;
        foreach (var destination in list2Fill)
        {
            interval += 0.5f;
			var block = ObjectManager.Instance.GetBlock(false);
            block.transform.position = positionReplenish.transform.position;
            movingBlocks++;

            int distanceX = Mathf.Abs(destination.Col)+1;
            int distanceY = Mathf.Abs(destination.Row)+1;
            var destinationCol = blockHolders[(destination.Col, 0)];
            // float finalInterval = interval + distance * 0.5f;
            var sequence = DOTween.Sequence();

			sequence.Append(block.transform.DOMove(positionReplenish.transform.position, 0.3f).SetDelay(interval));
            sequence.AppendCallback( () => { block.gameObject.SetActive(true); } );
            sequence.Append(block.transform.DOMove(block00.transform.position, 0.3f));
            sequence.Append(block.transform.DOMove(destinationCol.transform.position, distanceX * 0.5f));
            sequence.Append(block.transform.DOMove(destination.transform.position, distanceY * 0.5f));
            sequence.AppendCallback(() =>
            {
                movingBlocks--;
                MyLogger.Log($"New Blocks Moving Finished, moved {0} unit there are still {movingBlocks} blocks moving");

                blockHolders[(destination.Col, destination.Row)].SetBlock(block, true);
            }
            );

			/*block.transform.DOMove(block00.transform.position, 0.3f).SetDelay(interval).OnComplete(() =>
            {
				

				block.transform.DOMove(destinationCol.transform.position, distanceX * 0.5f).OnComplete(() =>
                {
                    block.transform.DOMove(destination.transform.position, distanceY * 0.5f).OnComplete(() =>
                    {
                        movingBlocks--;
                        MyLogger.Log($"New Blocks Moving Finished, moved {0} unit there are still {movingBlocks} blocks moving");

                        blockHolders[(destination.Col, destination.Row)].SetBlock(block, true);
                    });
                });
            });*/
        }
	}
}
