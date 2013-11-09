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
        int xTarget, yTarget, outputMode;
        bool lengthCheckX;
        bool lengthCheckY;

        List<int> statesAtLevel;
        List<int> doneAtLevel;
        CountdownEvent counter;
        Vertice SolvedVertice;
        Vertice root;
        Hashtable syncHT;
        //ConcurrentQueue<Vertice> queue;

        public Solver(Board rootBoard, int xTarget, int yTarget, int outputMode)
        {
            boardWidth = rootBoard.width;
            boardHeight = rootBoard.height;
            this.xTarget = xTarget;
            this.yTarget = yTarget;
            this.outputMode = outputMode;
            this.SolvedVertice = null;
            this.lengthCheckX = (xTarget - 1) >= 0;
            this.lengthCheckY = (yTarget - 1) >= 0;

            counter = new CountdownEvent(1);//Start board
            Hashtable HashTable = new Hashtable();
            root = new Vertice(rootBoard, null, 0);
            HashTable.Add(rootBoard.Hash(), rootBoard);
            syncHT = Hashtable.Synchronized(HashTable);
            statesAtLevel = new List<int>();
            doneAtLevel = new List<int>();
            statesAtLevel.Add(0);//Level 0
            statesAtLevel.Add(0);//State with 1 move
            doneAtLevel.Add(0);//Root level
            statesAtLevel[root.level]++;

            FirstBoard(rootBoard);
        }

        public void FirstBoard(Board b)
        {
            solve(root);
            counter.Signal();
            counter.Wait();//Wait for all the other threads

            if (SolvedVertice != null)
            {
                if (outputMode == 0)
                    Console.WriteLine(SolvedVertice.countToRoot());
                else
                    Console.WriteLine(SolvedVertice.movesToRoot());
            }
            else
                Console.WriteLine("Geen oplossing gevonden");
        }

        public void solve(Vertice vertice)
        {
            if (SolvedVertice != null)
                return;

            //Wait for the previous level to finish
            if(vertice.level != 0)
                while (statesAtLevel[vertice.level - 1] != doneAtLevel[vertice.level - 1])
                    ;

            List<Vertice> apm = new List<Vertice>(allPossibleMoves(vertice));
            foreach (Vertice v in apm)
            {
                //set the parent
                v.parent = vertice;
                if (winningBoard(v))
                {
                    SolvedVertice = v;
                    lock (doneAtLevel)
                        doneAtLevel[vertice.level]++;
                    return;
                }
                
                counter.AddCount();
                lock (statesAtLevel)
                {
                        statesAtLevel.Add(0);
                        statesAtLevel[v.level]++;
                }
                ThreadPool.QueueUserWorkItem((randomstringjustbecauseihavetotypesomething) => 
                    { 
                        solve(v); 
                        counter.Signal(); 
                    }
                    );
            }
            lock (doneAtLevel)
            {
                    doneAtLevel.Add(0);
                    doneAtLevel[vertice.level]++;
            }
        }
        private bool winningBoard(Vertice v)
        {
            if (v.state.board[xTarget, yTarget] == 'x')
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
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// For the state given, return all the possible moves.
        /// </summary>
        /// <param name="currentState">From which state to check</param>
        /// <returns>Unvisited states which can be got to in 1 move</returns>
        private List<Vertice> allPossibleMoves(Vertice vertice)
        {

            List<Vertice> result = new List<Vertice>();
            List<char> checkedCars = new List<char>();
            for (int y = 0; y < this.boardHeight; y++)
                for (int x = 0; x < this.boardWidth; x++)
                {
                    char car = vertice.state.board[x, y];

                    //Is there a car at the current spot.
                    //Or have we cheched this car already?
                    if (car == '.' || checkedCars.Contains(car))
                        continue;

                    //What direction does the car go? NS || WE

                    if (x + 1 < this.boardWidth && vertice.state.board[x + 1, y] == car)
                    {
                        //Console.WriteLine("Horizontal");
                        //Horizontal moving
                        //Grab position of the car
                        int[] oldTopLeftCorner = { x, y };

                        int endCar = x + 1;
                        while (endCar + 1 < this.boardWidth && vertice.state.board[endCar + 1, y] == car)
                            endCar++;
                        int[] oldBottomRightCorner = { endCar, y };

                        //Scan left and right for open position, for each position open
                        //we create a new vertice.
                        // Xy
                        for (int i = x - 1; i >= 0; i--)
                        {
                            //Console.WriteLine("Horizontal-LEFT");
                            if (vertice.state.board[i, y] != '.')
                                break;

                            int stepstaken = oldTopLeftCorner[0] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = { i, y };
                            int[] newBottomRightCorner = { endCar - stepstaken, y };

                            Board newBoard = createNewState(vertice.state, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            lock (syncHT)
                            {
                                if (!isVisitedState(newBoard))
                                {
                                    Vertice vResult = new Vertice(newBoard, car + "l" + stepstaken, vertice.level + 1);
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
                            if (vertice.state.board[i, y] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[0];

                            int[] newTopLeftCorner = { x + stepstaken, y };
                            int[] newBottomRightCorner = { endCar + stepstaken, y };

                            Board newBoard = createNewState(vertice.state, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);

                            lock (syncHT)
                            {
                                if (!isVisitedState(newBoard))
                                {
                                    Vertice vResult = new Vertice(newBoard, car + "r" + stepstaken, vertice.level +1);
                                    result.Add(vResult);
                                    syncHT.Add(newBoard.Hash(), newBoard);
                                }
                            }
                        }
                    }
                    else
                    {
                        int[] oldTopLeftCorner = { x, y };

                        int endCar = y + 1;
                        while (endCar + 1 < this.boardHeight && vertice.state.board[x, endCar + 1] == car)
                            endCar++;
                        int[] oldBottomRightCorner = { x, endCar };

                        //Now look up and down for the all moves
                        //Looking up here
                        for (int i = y - 1; i >= 0; i--)
                        {
                            if (vertice.state.board[x, i] != '.')
                                break;

                            int stepstaken = oldTopLeftCorner[1] - i;
                            //Create new Board 
                            int[] newTopLeftCorner = { x, i };
                            int[] newBottomRightCorner = { x, endCar - stepstaken };

                            Board newBoard = createNewState(vertice.state, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);

                            lock (syncHT)
                            {
                                if (!isVisitedState(newBoard))
                                {
                                    Vertice vResult = new Vertice(newBoard, car + "u" + stepstaken, vertice.level + 1);
                                    result.Add(vResult);
                                    syncHT.Add(newBoard.Hash(), newBoard);
                                }
                            }
                        }

                        //Now scan below the car
                        for (int i = endCar + 1; i < this.boardHeight; i++)
                        {
                            if (vertice.state.board[x, i] != '.')
                                break;

                            int stepstaken = i - oldBottomRightCorner[1];

                            int[] newTopLeftCorner = { x, y + stepstaken };
                            int[] newBottomRightCorner = { x, endCar + stepstaken };

                            Board newBoard = createNewState(vertice.state, car,
                                                    oldTopLeftCorner, oldBottomRightCorner,
                                                    newTopLeftCorner, newBottomRightCorner);
                            lock (syncHT)
                            {
                                if (!isVisitedState(newBoard))
                                {
                                    Vertice vResult = new Vertice(newBoard, car + "d" + stepstaken, vertice.level + 1);
                                    result.Add(vResult);
                                    syncHT.Add(newBoard.Hash(), newBoard);
                                }
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
            Board result = new Board(board);
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
                for (int i = y; i < newBottomRightCorner[1] + 1; i++)
                    result.board[x, i] = car;
            }
            else
            {
                for (int i = x; i < newBottomRightCorner[0] + 1; i++)
                    result.board[i, y] = car;
            }

            return result;
        }

        private bool isVisitedState(Board newState)
        {
            bool test = syncHT.ContainsKey(newState.Hash());
            return test;
        }
    }
}
