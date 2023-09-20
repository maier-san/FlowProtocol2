namespace FlowProtocol2.Helper
{
    public class Triplelist<T>
    {
        public T[,] Tabarray { get; private set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        public Triplelist(List<T> sourcelist)
        {
            int ecount = sourcelist.Count;
            Cols = 1;
            if (ecount > 10) Cols = 2;
            if (ecount > 20) Cols = 3;
            Rows = ecount / Cols;
            int rem = ecount % Cols;
            if (rem > 0) Rows++;
            int idx = 0;
            Tabarray = new T[Rows, Cols];
            foreach (T element in sourcelist)
            {                
                int col = idx/ Rows;
                int row = idx % Rows;
                Tabarray[row, col] = element;
                idx++;
            }
        }
    }
}