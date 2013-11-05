using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace RushHour_Practicum2
{
	public class Solver
	{
        int boardWidth;
        Vertice root;
        Hashtable HashTable;
        ConcurrentQueue<Vertice> queue;

        public Solver (Board rootBoard, int xTarget, int yTarget, int outputMode)
		{
            boardWidth = rootBoard.width;
            
            HashTable = new Hashtable();
            root = new Vertice(rootBoard, null);
            
            HashTable.Add(rootBoard.Hash(), rootBoard);
            queue = new ConcurrentQueue<Vertice>();
            queue.Enqueue(root);
            solve(xTarget, yTarget, outputMode);
		}

        public void solve(int xTarget, int yTarget, int outputMode)
        {
            bool isSolved = false;
            Vertice dequeue;
            Vertice solvedGame = null;
            Console.WriteLine("Starting brute force solving..");
            while (!isSolved && queue.TryDequeue(out dequeue))
            {
                Console.WriteLine("Queuesize: "+ queue.Count);
                //Concurrent For loop gebruiken hierna
                List<Vertice> moves = new List<Vertice>(allPossibleMoves(dequeue.state));
                foreach (Vertice v in moves)
                {
                    if (v.state.board[xTarget, yTarget] == 'x')
                    {
                        try
                        {
                            isSolved = ((v.state.board[xTarget + 1, yTarget] == 'x') &&
                                       (v.state.board[xTarget, yTarget + 1] == 'x'));
                            if (isSolved)
                                solvedGame = v;
                        }
                        catch (IndexOutOfRangeException)
                        {

                        }
                    }
                    Console.WriteLine("Child added");
                    dequeue.AddChild(v);
                }
            }
            if (isSolved)
            {
                Console.WriteLine("Found!");
                if (outputMode == 0)
                    Console.WriteLine(solvedGame.countToRoot());
                else
                    Console.WriteLine(solvedGame.movesToRoot());
            }
            else if(queue.IsEmpty)
                Console.WriteLine("No solution");

        }

        /// <summary>
        /// For the state given, return all the possible moves.
        /// </summary>
        /// <param name="currentState">From which state to check</param>
        /// <returns>Unvisited states which can be got to in 1 move</returns>
        private List<Vertice> allPossibleMoves(Board currentState)
        {
            List<Vertice> result = new List<Vertice>();
            List<char> checkedCars = new List<char>();
            for (int x = 0; x < this.boardWidth; x++)
                for (int y = 0; y < this.boardWidth; y++)
                {
                    char car = currentState.board[x, y];

                    //Is there a car at the current spot.
                    //Or have we cheched this car already?
                    if (car == '.' || checkedCars.Contains(car))
                        continue;

                    Console.WriteLine("Checking: " + car);
                    //What direction does the car go? NS || WE
                    if (x < this.boardWidth && currentState.board[x + 1, y] == car)
                    {
                        //Horizontal moving

                        //Grab position of the car
                        int[] oldTopLeftCorner = {x,y};
                        
                        int endCar = x+1;
                        while(currentState.board[endCar, y] == car)
                            endCar++;
                        int[] oldBottomRightCorner = {endCar, y};

                        //Scan left and right for open position, for each position open
                        //we create a new vertice.
                        for (int i = x - 1; i >= 0; i--)
                        {
                            if (currentState.board[i, y] != '.')
                                break;
                            
                            int stepstaken = oldTopLeftCorner[0] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = {i, y};
                            int[] newBottomRightCorner = { endCar - stepstaken,y};

                            Board newBoard = createNewState(currentState, car, 
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            if (!isVisitedState(newBoard))
                            {
                                result.Add(new Vertice(newBoard, car + "l" + stepstaken));
                                HashTable.Add(newBoard.Hash(), newBoard);
                            }
                        }

                        //Now scan right of the car
                        for (int i = endCar + 1; i < this.boardWidth; i++)
                        {
                            if (currentState.board[i, y] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[0];

                            int[] newTopLeftCorner = { x + stepstaken, y };
                            int[] newBottomRightCorner = { i, y };

                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);

                            if (!isVisitedState(newBoard))
                            {
                                result.Add(new Vertice(newBoard, car + "r" + stepstaken));
                                HashTable.Add(newBoard.Hash(), newBoard);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(car + " goes Vertical");
                        //Set the old positions
                        int[] oldTopLeftCorner = { x, y };

                        int endCar = y + 1;
                        while (currentState.board[x, endCar] == car)
                            endCar++;
                        int[] oldBottomRightCorner = { x, endCar };

                        //Now look up and down for the all moves
                        //Looking up here
                        for (int i = y - 1; i >= 0; i--)
                        {
                            if (currentState.board[x, i] != '.')
                                break;

                            int stepstaken = oldTopLeftCorner[1] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = { x, i };
                            int[] newBottomRightCorner = { x, endCar - stepstaken };

                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            Console.WriteLine("New Board Check");
                            if (!isVisitedState(newBoard))
                            {
                                result.Add(new Vertice(newBoard, car + "d" + stepstaken));
                                HashTable.Add(newBoard.Hash(), newBoard);
                            }
                        }

                        //Now scan below the car
                        for (int i = endCar + 1; i < this.boardWidth; i++)
                        {
                            if (currentState.board[i, y] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[1];

                            int[] newTopLeftCorner = { x, y + stepstaken };
                            int[] newBottomRightCorner = { x, i };


                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            Console.WriteLine("New Board Check UP");
                            if (!isVisitedState(newBoard))
                            {
                                Console.WriteLine("New Board added");
                                result.Add(new Vertice(newBoard, car + "u" + stepstaken));
                                HashTable.Add(newBoard.Hash(), newBoard);
                            }
                        }
                    }
                    checkedCars.Add(car);
                }
                return result;
        }

        /// <summary>
        /// "Moves the car to the given position.
        /// </summary>
        /// <param name="car">Char to id the car</param>
        /// <param name="oldTopLeftCorner">[0] = x, [1] = y</param>
        /// <param name="oldBottomRightCorner">[0] = x, [1] = y</param>
        /// <param name="newTopLeftCorner">[0] = x, [1] = y</param>
        /// <param name="newBottomRightCorner">[0] = x, [1] = y</param>
        /// <returns>The board after the move</returns>
        private Board createNewState(Board board, char car, int[] oldTopLeftCorner, int[] oldBottomRightCorner,
                                                    int[] newTopLeftCorner, int[] newBottomRightCorner)
        {
            Board result = board;
            int x = oldTopLeftCorner[0];
            int y = oldTopLeftCorner[1];

            //Remove the old car
            //Do we move NS || WE
            bool goesVertical = (x == oldBottomRightCorner[0]);

            if (goesVertical)
            {
                //empty the spot the car used to be
                for (int i = y; i <= oldBottomRightCorner[1]; i++)
                    result.board[x, i] = '.';
            }
            else
            {
                for (int i = x; i <= oldBottomRightCorner[0]; i++)
                    result.board[i, y] = '.';
            }

            //Draw the car on it's new spot. :)
            x = newTopLeftCorner[0];
            y = newTopLeftCorner[1];

            if (goesVertical)
            {
                for(int i = y; i <= newBottomRightCorner[1]; i++)
                    result.board[x, i] = car;
            }
            else
            {
                for (int i = x; i <= newBottomRightCorner[0]; i++)
                    result.board[i, y] = car;
            }
            return board;
        }

        #region Helpers for the Hash Table
        private bool isVisitedState(Board newState)
        {
            bool test = HashTable.ContainsKey(newState.Hash());
            return test;
        }
        #endregion
    }
}

