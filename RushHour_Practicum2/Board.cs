using System;
using System.Security.Cryptography;
using System.Text;

namespace RushHour_Practicum2
{
	public class Board
	{
        public int height;
		public int width;
		public char[,] board;
		public Board (int width, int height, string[] lines)
		{
			this.width = width;
            this.height = height;
			this.board = new char[this.width, this.height];

            for (int y = 0; y < this.height; y++) {
				for (int x = 0; x < this.width; x++) {
					this.board [x, y] = lines [y] [x];
				}
			}
		}

        public Board(Board newState)
        {
            this.board = (char[,])newState.board.Clone();
            //this.board = newState.board;
            this.width = newState.width;
            this.height = newState.height;
        }

        public string Hash()
        {
            return this.ToString();
        }

        public void print()
        {
            string print = "";
            for(int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                    print += this.board[x, y];
                
                Console.WriteLine(print);
                print = "";
            }
            Console.WriteLine("\n");
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            //sb.Append("\n");
            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                    sb.Append(board[x, y]);
                sb.Append("\n");
            }
            sb.Remove(sb.Length-1,1);
            return sb.ToString();
        }
	}
}
