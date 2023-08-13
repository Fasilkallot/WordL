using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    private static readonly KeyCode[] SupportedKeys = new KeyCode[] {KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z, };

    private Raw[] rows;

    private string[] validWords;
    private string[] solutioons;
    private string word;

    private int rowIndex;
    private int colIndex;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State occupiedState;
    public Tile.State correctState;
    public Tile.State wrongSpotState;
    public Tile.State incorrectState;

    [Header("UI")]
    public GameObject invalidWord;
    public GameObject tryAgainButton;
    public GameObject newWordButton;
    public GameObject WinnerText;
    public GameObject GameOverText;
    public TextMeshProUGUI hintText;
    



    private void Awake()
    {
        rows = GetComponentsInChildren<Raw>();
    }
    private void Start()
    {
        LoadData();
        NewGame();
    }

    private void SetWord()
    {
        word = solutioons[Random.Range(0,solutioons.Length)];
        word = word.ToLower().Trim();
    }

    private void LoadData()
    {
        TextAsset textFile = Resources.Load("official_wordle_all") as TextAsset;
        validWords = textFile.text.Split('\n');

        textFile = Resources.Load("official_wordle_common") as TextAsset;
        solutioons = textFile.text.Split('\n');
    }

    void Update()
    {
        Raw currentRaw = rows[rowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            colIndex = Mathf.Max(colIndex - 1, 0);
            currentRaw.tiles[colIndex].SetLetter('\0');
            currentRaw.tiles[colIndex].SetState(emptyState);
            invalidWord.SetActive(false);

        }
        else if (colIndex >= currentRaw.tiles.Length)
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRaw(currentRaw);
            }
        }
        else
        {

            for (int i = 0; i < SupportedKeys.Length; i++)
            {
                if (Input.GetKeyDown(SupportedKeys[i]))
                {
                    currentRaw.tiles[colIndex].SetLetter((char)SupportedKeys[i]);
                    currentRaw.tiles[colIndex].SetState(occupiedState);
                    colIndex++;
                    break;
                }
            }
        }
    }

    private void SubmitRaw(Raw currentRaw)
    {
        if (!IsValidWord(currentRaw.word))
        {
            invalidWord.SetActive(true);
            return;
        }

        string remaining = word;

        for (int i = 0; i < currentRaw.tiles.Length; i++)
        {
            Tile tile = currentRaw.tiles[i];

            if (word[i] == tile.letter)
            {
                tile.SetState(correctState);

                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i, " ");
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetState(incorrectState);
            }
        
        }
        for(int i = 0;i < currentRaw.tiles.Length; i++)
        {
            Tile tile = currentRaw.tiles[i];

            if (tile.state != correctState && tile.state != incorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    tile.SetState(wrongSpotState);

                    int index = remaining.IndexOf(tile.letter);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetState(incorrectState);
                }
            }
        }
        if (isWin(currentRaw))
        {
            WinnerText.SetActive(true);
            enabled = false;
        }
        rowIndex++;
        colIndex = 0;

        if (rowIndex >= rows.Length)
        {
            GameOverText.SetActive(true);
            enabled = false;
        }
    }
    private void ClearBoard()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].tiles.Length; j++)
            {
                rows[i].tiles[j].SetLetter('\0');
                rows[i].tiles[j].SetState(emptyState);
            }
        }
        rowIndex = 0;
        colIndex = 0;
    }

    private bool IsValidWord(string word)
    {
        for(int i = 0; i < validWords.Length; i++) 
        {
            if (validWords[i] == word)  return true; 
        }
        return false;
    }
    private bool isWin(Raw currentRaw)
    {
        for (int i = 0; i < currentRaw.tiles.Length; i++)
        {
            if (currentRaw.tiles[i].state != correctState) return false;
        }
        return true;
    }
    private void OnEnable()
    {
        tryAgainButton.SetActive(false);
        newWordButton.SetActive(false);
        WinnerText.SetActive(false);
        hintText.gameObject.SetActive(false);
        GameOverText.SetActive(false);

    }
    private void OnDisable()
    {
        tryAgainButton.SetActive(true);
        newWordButton.SetActive(true);
    }
    public void NewGame()
    {
        ClearBoard();
        SetWord();
        enabled = true;
    }
    public void Retry()
    {
        ClearBoard();
        enabled = true;
    }
    public void HintButton()
    {
        hintText.text = $"Word to find is : {word.ToUpper()}";
        hintText.gameObject.SetActive(true);
    }
}
