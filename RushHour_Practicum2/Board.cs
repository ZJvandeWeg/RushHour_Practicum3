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
			this.height = height;
			this.width = width;
			this.board = new char[this.width, this.height];

			for (int i = 0; i < this.height; i++) {
				for (int i2 = 0; i2 < this.width; i2++) {
					this.board [i2, i] = lines [i] [i2];
				}
			}
		}

        public Board(Board newState)
        {
            this.board = newState.board;
            this.width = newState.width;
            this.height = newState.height;
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
	}
}
