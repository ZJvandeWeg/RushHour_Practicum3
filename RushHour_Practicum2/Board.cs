using System;
using System.Security.Cryptography;
using System.Text;

namespace RushHour_Practicum2
{
	public class Board
	{
		public int width;
		public char[,] board;
		public Board (int width, int height, string[] lines)
		{
			this.width = width;
			this.board = new char[this.width, this.width];

			for (int y = 0; y < this.width; y++) {
				for (int x = 0; x < this.width; x++) {
					this.board [x, y] = lines [y] [x];
				}
			}
		}

        public Board(Board newState)
        {
            this.board = newState.board;
            this.width = newState.width;
        }

        public string Hash()
        {
            string stringboard = "";
            foreach (char c in this.board)
            {
                stringboard += c;
            }
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(stringboard);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public void print()
        {
            string print = "";
            for(int y = 0; y < this.width; y++)
            {
                for (int x = 0; x < this.width; x++)
                    print += this.board[x, y];
                
                Console.WriteLine(print);
                print = "";
            }
            Console.WriteLine("\n");
        }
	}
}
