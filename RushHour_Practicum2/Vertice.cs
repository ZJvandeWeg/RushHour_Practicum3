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
        public List<Vertice> children = new List<Vertice>();

        public Vertice(Board board, string lastmove)
        {
            this.state = board;
            this.lastMove = lastmove;
        }

        //public void AddChild(Vertice v)
        //{
        //    v.parent = this;
        //    v.level = this.level + 1;
        //    children.Add(v);
        //}

        //public void AddChild(Board newState, string lastMove)
        //{
        //    Vertice child = new Vertice(newState, lastMove);
        //    child.parent = this;
        //    child.level = this.level + 1;
        //    children.Add(child);
        //}

        public string movesToRoot()
        {
            StringBuilder sb = new StringBuilder(); 
            Vertice node = this;
            for(int i = 0; i < this.level; i++)
            {
                sb.Insert(0, node.lastMove + ", ");
                node = node.parent;
            }
            sb.Remove(sb.Length-2,2);

            return sb.ToString();
        }

        public int countToRoot()
        {
            return this.level;
        }

        public override string ToString()
        {
            return this.state.ToString();
        }
    }
}
