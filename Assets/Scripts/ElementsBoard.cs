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

    public LevelManager levelManager;

    public Element[] elementsprefabs;
    public ParticleSystem[] particles;
    public int delayPS;

    public Cell[,] cellBoard;

    public GameObject elementsBoardSet;

    public static ElementsBoard instance;

    private List<Element> selectedElements = new List<Element>();
    private bool isSwapping = false;

    private bool isGameStarted = false;
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
        
        for (int y = 0; y < cols; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                int randomeIndex = Random.Range(0, elementsprefabs.Length);

                Element element = Instantiate(elementsprefabs[randomeIndex], position, Quaternion.identity);
                element.SetIndicies(x, y);
                cellBoard[x, y] = new Cell(true, element);
            }
        }
        while (CheckBoard()) { }
    }



    public void SelectedElement(Element element)
    {
        isGameStarted = true;
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
                    TrySwapElements(selectedElements[0], selectedElements[1]);
                }
                selectedElements.Clear();
            }
            else
            {
                selectedElements.Clear();
            }
        }

    }

    private void TrySwapElements(Element element1, Element element2)
    {
        SwapElementsPosition(element1, element2);

        bool hasMatched = CheckBoard();

        if (!hasMatched)
        {

            SoundManager.Instance.PlaySound(Sounds.Error);
            SwapElementsPosition(element1, element2);
        }
        else
        {
            if (isGameStarted)
            {
                SoundManager.Instance.PlaySound(Sounds.Gemclick);
                levelManager.MakeMove();
            }            
        }
        while (CheckBoard()) { }
    }


    private bool AreElementsAdjacent(Element element1, Element element2)
    {
        int dx = Mathf.Abs(element1.xIndex - element2.xIndex);
        int dy = Mathf.Abs(element1.yIndex - element2.yIndex);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }


    private void SwapElementsPosition(Element element1, Element element2)
    {
        Vector2Int tempIndices = new Vector2Int(element1.xIndex, element1.yIndex);

        cellBoard[element1.xIndex, element1.yIndex].element = element2;
        cellBoard[element2.xIndex, element2.yIndex].element = element1;

        element1.SetIndicies(element2.xIndex, element2.yIndex);
        element2.SetIndicies(tempIndices.x, tempIndices.y);

        Vector3 tempPosition = element1.transform.position;

        element1.transform.position = element2.transform.position;
        element2.transform.position = tempPosition;
    }


    public bool CheckBoard()
    {
        bool hasmatched = false;

        List<Element> elementsToRemove = new();

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
            if(isGameStarted)
            {
                int scoreValue = CalculateScore(elementsToRemove);
                int blueCount = CalculateBlueCount(elementsToRemove);
                int redCount = CalculateRedCount(elementsToRemove);
                levelManager.UpdateScore(scoreValue);
                levelManager.BlueElementsCleared(blueCount);
                levelManager.RedElementsCleared(redCount);

            }
            RemoveElements(elementsToRemove);
            }

        return hasmatched;
    }

    private int CalculateRedCount(List<Element> elementsToRemove)
    {
        int count = 0;
        foreach (var element in elementsToRemove)
        {
            if (element.elementType == ElementType.Red)
            {
                count++;
            }
        }
        return count;
    }

    private int CalculateBlueCount(List<Element> elementsToRemove)
    {
        int count = 0;
        foreach (var element in elementsToRemove)
        {
            if (element.elementType == ElementType.Blue)
            {
                count++;
            }
        }
        return count;
    }

    private int CalculateScore(List<Element> elementsToRemove)
    {
        int totalScore = 0;
        foreach (var element in elementsToRemove)
        {
            totalScore += element.scoreValue;
        }
        return totalScore;
    }


    public void RemoveElements(List<Element> elementsToRemove)
    {
        foreach (var elementToRemove in elementsToRemove)
        {
            int x = elementToRemove.xIndex;
            int y = elementToRemove.yIndex;

            if (cellBoard[x, y].element == elementToRemove)
            {
                ElementType elementType = elementToRemove.elementType;
                ParticleSystem particleSystemPrefab = particles[(int)elementType];

                ParticleSystem newParticleSystem = Instantiate(particleSystemPrefab, elementToRemove.transform.position, Quaternion.identity);
                newParticleSystem.Play();

                Destroy(cellBoard[x, y].element.gameObject);

                StartCoroutine(WaitForNewElement(newParticleSystem, x, y));
            }
        }
    }

    private IEnumerator WaitForNewElement(ParticleSystem particleSystem,  int x, int y)
    {
        yield return new WaitForSeconds(delayPS);

        Destroy(particleSystem.gameObject);
        // Instantiate a new element after the delay
        int randomIndex = Random.Range(0, elementsprefabs.Length);
        Element newElement = Instantiate(elementsprefabs[randomIndex], new Vector2(x - spacingX, y - spacingY), Quaternion.identity);
        newElement.SetIndicies(x, y);
        cellBoard[x, y].element = newElement;
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
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.Horizontal
            };
        }
        else if(connectedElements.Count > 3)
        {
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
            return new MatchResult
            {
                connectedElements = connectedElements,
                matchDirection = MatchDirection.Vertical
            };
        }
        else if (connectedElements.Count > 3)
        {
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

