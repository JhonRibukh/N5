using System;

namespace Snake
{
    enum Bounds : int
    {
        MaxRight = 40,
        MaxBottom = 20
    }

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    class Snake
    {
        private List<Tuple<int, int>> snake;
        private Tuple<int, int> apple;
        private Direction direction;
        private Tuple<int, int> tail;
        private bool eatedApple;
        private bool gameStarted;
        private bool changedDirection;
        private int score;
        private bool gameOver;

        private static void DrawArena()
        {
            Console.SetCursorPosition(0, 0);
            Console.Write(new string('#', (int)Bounds.MaxRight));
            Console.SetCursorPosition(0, (int)Bounds.MaxBottom - 1);
            Console.Write(new string('#', (int)Bounds.MaxRight));

            for (var i = 1; i < (int)Bounds.MaxBottom - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write('#');
                Console.SetCursorPosition((int)Bounds.MaxRight - 1, i);
                Console.Write('#');
            }
        }

        private void InitializeSnake()
        {
            var rnd = new Random();
            direction = (Direction)rnd.Next(0, 4);

            snake = new List<Tuple<int, int>>();
            snake.Add(new Tuple<int, int>((int)Bounds.MaxRight / 2, (int)Bounds.MaxBottom / 2));

            switch (direction)
            {
                case Direction.Up:
                    snake.Add(new Tuple<int, int>((int)Bounds.MaxRight / 2, (int)Bounds.MaxBottom / 2 + 1));
                    break;
                case Direction.Down:
                    snake.Add(new Tuple<int, int>((int)Bounds.MaxRight / 2, (int)Bounds.MaxBottom / 2 - 1));
                    break;
                case Direction.Left:
                    snake.Add(new Tuple<int, int>((int)Bounds.MaxRight / 2 + 1, (int)Bounds.MaxBottom / 2));
                    break;
                case Direction.Right:
                    snake.Add(new Tuple<int, int>((int)Bounds.MaxRight / 2 - 1, (int)Bounds.MaxBottom / 2));
                    break;
            }

            tail = snake.Last();
            eatedApple = false;
            changedDirection = false;
        }

        private void DrawSnake()
        {
            var first = true;
            foreach (var part in snake)
            {
                Console.SetCursorPosition(part.Item1, part.Item2);
                if (first)
                {
                    Console.Write('@');
                    first = false;
                }
                else
                {
                    Console.Write('0');
                }
            }

            if (!gameStarted)
            {
                gameStarted = true;
                return;
            }

            if (!eatedApple)
            {
                Console.SetCursorPosition(tail.Item1, tail.Item2);
                Console.Write(' ');
            }
            else
            {
                Console.SetCursorPosition(tail.Item1, tail.Item2);
                Console.Write('0');
                eatedApple = false;
            }

            Console.SetCursorPosition((int)Bounds.MaxRight + 1, (int)Bounds.MaxBottom + 1);
        }

        private void MoveSnake()
        {
            while (true)
            {
                Thread.Sleep(250);

                var part = snake[0];
                switch (direction)
                {
                    case Direction.Up:
                        snake[0] = new Tuple<int, int>(part.Item1, part.Item2 - 1);
                        break;
                    case Direction.Down:
                        snake[0] = new Tuple<int, int>(part.Item1, part.Item2 + 1);
                        break;
                    case Direction.Left:
                        snake[0] = new Tuple<int, int>(part.Item1 - 1, part.Item2);
                        break;
                    case Direction.Right:
                        snake[0] = new Tuple<int, int>(part.Item1 + 1, part.Item2);
                        break;
                }

                tail = snake.Last();

                for (var i = 1; i < snake.Count; i++)
                {
                    (snake[i], part) = (part, snake[i]);
                }

                if (snake[0].Item1 == apple.Item1 && snake[0].Item2 == apple.Item2)
                {
                    eatedApple = true;
                    score++;
                    snake.Add(tail);
                    SpawnApple();
                }

                CheckCollision();

                if (gameOver)
                {
                    break;
                }

                DrawSnake();

                changedDirection = false;
            }
        }

        private void Game()
        {
            while (true)
            {
                if (gameOver)
                {
                    break;
                }

                var key = Console.ReadKey();

                if (changedDirection)
                {
                    continue;
                }


                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        if (direction != Direction.Down && direction != Direction.Up)
                        {
                            direction = Direction.Up;
                            changedDirection = true;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if (direction != Direction.Up && direction != Direction.Down)
                        {
                            direction = Direction.Down;
                            changedDirection = true;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (direction != Direction.Right && direction != Direction.Left)
                        {
                            direction = Direction.Left;
                            changedDirection = true;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        if (direction != Direction.Left && direction != Direction.Right)
                        {
                            direction = Direction.Right;
                            changedDirection = true;
                        }

                        break;
                }
            }
        }

        private void SpawnApple()
        {
            var availablePositions = new List<Tuple<int, int>>();
            for (var i = 1; i < (int)Bounds.MaxRight - 1; i++)
            {
                for (var j = 1; j < (int)Bounds.MaxBottom - 1; j++)
                {
                    var flag = true;
                    foreach (var part in snake)
                    {
                        if (part.Item1 == i && part.Item2 == j)
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        availablePositions.Add(new Tuple<int, int>(i, j));
                    }
                }
            }


            var rnd = new Random();
            apple = new Tuple<int, int>(availablePositions[rnd.Next(0, availablePositions.Count)].Item1,
                availablePositions[rnd.Next(0, availablePositions.Count)].Item2);

            Console.SetCursorPosition(apple.Item1, apple.Item2);
            Console.Write('*');
        }

        private void CheckCollision()
        {
            if (snake[0].Item1 == 0 || snake[0].Item1 == (int)Bounds.MaxRight - 1 ||
                snake[0].Item2 == 0 || snake[0].Item2 == (int)Bounds.MaxBottom - 1)
            {
                gameOver = true;
                Console.Clear();
                Console.WriteLine("Игра окончена, ты проиграл!");
                Console.WriteLine($"Твой счет: {score}");
                Console.WriteLine("Нажми любую кнопку для выхода");
            }

            for (var i = 1; i < snake.Count; i++)
            {
                if (snake[0].Item1 == snake[i].Item1 && snake[0].Item2 == snake[i].Item2)
                {
                    gameOver = true;
                    Console.Clear();
                    Console.WriteLine("Игра окончена, ты проиграл!");
                    Console.WriteLine($"Твой счет: {score}");
                    Console.WriteLine("Нажми любую кнопку для выхода");
                }
            }
        }

        private void CheckWin()
        {
            if (snake.Count == (int)Bounds.MaxRight * (int)Bounds.MaxBottom - 1)
            {
                gameOver = true;
                Console.Clear();
                Console.WriteLine("Ты победил!");
                Console.WriteLine($"Твой счет: {score}");
                Console.WriteLine("Нажми любую кнопку для выхода.");
            }
        }

        public Snake()
        {
            gameStarted = false;
            gameOver = false;
            score = 0;
            InitializeSnake();
            DrawSnake();
            SpawnApple();
        }

        public static void Main(string[] args)
        {
            Console.Clear();
            DrawArena();
            var snake = new Snake();

            var drawSnakeThread = new Thread(snake.MoveSnake);
            var gameThread = new Thread(snake.Game);

            Console.ReadKey();

            drawSnakeThread.Start();
            gameThread.Start();

            gameThread.Join();
            drawSnakeThread.Join();
        }
    }
}