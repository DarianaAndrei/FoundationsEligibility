using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TicTacToeState
{
    none,
    cross,
    circle
}

[System.Serializable]
public class WinnerEvent : UnityEvent<int> { }

public class TicTacToeAI : MonoBehaviour
{
    int _aiLevel;

    TicTacToeState[,] boardState;

    [SerializeField]
    private bool _isPlayerTurn;

    [SerializeField]
    private int _gridSize = 3;

    [SerializeField]
    private TicTacToeState playerState = TicTacToeState.circle;
    TicTacToeState aiState = TicTacToeState.cross;
    TicTacToeState emptyState = TicTacToeState.none;

    [SerializeField]
    private GameObject _xPrefab;

    [SerializeField]
    private GameObject _oPrefab;

    public UnityEvent onGameStarted;

    //Call This event with the player number to denote the winner
    public WinnerEvent onPlayerWin;

    ClickTrigger[,] _triggers;

    private void Awake()
    {
        if (onPlayerWin == null)
        {
            onPlayerWin = new WinnerEvent();
        }
    }

    public void StartAI(int AILevel)
    {
        _aiLevel = AILevel;

        StartGame();
    }

    public void RegisterTransform(int myCoordX, int myCoordY, ClickTrigger clickTrigger)
    {
        _triggers[myCoordX, myCoordY] = clickTrigger;
    }

    private void StartGame()
    {
        _triggers = new ClickTrigger[_gridSize, _gridSize];
        boardState = new TicTacToeState[_gridSize, _gridSize];
        onGameStarted.Invoke();
    }

    private bool CheckIfGameOver()
    {
        int result = CheckForWinner();
        if (result >= -1)
        {
            onPlayerWin.Invoke(result);
            return true;
        }
        return false;
    }

    public void PlayerSelects(int coordX, int coordY)
    {
        if (_isPlayerTurn && IsBoardPositionFree(coordX, coordY))
        {
            SetVisual(coordX, coordY, playerState);
            _isPlayerTurn = !_isPlayerTurn;
            CheckIfGameOver();
            AIMoves();
        }
    }

    private void AIMoves()
    {
        (int, int) move = FindBestMove(boardState, _aiLevel);
        AiSelects(move.Item1, move.Item2);
    }

    public void AiSelects(int coordX, int coordY)
    {
        if (!_isPlayerTurn && IsBoardPositionFree(coordX, coordY))
        {
            SetVisual(coordX, coordY, aiState);
            _isPlayerTurn = !_isPlayerTurn;
            CheckIfGameOver();
        }
    }

    private int EvaluateBoard(TicTacToeState[,] board)
    {
        // Check rows for winner
        for (int row = 0; row < _gridSize; row++)
        {
            if (board[row, 0] == board[row, 1] && board[row, 1] == board[row, 2])
            {
                if (board[row, 0] == aiState)
                    return 10;
                else if (board[row, 0] == playerState)
                    return -10;
            }
        }

        // Checking columns for winner
        for (int col = 0; col < 3; col++)
        {
            if (board[0, col] == board[1, col] && board[1, col] == board[2, col])
            {
                if (board[0, col] == aiState)
                    return 10;
                else if (board[0, col] == playerState)
                    return -10;
            }
        }

        // Chewck diagonals for winner
        if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
        {
            if (board[0, 0] == aiState)

                return 10;
            else if (board[0, 0] == playerState)
                return -10;
        }

        if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
        {
            if (board[0, 2] == aiState)
                return 10;
            else if (board[0, 2] == playerState)
                return -10;
        }

        return 0;
    }

