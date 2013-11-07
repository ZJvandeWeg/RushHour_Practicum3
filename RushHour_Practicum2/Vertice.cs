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
        public List<Vertice> children = new List<Vertice>();

        public Vertice(Board board, string lastmove)
        {
            this.state = board;
            this.lastMove = lastmove;
        }

        public void AddChild(Vertice v)
        {
            v.parent = this;
            children.Add(v);
        }

        public void AddChild(Board newState, string lastMove)
        {
            Vertice child = new Vertice(newState, lastMove);
            child.parent = this;
            children.Add(child);
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

            //node = this;
            //sb.Append("\n");
            //sb.Append(node);
            //sb.Append("\n");
            //while (node.parent != null)
            //{
            //    node = node.parent;
            //    sb.Append(node);
            //    sb.Append("\n");
            //}

            return sb.ToString();
        }

        public int countToRoot()
        {
            Vertice node = this;
            int count = 0;
            while (node.parent != null)
            {
                count++;
                node = node.parent;
            }

            return count;
        }

        public override string ToString()
        {
            return this.state.ToString();
        }
    }
}
