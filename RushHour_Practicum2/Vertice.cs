using System;
using System.Collections.Generic;
using System.Text;

namespace RushHour_Practicum2
{
    public class Vertice
    {
        public Board state;
        public Vertice parent;
        public string lastMove;
        public int level;

        public Vertice(Board board, string lastmove, int level)
        {
            this.state = board;
            this.lastMove = lastmove;
            this.level = level;
        }

        public string movesToRoot()
        {
            StringBuilder sb = new StringBuilder(); 
            Vertice node = this;
            while (node.parent != null) 
            {
                sb.Insert(0, node.lastMove + ", ");
                node = node.parent;
            }
            sb.Remove(sb.Length-2,2);

            return sb.ToString();
        }

        public int countToRoot()
        {
            Vertice node = this;
            int i = 0;
            while (node.parent != null)
            {
                i++;
                node = node.parent;
            }

            return i;
            
        }

        public override string ToString()
        {
            return this.state.ToString();
        }
    }
}
