using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Element : MonoBehaviour
{
    public ElementType elementType;
    public int xIndex;
    public int yIndex;
    public bool isMatched;

    public int scoreValue;

    private ElementsBoard board;

    public Element(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
   

    private void Start()
    {
        board = ElementsBoard.instance;
    }

    private void OnMouseDown()
    {
        if (!isMatched)
        {
            board.SelectedElement(this);

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
