using System;

namespace RushHour_Practicum2
{
	class Program
	{
		public static void Main (string[] args)
		{
			int outputMode 	= int.Parse(Console.ReadLine ());
			int boardHeight = int.Parse(Console.ReadLine ());
			int yTarget = int.Parse (Console.ReadLine ());
			int xTarget = int.Parse (Console.ReadLine ());

			string[] lines = new string[boardHeight];
			for (int i = 0; i < boardHeight; i++) {
				lines [i] = Console.ReadLine ();
			}
            
			int boardWidth = lines [0].Length;
			Board board = new Board (boardWidth, boardHeight, lines);

            int amountOfRuns = 25;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (int i = 0; i < amountOfRuns; i++)
            {
                new Solver(board, xTarget, yTarget, outputMode);    
            }
            sw.Stop();
            Console.WriteLine("On average time in ms: " + sw.ElapsedMilliseconds / amountOfRuns);
            Console.ReadKey();
        }
	}
}
