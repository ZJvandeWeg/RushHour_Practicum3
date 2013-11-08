using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace RushHour_Practicum2
{
	public class Solver
	{
        int boardWidth;
        int boardHeight;
        int xTarget;
        int yTarget;
        int outputMode;
        Vertice root;
        Vertice solvedVertice;
        Hashtable syncHT;
        isSolved mt_isSolved = new isSolved(false);
        bool lengthCheckX;
        bool lengthCheckY;
        Vertice solvedGame = null;
        //ConcurrentQueue<Vertice> queue;

        public Solver (Board rootBoard, int xTarget, int yTarget, int outputMode)
        {
            boardWidth = rootBoard.width;
            boardHeight = rootBoard.height;
            this.xTarget = xTarget;
            this.yTarget = yTarget;
            this.outputMode = outputMode;

            Hashtable HashTable = new Hashtable();
            root = new Vertice(rootBoard, null);
            solvedVertice = null;

            HashTable.Add(rootBoard.Hash(), rootBoard);
            lengthCheckX = (xTarget - 1) >= 0;
            lengthCheckY = (yTarget - 1) >= 0;
            syncHT = Hashtable.Synchronized(HashTable);
            //queue = new ConcurrentQueue<Vertice>();
            //queue.Enqueue(root);
            solve(root);
		}

        public void solve(Vertice vertice)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(checkBoard), vertice);

            //Na alle Treads, of als ze allemaal gekilld zijn.
            if (mt_isSolved.b)
            {
                if (outputMode == 0)
                    Console.WriteLine(solvedVertice.countToRoot());
                else
                    Console.WriteLine(solvedVertice.movesToRoot());
            }
            else
                Console.WriteLine("Geen oplossing gevonden");
        }

        public void checkBoard(object o)
        {
            Vertice board = (Vertice) o;
            if (mt_isSolved.b)
                return;

            List<Vertice> moves = new List<Vertice>(allPossibleMoves(board.state));

            foreach (Vertice v in moves)
            {
                if (v.state.board[this.xTarget, this.yTarget] == 'x')
                {
                    //Vertical + other lengths
                    bool solveX = false;
                    if (lengthCheckX)
                        solveX = (v.state.board[xTarget - 1, yTarget] != 'x') &&
                            (v.state.board[xTarget + 1, yTarget] == 'x');
                    else
                        solveX = (v.state.board[xTarget + 1, yTarget] == 'x');

                    bool solveY = false;
                    if (lengthCheckY)
                        solveY = (v.state.board[xTarget, yTarget - 1] != 'x') &&
                            (v.state.board[xTarget, yTarget + 1] == 'x');
                    else
                        solveX = (v.state.board[xTarget, yTarget + 1] == 'x');

                    if ((v.state.board[xTarget, yTarget] == 'x') &&
                        (solveX || solveY))
                    {
                        lock (mt_isSolved)
                        {
                            mt_isSolved.b = true;
                        }
                    }
                    if (mt_isSolved.b)
                        this.solvedVertice = v;

                }

                if (mt_isSolved.b)
                    break;

                //dequeue.AddChild(v);
                ThreadPool.QueueUserWorkItem(new WaitCallback(checkBoard), v);
                // queue.Enqueue(v);

            }
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
            for (int y = 0; y < this.boardHeight; y++)
                for (int x = 0; x < this.boardWidth; x++)
                {
                    char car = currentState.board[x, y];

                    //Is there a car at the current spot.
                    //Or have we cheched this car already?
                    if (car == '.' || checkedCars.Contains(car))
                        continue;

                    //Console.WriteLine("Checking: " + car);
                    //What direction does the car go? NS || WE

                    //Console.WriteLine("x = " + x + " car = " + car + currentState.board[x + 1, y]);
                    if (x+1 < this.boardWidth && currentState.board[x+1,y] == car)
                    {
                        //Console.WriteLine("Horizontal");
                        //Horizontal moving
                        //Grab position of the car
                        int[] oldTopLeftCorner = {x,y};

                        int endCar = x + 1;
                        while(endCar + 1 < this.boardWidth && currentState.board[endCar + 1, y] == car)
                            endCar++;
                        int[] oldBottomRightCorner = { endCar, y };

                        //Scan left and right for open position, for each position open
                        //we create a new vertice.
                        // Xy
                        for (int i = x - 1; i >= 0; i--)
                        {
                            //Console.WriteLine("Horizontal-LEFT");
                            if (currentState.board[i, y] != '.')
                                break;
                            
                            int stepstaken = oldTopLeftCorner[0] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = {i, y};
                            int[] newBottomRightCorner = { endCar - stepstaken, y};

                            Board newBoard = createNewState(currentState, car, 
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            lock (syncHT.SyncRoot)
                            {
                            if (!isVisitedState(newBoard))
                            {
                                Vertice vResult = new Vertice(newBoard, car + "l" + stepstaken);
                                //Console.WriteLine(vResult);
                                result.Add(vResult);
                                
                                    syncHT.Add(newBoard.Hash(), newBoard);
                                }
                            }
                        }

                        //Now scan right of the car
                        for (int i = endCar + 1; i < this.boardWidth; i++)
                        {
                            //Console.WriteLine("Horizontal-RIGHT");
                            if (currentState.board[i, y] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[0];

                            int[] newTopLeftCorner = { x + stepstaken, y };
                            int[] newBottomRightCorner = { endCar + stepstaken, y };
                            //Console.WriteLine("" + newTopLeftCorner[0] + newTopLeftCorner[1]);
                            //Console.WriteLine("" + newBottomRightCorner[0] + newBottomRightCorner[1]);

                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);

                            lock (syncHT.SyncRoot)
                                {
                            if (!isVisitedState(newBoard))
                            {
                                Vertice vResult = new Vertice(newBoard, car + "r" + stepstaken);
                                //Console.WriteLine(vResult);
                                result.Add(vResult); 
                                syncHT.Add(newBoard.Hash(), newBoard);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Console.WriteLine("Vertical");
                        //Set the old positions
                        int[] oldTopLeftCorner = { x, y };

                        int endCar = y + 1;
                        while (endCar + 1 < this.boardHeight && currentState.board[x, endCar + 1] == car)
                            endCar++;
                        int[] oldBottomRightCorner = { x, endCar };

                        //Console.WriteLine("" + oldTopLeftCorner[0] + oldTopLeftCorner[1]);
                        //Console.WriteLine("" + oldBottomRightCorner[0] + oldBottomRightCorner[1]);

                        //Now look up and down for the all moves
                        //Looking up here
                        for (int i = y - 1; i >= 0; i--)
                        {
                            //Console.WriteLine("Vertical-UP");
                            //Console.WriteLine(currentState);
                            //Console.WriteLine("Vertical-UP");
                            if (currentState.board[x, i] != '.')
                                break;

                            int stepstaken = oldTopLeftCorner[1] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = { x, i };
                            int[] newBottomRightCorner = { x, endCar - stepstaken };

                            //Console.WriteLine("" + newTopLeftCorner[0] + newTopLeftCorner[1]);
                            //Console.WriteLine("" + newBottomRightCorner[0] + newBottomRightCorner[1]);

                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);

                            //Console.WriteLine(newBoard);
                            lock (syncHT.SyncRoot)
                            {
                                if (!isVisitedState(newBoard))
                                {
                                    Vertice vResult = new Vertice(newBoard, car + "u" + stepstaken);
                                    //Console.WriteLine(vResult);
                                    result.Add(vResult);
                                    syncHT.Add(newBoard.Hash(), newBoard);
                                }
                            }
                            //Console.WriteLine("Vertical-UP");
                            //Console.WriteLine(currentState);
                            //Console.WriteLine("Vertical-UP");
                        }

                        //Now scan below the car
                        for (int i = endCar + 1; i < this.boardHeight; i++)
                        {
                            //Console.WriteLine("Vertical-DOWN");
                            //Console.WriteLine("Vertical-DOWN: " + i);
                            //Console.WriteLine("Vertical-DOWN: " + currentState.board[x, i]);
                            if (currentState.board[x, i] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[1];

                            int[] newTopLeftCorner = { x, y + stepstaken };
                            int[] newBottomRightCorner = { x, endCar + stepstaken };

                            //Console.WriteLine("" + newTopLeftCorner[0] + newTopLeftCorner[1]);
                            //Console.WriteLine("" + newBottomRightCorner[0] + newBottomRightCorner[1]);

                            Board newBoard = createNewState(currentState, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            lock (syncHT.SyncRoot)
                                {
                                    if (!isVisitedState(newBoard))
                                    {
                                        //Console.WriteLine("New Board added");
                                        Vertice vResult = new Vertice(newBoard, car + "d" + stepstaken);
                                        //Console.WriteLine(vResult);
                                        result.Add(vResult);

                                        syncHT.Add(newBoard.Hash(), newBoard);
                                    }
                            }
                        }
                    }
                    //Console.ReadKey();
                    //Console.WriteLine("");
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
            Board result = new Board(board);
            //Console.WriteLine(result);
            int x = oldTopLeftCorner[0];
            int y = oldTopLeftCorner[1];

            //Remove the old car
            //Do we move NS || WE
            bool goesVertical = (x == oldBottomRightCorner[0]);
            
            if (goesVertical)
            {
                // HORI +1
                //empty the spot the car used to be
                for (int i = y; i < oldBottomRightCorner[1] + 1; i++)
                    result.board[x, i] = '.';
            }
            else
            {
                for (int i = x; i < oldBottomRightCorner[0] + 1; i++)
                    result.board[i, y] = '.';
            }
            
            //Draw the car on it's new spot. :)
            x = newTopLeftCorner[0];
            y = newTopLeftCorner[1];

            if (goesVertical)
            {
                for(int i = y; i < newBottomRightCorner[1] + 1; i++)
                    result.board[x, i] = car;
            }
            else
            {
                for (int i = x; i < newBottomRightCorner[0] + 1; i++)
                    result.board[i, y] = car;
            }

            return result;
        }

        #region Helpers for the Hash Table
        private bool isVisitedState(Board newState)
        {
            return syncHT.ContainsKey(newState.Hash());
        }
        #endregion
    }

    class isSolved
    {
        public bool b;
        public isSolved(bool b)
        {
            this.b = b;
        }
    }
}

