using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using File = System.IO.File;

public class Board : MonoBehaviour
{
    int _width;
    int _height;
    int _numOfMoves;
    int _score = 0;
    Tile[,] _tiles;
    List<int> _finishedRows = new List<int>();
    List<ColorVector4> _colorsCount = new List<ColorVector4>(); // blue, green, red, yellow
    TextMeshProUGUI _movesText;
    TextMeshProUGUI _scoreText;
    public Tile TilePrefab;
    public GameObject SpritePrefab;
    public GameObject GameOverPanel;
    public Sprite BlueSprite, GreenSprite, RedSprite, YellowSprite, DoneSprite;
    public AudioClip OnMove;
    public AudioClip OnMatch;
    
    // Start is called before the first frame update
    void Start()
    {
        Regex rx = new Regex(@"\d+");   
        
        int level = LevelsPopUp.CurrentLevel;
        string[] levelString = File.ReadAllLines("Assets/LevelInstructions/RM_A" + level);
        
        _width = Int32.Parse(rx.Match(levelString[1]).Value);
        _height = Int32.Parse(rx.Match(levelString[2]).Value);
        _numOfMoves = Int32.Parse(rx.Match(levelString[3]).Value);
        
        TextMeshProUGUI[] components = transform.parent.gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        _scoreText = components[0];
        _movesText = components[1];
        
        _movesText.text = "Remaining Moves: " + _numOfMoves.ToString();
        _scoreText.text = "Score: " + _score.ToString();
        CreateTiles(levelString[4]);
    }

    void CreateTiles(string grid)
    {
        Regex rx = new Regex(@"([bygr],)+[bygr]");
        string[] gridArray = rx.Match(grid).Value.Split(',');
        _tiles = new Tile[_height, _width];
        
        // considering both rows and columns for the grid
        // we need to add paddings from each sides to make the grid centered
        Transform parentTransform = transform.parent;
        Vector2 sizeDelta = parentTransform.GetComponent<RectTransform>().sizeDelta;
        sizeDelta.x *= 0.8f;
        sizeDelta.y *= 0.7f;
        
        GridLayoutGroup gridLayoutGroup = GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraintCount = _width;
        
        // cell size must be 10 less than the spacing to avoid overlap
        // the axes must be equal to avoid distortion
        // for example if sizeDelta / width == 100, then cellSize = 45, spacing = 55
        // choosing the smaller one to avoid masking
        float ratio = Math.Min(sizeDelta.x / _width, sizeDelta.y / _height);
        float cellSize = (ratio - 10) / 2;
        
        gridLayoutGroup.cellSize = new Vector2((ratio - 10) / 2, (ratio - 10) / 2);
        gridLayoutGroup.spacing = new Vector2((ratio + 10) / 2, (ratio + 10) / 2);
        
        for (int i = _height-1; i >= 0; i--)
        {
            ColorVector4 colors = new ColorVector4(0, 0, 0, 0);
            for (int j = 0; j < _width; j++)
            {
                Color color = Tile.GetColor(gridArray[i * _width + j]);
                SetColor(color, colors, 1);
                
                Tile tile = Instantiate(TilePrefab, new Vector3(), Quaternion.identity);
                GameObject child = Instantiate(SpritePrefab, new Vector3(), Quaternion.identity);
                
                Transform tileTransform = tile.transform;
                RectTransform childTransform = child.GetComponent<RectTransform>();
                
                childTransform.sizeDelta = new Vector2(cellSize, cellSize);
                    
                tile.name = "Tile " + (_height - i - 1) + ", " + j;
                child.name = "Sprite " + (_height - i - 1) + ", " + j;
                
                child.transform.SetParent(tileTransform);
                tileTransform.SetParent(transform);
                
                tile.color = color;
                child.GetComponent<Image>().sprite = GetSprite(color);
                
                tile.size = cellSize;
                tile.row = _height - i - 1;
                tile.column = j;
                
                _tiles[tile.row, j] = tile;
                tile.isActive = true;
            }
            _colorsCount.Add(colors);
        }

        ResizeBoard(sizeDelta);
    }
    
    public ColorVector4 GetRowCount(int row)
    {
        return _colorsCount[row];
    }

    public void SetColor(Color color, ColorVector4 colors, int value)
    {
        switch (color)
        {
            case Color.Blue:
                colors.Blue += value;
                break;
            case Color.Green:
                colors.Green += value;
                break;
            case Color.Red:
                colors.Red += value;
                break;
            case Color.Yellow:
                colors.Yellow += value;
                break;
            default:
                throw new InvalidOperationException("Invalid color!");
        }
    }
    void ResizeBoard(Vector2 boardSize)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        // GameObject referenceTile = _tiles[_height - 1, 0].gameObject;
        float size = 2 * _tiles[_height - 1, 0].size + 10;

