using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ElementsBoard : MonoBehaviour
{
    public int rows;
    public int cols;

    public float spacingX;
    public float spacingY;

    public Element[] elementsprefabs;

    public Cell[,] cellBoard;

    public GameObject elementsBoardSet;

    public ArrayLayout arrayLayout;
    public static ElementsBoard instance;

    private List<Element> selectedElements = new List<Element>();
    private bool isSwapping = false;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeBoard();
    }

    private void InitializeBoard()
    {
        cellBoard = new Cell[rows, cols];
        spacingX = (float)(rows - 1) / 2;
        spacingY = (float)((cols - 1) / 2) + 1;

        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                if (arrayLayout.rows[y].row[x])
                {
                    cellBoard[x, y] = new Cell(false, null);
                }
                else
                {
                    int randomeIndex = Random.Range(0, elementsprefabs.Length);

                    Element element = Instantiate(elementsprefabs[randomeIndex], position, Quaternion.identity);
                    element.SetIndicies(x, y);
                    cellBoard[x, y] = new Cell(true, element);
                }

            }
        }
        while (CheckBoard()) { }
    }


    public void SelectedElement(Element element)
    {
        if (isSwapping)
            return;

        if (!selectedElements.Contains(element))
        {
            if (selectedElements.Count == 0)
            {
                selectedElements.Add(element);
            }
            else if (selectedElements.Count == 1)
            {
                if (AreElementsAdjacent(selectedElements[0], element))
                {
                    selectedElements.Add(element);
                    // Attempt to swap the selected elements.
                    TrySwapElements(selectedElements[0], selectedElements[1]);
                }
            }
            else
            {
                selectedElements.Clear();
            }
        }

    }

    private void TrySwapElements(Element element1, Element element2)
    {
        // Swap the elements' positions.
        SwapElementsPosition(element1, element2);

        // Check for matches and handle them.
        bool hasMatched = CheckBoard();

        // If no match is found, swap the elements back.
        if (!hasMatched)
        {
            SwapElementsPosition(element1, element2);
        }
        else
        {
            // Clear the selected elements.
            selectedElements.Clear();
        }
    }


    private bool AreElementsAdjacent(Element element1, Element element2)
    {
        int dx = Mathf.Abs(element1.xIndex - element2.xIndex);
        int dy = Mathf.Abs(element1.yIndex - element2.yIndex);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }


    private void SwapElementsPosition(Element element1, Element element2)
    {
        // Swap the elements' positions in the elementsBoard array.
        Vector2Int tempIndices = new Vector2Int(element1.xIndex, element1.yIndex);

        cellBoard[element1.xIndex, element1.yIndex].element = element2;
        cellBoard[element2.xIndex, element2.yIndex].element = element1;

        // Swap the elements' indices.
        element1.SetIndicies(element2.xIndex, element2.yIndex);
        element2.SetIndicies(tempIndices.x, tempIndices.y);

        // Update the visual positions of the GameObjects.
        Vector3 tempPosition = element1.transform.position;

        element1.transform.position = element2.transform.position;
        element2.transform.position = tempPosition;
    }


    public bool CheckBoard()
    {
        Debug.Log("Checking Board");

        bool hasmatched = false;

        List<Element> elementsToRemove = new();

        do
        {
            // Clear the matched elements list for each iteration.
            elementsToRemove.Clear();
            for (int y = 0; y < cols; y++)
            {
                for (int x = 0; x < rows; x++)
                {
                    if (cellBoard[x, y].isUsable)
                    {
                        Element element = cellBoard[x, y].element;

                        if (!element.isMatched)
                        {
                            MatchResult matchedElements = IsConnected(element);
                            if (matchedElements.connectedElements.Count >= 3)
                            {
                                elementsToRemove.AddRange(matchedElements.connectedElements);

                                foreach (Element element1 in matchedElements.connectedElements)
                                {
                                    element1.isMatched = true;
                                }
                                hasmatched = true;
                            }
                        }
                    }
                }
            }
            if (hasmatched)
            {
                RemoveElements(elementsToRemove);
            }
        } while (hasmatched);

        return hasmatched;
    }
    public void RemoveElements(List<Element> elementsToRemove)
    {
        foreach (var elementToRemove in elementsToRemove)
        {
            int x = elementToRemove.xIndex;
            int y = elementToRemove.yIndex;

            if (cellBoard[x, y].element == elementToRemove)
            {
                Debug.Log($"old element type: {elementToRemove.name}");


                Debug.Log($"Randomizing element at ({x}, {y})");

                // Destroy the old element
                Destroy(cellBoard[x, y].element.gameObject);

                // Instantiate a new element with a random element type
                int randomIndex = Random.Range(0, elementsprefabs.Length);
                Element newElement = Instantiate(elementsprefabs[randomIndex], new Vector2(x - spacingX, y - spacingY), Quaternion.identity);
                newElement.SetIndicies(x, y);
                cellBoard[x, y].element = newElement;

                Debug.Log($"New element type: {newElement.name}");
            }
        }
    }




    private MatchResult IsConnected(Element element)
    {
        List<Element> connectedElements = new();
        ElementType elementType = element.elementType;

        connectedElements.Add(element);

        //check Right
        CheckDirection(element, new Vector2Int(1,0), connectedElements);
        //check Left
        CheckDirection(element, new Vector2Int(-1, 0), connectedElements);

        if(connectedElements.Count == 3) 
        { 
            Debug.Log(" Horizontal match 3 of Type: " + connectedElements[0].elementType);
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.Horizontal
            };
        }
        else if(connectedElements.Count > 3)
        {
            Debug.Log(" Long Horizontal match of Type: " + connectedElements[0].elementType);
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.LongHorizontal
            };
        }

        connectedElements.Clear();

        connectedElements.Add(element); 

        //check Up
        CheckDirection(element, new Vector2Int(0, 1), connectedElements);
        //check Down
        CheckDirection(element, new Vector2Int(0, -1), connectedElements);

        if (connectedElements.Count == 3)
        {
            Debug.Log(" Vertical match 3 of Type: " + connectedElements[0].elementType);
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.Vertical
            };
        }
        else if (connectedElements.Count > 3)
        {
            Debug.Log(" Long Vertical match of Type: " + connectedElements[0].elementType);
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.None
            };

        }

    }

    void CheckDirection(Element element, Vector2Int direction, List<Element> cconnectedElements)
    {
        ElementType elementType = element.elementType;
        int x = element.xIndex + direction.x; 
        int y = element.yIndex + direction.y;

        while(x >= 0 && x < rows && y >= 0 && y <cols )
        {
            if (cellBoard[x, y].isUsable)
            {
                Element neighbourElement = cellBoard[x, y].element;

                if (!neighbourElement.isMatched && neighbourElement.elementType == elementType)
                {
                    cconnectedElements.Add(neighbourElement);

                    x += direction.x;
                    y += direction.y;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            } 
        }        
    }

}

public class MatchResult
{
    public List<Element> connectedElements;
    public MatchDirection matchDirection;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    None
}

