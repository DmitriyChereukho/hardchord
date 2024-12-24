using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class Grid : MonoBehaviour
    {
        public int columns = 0;
        public int rows = 0;
        public float squaresGap = 0.1f;
        public GameObject gridSquare;
        public Vector2 startPosition = new Vector2(0.0f, 0.0f);
        public float squareScale = 0.5f;
        public float everySquareOffset = 0.0f;
        
        private Vector2 _offset = new Vector2(0.0f, 0.0f);
        private List<GameObject> _gridSquares = new List<GameObject>();

        private void Start()
        {
            CreateGrid();
        }

        private void CreateGrid()
        {
            SpawnGridSquares();
            SetGridSquaresPositions();
        }

        private void SetGridSquaresPositions()
        {
            int column_number = 0;
            int row_number = 0;
            Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
            bool row_moved = false;
            
            var square_rect = _gridSquares[0].GetComponent<RectTransform>();
            
            _offset.x = square_rect.rect.width + everySquareOffset + squaresGap;
            _offset.y = square_rect.rect.height + everySquareOffset + squaresGap;

            foreach (GameObject gridSquare in _gridSquares)
            {
                if (column_number >= columns)
                {
                    column_number = 0;
                    row_number++;
                }

                var pos_x_offset = _offset.x * column_number;
                var pos_y_offset = _offset.y * row_number;

                gridSquare.GetComponent<RectTransform>().localPosition = new Vector3(
                    startPosition.x + pos_x_offset,
                    startPosition.y - pos_y_offset,
                    0.0f
                );

                column_number++;
            }
        }

        private void SpawnGridSquares()
        {
            int square_index = 0;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    _gridSquares.Add(Instantiate(gridSquare));
                    _gridSquares[^1].transform.SetParent(transform);
                    _gridSquares[^1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                    _gridSquares[^1].GetComponent<GridSquare>().SetImage(square_index % 2 == 0);
                    square_index++;
                }
            }
        }
    }
}
