using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class ShapeData : ScriptableObject
{
    [System.Serializable]
    public class Row
    {
        public bool[] column;
        private int _size;

        public Row()
        {
        }

        public Row(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (var i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int columns;

    public int rows;

    public Row[] Board;
    
    public void Clear()
    {
        for (var i = 0; i < rows; i++)
        {
            Board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        Board = new Row[rows];

        for (var i = 0; i < rows; i++)
        {
            Board[i] = new Row(columns);
        }
    }
}