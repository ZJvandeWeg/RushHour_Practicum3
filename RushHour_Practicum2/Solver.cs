using System;
using System.Collections.Concurrent;

namespace RushHour_Practicum2
{
	public class Solver
	{
        Vertice root;
		public Solver (Board rootBoard, int xTarget, int yTarget)
		{
            root = new Vertice(rootBoard, null);
		}

        public bool duplicateState(Vertice root, Board newState)
        {
            if (newState == root.state) return true;

            BlockingCollection<Vertice> queue = new BlockingCollection<Vertice>();
            queue.Add(root);
            bool isFound = false;

            do
            {
                Vertice temp = queue.Take();
                if (checkChildren(temp, newState))
                    isFound = true;
                else
                    foreach (Vertice item in temp.children)
                        queue.Add(item);
            }
            while (queue.Count != 0 && !isFound);

            return isFound;
        }
        
        private bool checkChildren(Vertice vertice, Board newState)
        {
            foreach (Vertice v in vertice.children)
                if (v.state == newState)
                    return true;
            
            return false;
        }
	}
}

