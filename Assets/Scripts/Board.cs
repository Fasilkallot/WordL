using System;
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


    private void Awake()
    {
        rows = GetComponentsInChildren<Raw>();
    }
    private void Start()
    {
        LoadData();
        SetWord();
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

        for (int i = 0; i < currentRaw.tiles.Length; i++)
        {
            Tile tile = currentRaw.tiles[i];

            if (word[i] == tile.letter)
            {
                tile.SetState(correctState);
            }
            else if (word.Contains(tile.letter))
            {
                tile.SetState(wrongSpotState);
            }
            else
            {
                tile.SetState(incorrectState);
            }
            
        }
        rowIndex++;
        colIndex = 0;

        if (rowIndex >= rows.Length)
        {
            enabled = false;
        }
    }
}