    private int CheckForWinner()
    {
        // Check rows for winner
        for (int row = 0; row < _gridSize; row++)
        {
            if (
                boardState[row, 0] == boardState[row, 1] && boardState[row, 1] == boardState[row, 2]
            )
            {
                if (boardState[row, 0] == aiState)
                    return 1;
                else if (boardState[row, 0] == playerState)
                    return 0;
            }
        }

        // Checking columns for winner
        for (int col = 0; col < 3; col++)
        {
            if (
                boardState[0, col] == boardState[1, col] && boardState[1, col] == boardState[2, col]
            )
            {
                if (boardState[0, col] == aiState)
                    return 1;
                else if (boardState[0, col] == playerState)
                    return 0;
            }
        }

        // Chewck diagonals for winner
        if (boardState[0, 0] == boardState[1, 1] && boardState[1, 1] == boardState[2, 2])
        {
            if (boardState[0, 0] == aiState)

                return 1;
            else if (boardState[0, 0] == playerState)
                return 0;
        }

        if (boardState[0, 2] == boardState[1, 1] && boardState[1, 1] == boardState[2, 0])
        {
            if (boardState[0, 2] == aiState)
                return 1;
            else if (boardState[0, 2] == playerState)
                return 0;
        }

        if (!CheckMovesLeft(boardState))
        {
            return -1;
        }

        return -2;
    }

    private bool CheckMovesLeft(TicTacToeState[,] board)
    {
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                if (board[i, j] == emptyState)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private int MiniMax(TicTacToeState[,] board, int depth, Boolean isMax)
    {
        int score = EvaluateBoard(board);

        // If Maximizer has won the game
        // return his/her evaluated score
        if (score == 10)
            return score;

        // If Minimizer has won the game
        // return his/her evaluated score
        if (score == -10)
            return score;

        // If there are no more moves and
        // no winner then it is a tie
        if (CheckMovesLeft(board) == false)
            return 0;

        // If this maximizer's move
        if (isMax)
        {
            int best = -1000;

            // Traverse all cells
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == emptyState)
                    {
                        // Make the move
                        board[i, j] = aiState;

                        // Call minimax recursively and choose
                        // the maximum value
                        best = Math.Max(best, MiniMax(board, depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = emptyState;
                    }
                }
            }
            return best;
        }
        // If this minimizer's move
        else
        {
            int best = 1000;

            // Traverse all cells
            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    // Check if cell is empty
                    if (board[i, j] == emptyState)
                    {
                        // Make the move
                        board[i, j] = playerState;

                        // Call minimax recursively and choose
                        // the minimum value
                        best = Math.Min(best, MiniMax(board, depth + 1, !isMax));

                        // Undo the move
                        board[i, j] = emptyState;
                    }
                }
            }
            return best;
        }
    }

    // This will return the best possible
    // move for the player
    private (int coordX, int coordY) FindBestMove(TicTacToeState[,] board, int aiLevel)
    {
        int bestVal = -1000;
        int bestRow = -1;
        int bestColumn = -1;

        // Traverse all cells, evaluate minimax function
        // for all empty cells. And return the cell
        // with optimal value.
        for (int i = 0; i < _gridSize; i++)
        {
            for (int j = 0; j < _gridSize; j++)
            {
                // Check if cell is empty
                if (IsBoardPositionFree(i, j))
                {
                    // Make the move
                    board[i, j] = aiState;

                    // compute evaluation function for this
                    // move.
                    int moveVal = MiniMax(board, 0, false);

                    // Undo the move
                    board[i, j] = TicTacToeState.none;

                    // If the value of the current move is
                    // more than the best value, then update
                    // best/
                    if (aiLevel == 1)
                    {
                        // Hard
                        if (moveVal > bestVal)
                        {
                            bestRow = i;
                            bestColumn = j;
                            bestVal = moveVal;
                        }
                    }
                    else
                    {
                        // Easy
                        if (moveVal > bestVal && UnityEngine.Random.Range(1, 100) <= 66)
                        {
                            bestRow = i;
                            bestColumn = j;
                            bestVal = moveVal;
                        }
                    }
                }
            }
        }

        return (bestRow, bestColumn);
    }

    private bool IsBoardPositionFree(int coordX, int coordY)
    {
        if (coordX < 0 || coordY < 0)
        {
            return false;
        }
        return boardState[coordX, coordY] == TicTacToeState.none;
    }

    private void SetVisual(int coordX, int coordY, TicTacToeState targetState)
    {
        Instantiate(
            targetState == TicTacToeState.circle ? _oPrefab : _xPrefab,
            _triggers[coordX, coordY].transform.position,
            Quaternion.identity
        );
        boardState[coordX, coordY] = targetState;
    }
}
