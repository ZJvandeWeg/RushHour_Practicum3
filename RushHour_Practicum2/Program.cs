using System;
//RushHour Solver Practicum opdracht 3
//3689840Jasper Modderaar
//3717259 ZJ van de Weg	

//Het zoeken van de oplossing voor het gegeven bord gaat bruteforce. 
//De datastructuur die dit ondersteund is de breadth-First-Search, in combinatie met een hashtable om te kijken of een 'state' al geweest is.
//De eerste bottleneck die we tegenkwamen, was de hashtable aangezien we een md5 hadden gebruikt. Dit bleek langzaam dus nu gebruiken we het bord zelf aangezien het altijd uniek is.
//Verder wordt elke Vertice parallel uitgerekend. Hierdoor zullen de eerste levels niet optimaal berekend worden, maar aangezien het aantal borden per move sterk groeit is dit later juist een goed idee.

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

            //int amountOfRuns = 10;
            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //sw.Start();
            //for (int i = 0; i < amountOfRuns; i++)
            //{
                new Solver(board, xTarget, yTarget, outputMode);    
            //}
            //sw.Stop();
            //Console.WriteLine("On average time in ms: " + sw.ElapsedMilliseconds / amountOfRuns);
            Console.ReadKey();
        }
	}
}
