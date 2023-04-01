using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Color color;
    public static float SizeThreshold = 0.4f;
    public float size;
    public int row, column;
    public bool isActive = true;
    public Vector3 InitialMousePosition, FinalMousePosition;
    
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(2, 2, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InitialMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isActive)
            return;
        
        FinalMousePosition = Input.mousePosition;
        Vector3 direction = FinalMousePosition - InitialMousePosition;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (Mathf.Abs(direction.x) > size * SizeThreshold)
            {
                if (direction.x > 0)
                {
                    MoveTile(row, column + 1);
                    // Debug.Log("Right");
                }
                else
                {
                    MoveTile(row, column - 1);
                    // Debug.Log("Left");
                }
            }
            
        }
        else
        {
            if (Mathf.Abs(direction.y) > size * SizeThreshold)
            {
                if (direction.y > 0)
                {
                    MoveTile(row + 1, column);
                    // Debug.Log("Up");
                }
                else
                {
                    MoveTile(row - 1, column);
                    // Debug.Log("Down");
                }
            }
            
        }
    }
    
    void MoveTile(int x, int y)
    {
        Board board = GetComponentInParent<Board>();
        Tile otherTile = board.GetTile(x, y);
        if (!otherTile.isActive)
            return;
        
        Transform otherTileTransform = otherTile.transform;
        Transform tileChild = transform.GetChild(0);
        Transform otherTileChild = otherTile.transform.GetChild(0);
        
        (otherTile.color, color) = (color, otherTile.color);
        (tileChild.name, otherTileChild.name) = (otherTileChild.name, tileChild.name);

        tileChild.DOMove(otherTileTransform.position, 0.2f);
        otherTileChild.DOMove(transform.position, 0.2f).onKill = () =>
        {
            if (y == column)
            {
                ColorVector4 rowColors = board.GetRowCount(row);
                ColorVector4 otherRowColors = board.GetRowCount(otherTile.row);
                Debug.Log(rowColors);
                Debug.Log(otherRowColors);
                board.SetColor(otherTile.color, rowColors, -1);
                board.SetColor(color, otherRowColors, -1);
                board.SetColor(otherTile.color, otherRowColors, 1);
                board.SetColor(color, rowColors, 1);
                board.CheckRow(row);
                board.CheckRow(otherTile.row);
                Debug.Log(board.GetRowCount(row));
                Debug.Log(board.GetRowCount(otherTile.row));
            }
            board.CheckMove();
        };

        tileChild.SetParent(otherTileTransform);
        otherTileChild.SetParent(transform);
        
        
        board.ReduceMoves();
    }
    
    public static Color GetColor(string color)
    {
        switch (color)
        {
            case "b":
                return Color.Blue;
            case "y":
                return Color.Yellow;
            case "r":
                return Color.Red;
            case "g":
                return Color.Green;
            default:
                throw new InvalidOperationException("Invalid color!");
        }
    }

    public static int GetScore(Color color)
    {
        switch (color)
        {
            case Color.Blue:
                return 200;
            case Color.Red:
                return 100;
            case Color.Yellow:
                return 250;
            case Color.Green:
                return 150;
            default:
                throw new InvalidOperationException("Invalid color!");
        }
    }
}

public enum Color
{
    Red,
    Green,
    Blue,
    Yellow,
    None
}