        float xPadding = boardSize.x - _width * size - 20;
        float yPadding = boardSize.y - _height * size - 20;
        
        
        // Vector3 referencePosition = referenceTile.transform.position;
        // Vector2 boardPosition = GetComponent<RectTransform>().sizeDelta;
        // Debug.Log(referencePosition);
        // Debug.Log(boardPosition);
        // // float xPadding = referencePosition.x - size - 5 - boardPosition.x;
        // // float yPadding = boardPosition.y - referencePosition.y - size - 5;
        // Debug.Log(xPadding);
        // Debug.Log(yPadding);
        rectTransform.offsetMax = new Vector2(-xPadding / 2, -yPadding / 2);
        rectTransform.offsetMin = new Vector2(xPadding / 2, yPadding / 2);
    }
    
    Sprite GetSprite(Color color)
    {
        switch (color)
        {
            case Color.Blue:
                return BlueSprite;
            case Color.Yellow:
                return YellowSprite;
            case Color.Red:
                return RedSprite;
            case Color.Green:
                return GreenSprite;
            default:
                throw new InvalidOperationException("Invalid color!");
        }
    }
    
    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= _height || y < 0 || y >= _width)
            return null;
        return _tiles[x, y];
    }
    
    public void ReduceMoves()
    {
        _movesText.text = "Remaining Moves: " + --_numOfMoves;
        GetComponent<AudioSource>().PlayOneShot(OnMove);
    }
    public void CheckRow(int row)
    {
        bool isRowDone = true;
        Color color = _tiles[row, 0].color;
        for (int i = 1; i < _width; i++)
        {
            if (_tiles[row, i].color != color)
            {
                isRowDone = false;
                break;
            }
        }

        if (isRowDone)
        {
            GetComponent<AudioSource>().PlayOneShot(OnMatch);
            _finishedRows.Add(row);
            _score += Tile.GetScore(color);
            _scoreText.text = "Score: " + _score; 
            for (int i = 0; i < _width; i++)
            {
                Tile tile = _tiles[row, i];
                tile.isActive = false;
                tile.color = Color.None;
                tile.GetComponentInChildren<Image>().sprite = DoneSprite;
            }
        }
    }

    public void CheckMove()
    {
        if (_numOfMoves == 0)
        {
            GameOver();
            return;
        }
        ColorVector4 totalColors = new ColorVector4();
        bool isGameOver = true;
        for (int i = 0; i < _height; i++)
        {
            if (_finishedRows.Contains(i))
            {
                if (totalColors.Red >= _width || totalColors.Green >= _width ||
                    totalColors.Blue >= _width || totalColors.Yellow >= _width)
                {
                    isGameOver = false;
                    break;
                }
                totalColors = new ColorVector4();
            }
            else
            {
                ColorVector4 colors = _colorsCount[i];
                totalColors += colors;
            }
        }
        Debug.Log(totalColors);
        if (isGameOver && !(totalColors.Red >= _width || totalColors.Green >= _width ||
                            totalColors.Blue >= _width || totalColors.Yellow >= _width))
        {
            
            GameOver();
        }
    }

    void GameOver()
    {
        PlayerInfo info = DataSaver.LoadData<PlayerInfo>(LevelsPopUp.PlayerInfoName);
        Transform gameOver = GameOverPanel.transform;
        gameOver.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        GameOverPanel.SetActive(true);
        
        List<int> scores = info.HighScores;
        int currentScore = scores[LevelsPopUp.CurrentLevel - 1];
        
        if (_score > currentScore)
        {
            scores[LevelsPopUp.CurrentLevel - 1] = _score;
            info.HighScores = scores;
            DataSaver.SaveData(info, LevelsPopUp.PlayerInfoName);
            gameOver.GetComponentInChildren<TextMeshProUGUI>().text = $"CONGRATULATIONS!\nNEW HIGH SCORE:\n{_score.ToString()}";
        }else
        {
            gameOver.GetComponent<AudioSource>().mute = true;
            gameOver.GetComponentInChildren<TextMeshProUGUI>().text = $"Game Over!\nYour Score: {_score.ToString()}\nHigh Score: {currentScore.ToString()}";
        }
        
        gameOver.DOScale(new Vector3(1, 1, 1), 0.5f);
    }
    
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
    
    // Update is called once per frame
    void Update()
    {
    }
}
