using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

	}

	[SerializeField] float thresholdMove;

	private bool isDragging = false;
	private bool isMoving = false;
	private PlaceHolder selectedBlock;
	Vector3 selectedPosition;
	private float time = 0;

	// [SerializeField] GameData gameData; // uses scriptable object for customized settings

	void Update()
    {
		time += Time.deltaTime;
		// UpdateTime(time);

		if (Board.Instance.IsMoving()) // prevent interruption
		{
			return;
		}

		if (isDragging)
		{
			// dragging finished
			//if (movedDistance > thresholdMove)
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
			if (hit.collider == null)   // corresponding block is not active
			{
				return;
			}
			var newTile = hit.collider.GetComponent<PlaceHolder>();
			if (newTile != selectedBlock)
			{
				MyLogger.Log($"Detected next block {newTile.Block.name} ({newTile.Col},{newTile.Row})");
				// Do switch animation
				// if there is working match, do next logic
				// if not, move the blocks to their original positions.

				// do check swap

				bool check = Board.Instance.CheckSwapBlock(newTile, selectedBlock);
				if (!check)
				{ 
					int j = 0;
				}
					
				selectedBlock = null;
				isDragging = false;
				isMoving = true; // clear after one second
				MyLogger.Log("Stopped Dragging");
			}
		}
		else
		{
			// dragging started
			if (Input.GetMouseButtonDown(0))
			{
				MyLogger.Log("Started Dragging");

				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);
				if (hit.collider == null)   // corresponding block is not active
				{
					return;
				}
				var newTile = hit.collider.GetComponent<PlaceHolder>();
				if (newTile.Block != null)
				{
					MyLogger.Log($"Detected first block {newTile.Block.name} ({newTile.Col},{newTile.Row})");
					selectedBlock = newTile;// PlaceHolder.Block;
					selectedPosition = Input.mousePosition;

					isDragging = true;
				}
			}
		}
	}
}
