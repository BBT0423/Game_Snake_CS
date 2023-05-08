using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace WpfApp4
{
    public class gamestate
    {
        public int Row { get; }
        public int Column { get; }
        public Gridevalue[,] Grid { get; }
        public Directon Dir { get; private set; }
        public int Score { get; private set; }
        public bool GameOver { get; private set; }

        private readonly LinkedList<Directon> dirChange = new LinkedList<Directon>();
        private readonly LinkedList<Position> snakePositions = new LinkedList<Position>();
        private readonly Random random = new Random();

        public gamestate(int rows,int cols) 
        {
            Row = rows;
            Column = cols;
            Grid = new Gridevalue[rows, cols];
            Dir = Directon.Right;

            AddSnake();
            AddFood();
        }

        private void AddSnake()
        {
            int r = Row / 2;
            
            for(int c = 1; c <= 3; c++)
            {
                Grid[r, c] = Gridevalue.Snake;
                snakePositions.AddFirst(new Position(r, c));
            }
        }

        private IEnumerable<Position> EmptyPositions()
        {
            for(int r = 0;r<Row; r++)
            {
                for(int c = 0; c < Column; c++)
                {
                    if (Grid[r,c] == Gridevalue.Empty)
                    {
                        yield return new Position(r, c);
                    }
                }
            }
        }

        private void AddFood()
        {
            List<Position> empty = new List<Position>(EmptyPositions());

            if(empty.Count == 0)
            {
                return;
            }

            Position pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Column] = Gridevalue.Food;
        }

        public Position HeadPosition()
        {
            return snakePositions.First.Value;
        }

        public Position TailPosition() 
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<Position> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(Position pos)
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Column] = Gridevalue.Snake;
        }

        private void RemoveTail()
        {
            Position tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Column] = Gridevalue.Empty;
            snakePositions.RemoveLast();
        }

        private Directon GetLastDirection()
        {
            if(dirChange.Count == 0)
            {
                return Dir;
            }
            return dirChange.Last.Value;
        }

        private bool CanChangeDirection(Directon newDir)
        {
            if(dirChange.Count == 2)
            {
                return false;
            }

            Directon lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite();
        }
        public void ChangeDirection(Directon dir)
        {
            //Dir = dir; if can change direction
            if(CanChangeDirection(dir))
            {
                dirChange.AddLast(dir);
            }
        }

        private bool OutsideGrid(Position pos)
        {
            return pos.Row < 0 || pos.Row >= Row || pos.Column < 0 || pos.Column >= Column;
        }

        private Gridevalue WillHit(Position newHeadpos)
        {
            if (OutsideGrid(newHeadpos))
            {
                return Gridevalue.Outside;
            }

            if(newHeadpos == TailPosition())
            {
                return Gridevalue.Empty;
            }

            return Grid[newHeadpos.Row, newHeadpos.Column];
        }

        public void Move()
        {
            if(dirChange.Count > 0)
            {
                Dir = dirChange.First.Value;
                dirChange.RemoveFirst();
            }
            Position newHeadPos = HeadPosition().Translate(Dir);
            Gridevalue hit = WillHit(newHeadPos);

            if(hit == Gridevalue.Outside || hit == Gridevalue.Snake)
            {
                GameOver = true;
            }
            else if(hit == Gridevalue.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }
            else if(hit == Gridevalue.Food)
            {
                AddHead(newHeadPos);
                Score++;
                AddFood();
            }
        }
    }
}
