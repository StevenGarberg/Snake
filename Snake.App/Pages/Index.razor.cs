using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Snake.App.Enums;
#pragma warning disable 4014

namespace Snake.App.Pages
{
    public partial class Index : ComponentBase
    {
        private ElementReference gameContainer;
        private BoardState?[][] _board;
        private List<(int i, int j)> _snake;
        private Direction _currentDirection;
        private bool? _gameOver;
        private int _score;
        
        private async Task Start()
        {
            _board = new BoardState?[40][];
            _snake = new List<(int i, int j)> { new(20, 20) };
            _currentDirection = Direction.Right;
            _gameOver = false;
            _score = 0;

            for (var i = 0; i < 40; i++)
            {
                _board[i] = new BoardState?[40];
            }

            _board[20][20] = BoardState.Snake;
            PlaceFood();
            await gameContainer.FocusAsync();
            GameLoop();
        }

        private async Task GameLoop()
        {
            while (_gameOver == false)
            {
                var delayTime = 250 - (_score * 10);
                if (delayTime < 50) delayTime = 50;
                await Task.Delay(delayTime);
                try
                {
                    MoveSnake();
                }
                catch(Exception e)
                {
                    _gameOver = true;
                    Console.WriteLine("Game over!");
                }
                StateHasChanged();
            }
        }
        
        private void PlaceFood()
        {
            var potentialSpots = new List<(int x, int y)>();
            for (var i = 0; i < 40; i++)
            {
                for (var j = 0; j < 40; j++)
                {
                    if (_board[i][j] == null)
                        potentialSpots.Add((i, j));
                }
            }
            var randomIndex = new Random().Next(0, potentialSpots.Count);
            var randomSpot = potentialSpots[randomIndex];
            
            _board[randomSpot.x][randomSpot.y] = BoardState.Food;
        }

        private void ChangeDirection(Direction direction)
        {
            _currentDirection = direction;
        }

        private void KeyboardEventHandler(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                default:
                    return;
                case "a":
                case "ArrowLeft":
                    _currentDirection = Direction.Left;
                    break;
                case "d":
                case "ArrowRight":
                    _currentDirection = Direction.Right;
                    break;
                case "w":
                case "ArrowUp":
                    _currentDirection = Direction.Up;
                    break;
                case "s":
                case "ArrowDown":
                    _currentDirection = Direction.Down;
                    break;
            }
        }
        
        private void MoveSnake()
        {
            (int i, int j) movement = _currentDirection switch
            {
                Direction.Down => (1, 0),
                Direction.Up => (-1, 0),
                Direction.Right => (0, 1),
                Direction.Left => (0, -1),
                _ => (0, 0)
            };

            var playedHead = _snake[0];
            (int i, int j) newLocation = (playedHead.i + movement.i, playedHead.j + movement.j);
            
            var currentItem = _board[newLocation.i][newLocation.j];
            if (currentItem == BoardState.Food)
            {
                _score++;
                _board[newLocation.i][newLocation.j] = BoardState.Snake;
                _snake.Insert(0, (newLocation.i, newLocation.j));
                PlaceFood();
            }
            else if (currentItem == BoardState.Snake)
            {
                _gameOver = true;
                Console.WriteLine("Game over!");
            }
            else
            {
                var lastPosition = _snake[0];
                _board[_snake[0].i][_snake[0].j] = null;
                _snake[0] = (newLocation.i, newLocation.j);
                _board[newLocation.i][newLocation.j] = BoardState.Snake;
                
                for (var i = 1; i < _snake.Count; i++)
                {
                    _board[_snake[i].i][_snake[i].j] = null;
                    (_snake[i], lastPosition) = (lastPosition, _snake[i]);
                    _board[_snake[i].i][_snake[i].j] = BoardState.Snake;
                }
            }
        }
    }
}