using System;

namespace RushHour_Practicum2
{
	public class Board
	{
		public int height;
		public int width;
		private char[,] board;
		public Board (int width, int height, string[] lines)
		{
			this.height = height;
			this.width = width;
			this.board = new char[this.width, this.height];

			for (int i = 0; i < this.height; i++) {
				for (int i2 = 0; i2 < this.width; i2++) {
					this.board [i, i2] = lines [i] [i2];
				}
			}
		}
	}
}
