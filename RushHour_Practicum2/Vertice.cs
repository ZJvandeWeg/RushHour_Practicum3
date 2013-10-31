using System;
using System.Collections.Generic;

namespace RushHour_Practicum2
{
    public class Vertice
    {
        public Board state;
        public Vertice parent;
        public string lastMove;
        public List<Vertice> children = new List<Vertice>();

        public Vertice(Board board, string lastmove)
        {
            this.state = board;
            this.lastMove = lastmove;
        }

        public void AddChild(Board newState, string lastMove)
        {
            Vertice child = new Vertice(newState, lastMove);
            child.parent = this;
            children.Add(child);
        }

        public string movesToRoot()
        {
            string moves = "";
            Vertice node = this;
            while (node.parent != null)
            {
                moves = this.lastMove + "," + moves;
                node = node.parent;
            }

            return moves;
        }
    }
}
