using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static BlockObject;
using static Unity.Collections.AllocatorManager;

public class Board : MonoBehaviour
{
    Dictionary<(int, int), PlaceHolder> blockHolders = new Dictionary<(int, int), PlaceHolder>();
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
		holders = GetComponentsInChildren<PlaceHolder>();
		RestartDefault();
	}

	PlaceHolder[] holders;

	public void ClearAll()
	{
		foreach (var holder in blockHolders.Values)
		{
			holder.ClearBlock();
		}
	}

	public void RestartDefault()
	{
		MyLogger.Log("RestartDefault");
		ClearAll();

		foreach (var holder in holders)
		{
			if (blockHolders.ContainsKey((holder.Col, holder.Row)))
			{
				MyLogger.Log($"{holder.Col}, {holder.Row} = duplicated");
			}
			blockHolders[(holder.Col, holder.Row)] = holder;

			int x = holder.Col + 3;

			var block = ObjectManager.Instance.GetBlock(true, defaultSetup[x].GetColor(holder.Row));
			holder.SetBlock(block);
			block.gameObject.SetActive(true);

			MyLogger.Log($"{holder.Col}, {holder.Row} = {defaultSetup[x].GetColor(holder.Row)}");
		}
	}

	public void ResetRandom()
	{
		MyLogger.Log("ResetRandom");
		ClearAll();

		foreach (var holder in holders)
		{
			if (blockHolders.ContainsKey((holder.Col, holder.Row)))
			{
				MyLogger.Log($"{holder.Col}, {holder.Row} = duplicated");
			}
			blockHolders[(holder.Col, holder.Row)] = holder;

			int x = holder.Col + 3;

			var block = ObjectManager.Instance.GetBlock(true, BlockColor.None);
			holder.SetBlock(block);
			block.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update()
    {
        if (blocks2Clear.Count > 0)
        {
            if (movingBlocks == 0)
			{
				// Debug.Break();
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
					if (Board.Instance.CheckMatch3(elem))
					{
						MyLogger.Log("CheckMatch3");
						//Board.Instance.CheckMatch4(newTile);
						//Board.Instance.CheckMatch4(selectedBlock);
						///Board.Instance.SwapBlock(newTile, selectedBlock);
					}
				}

                ClearBlock();
			}
        }
	}

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

    public bool CheckSwapBlock(PlaceHolder a, PlaceHolder b)
    {
        SwapBlock(a, b);
		bool con1 = Board.Instance.CheckMatch4(a);
		bool con2 = Board.Instance.CheckMatch4(b);
		bool con3 = Board.Instance.CheckMatch3(a);
		bool con4 = Board.Instance.CheckMatch3(b);

        bool condition = con1 || con2 || con3 || con4;

        var seq = DOTween.Sequence();
		seq.Append(a.Block.transform.DOMove(b.Block.transform.position, 0.2f));
        seq.Join(b.Block.transform.DOMove(a.Block.transform.position, 0.2f));
		seq.AppendCallback( () =>
		{
			if (!condition)
			{
				// do moving and revert animation
				a.Block.transform.DOMove(b.Block.transform.position, 0.2f);
				b.Block.transform.DOMove(a.Block.transform.position, 0.2f);

				SwapBlock(a, b);
			}
            else
            {
				Board.Instance.ClearBlock();
			}
		}
        );

        return condition;
	}


    public bool SwapBlock(PlaceHolder a, PlaceHolder b)
    {
        if (a != b)
        {
            BlockObject tmp = a.Block;

            a.SetBlock(b.Block, false); ;
            b.SetBlock(tmp, false);
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
			/*var saved = block.Block;
			DOTween.ToAlpha(() => saved.Alpha, x => saved.Alpha = x, 0, 1).OnComplete(() =>
			{ 
				saved.gameObject.SetActive(false);
			});*/

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

        FallLogic(newBlocksSize);
        var blockSorted2 = blocksSorted.OrderByDescending(x => Mathf.Abs(x.Col) + x.Row).ToList();
		PopuplateBlocks(blockSorted2);
		// Debug.Break();
	}

    public enum BoardState
    {
        Play,
        Fall,
    }

    private int movingBlocks = 0;
    public bool IsMoving()
    {
        return movingBlocks > 0 || blocks2Clear.Count > 0;
	}

    public void FallLogic(int destroySize)
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

					movingBlocks++;
					var seq = DOTween.Sequence();
					block.state = PlaceHolder.HolderState.Moving;
					seq.Append(block.Block.transform.DOMoveY(destination.transform.position.y, fallLength / 2.0f));

					seq.AppendCallback(() =>
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

		// horizontally move then.. 실제 Toy party에서처럼 좌측으로 빈 공간이 있을 때 처리하는 로직을 구현하려 시도.
		/*foreach (var block in blockHolders.Values)
		{
			int slideLength = 0;
			if (block.state != PlaceHolder.HolderState.Empty)
			{
				continue;
			}

			slideLength++;
			}

		}*/

	}

    public void PopuplateBlocks(List<PlaceHolder> list2Fill)
    {
        var block00 = blockHolders[(0, 0)];

        float interval = -0.8f;
        foreach (var destination in list2Fill)
        {
            interval += 0.8f;
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
			if (destination.Col != 0)
				sequence.Append(block.transform.DOMove(destinationCol.transform.position, distanceX * 0.5f));
			// if (destination.Row != 0)
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
