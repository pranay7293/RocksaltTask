using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public ElementType elementType;
    public int xIndex;
    public int yIndex;
    public bool isMatched;

    private Vector2 initialPos;


    private ElementsBoard board;

    public Element(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
   

    private void Start()
    {
        initialPos = transform.position;
        board = ElementsBoard.instance;
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked ");
        if (!isMatched)
        {
            board.SelectedElement(this);
            Debug.Log("Processed ");

        }
    }

    public void SetIndicies(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
}

public enum ElementType
{
    Red, 
    Green, 
    Blue,
    Orange,
    Yellow    
}
