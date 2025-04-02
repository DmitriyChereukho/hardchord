using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Grid : MonoBehaviour
    {
        public ShapeStorage shapeStorage;
        public int columns;
        public int rows;
        public float squaresGap = 0f;
        public GameObject gridSquare;
        public Vector2 startPosition = new(0.0f, 0.0f);
        public float squareScale = 0.5f;
        public float everySquareOffset = 0;

        private Vector2 _offset = new(0.0f, 0.0f);
        private readonly List<GameObject> _gridSquares = new();

        private void OnEnable()
        {
            GameEvents.checkIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        }

        private void OnDisable()
        {
            GameEvents.checkIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        }
        
        public List<GameObject> GetGridSquares()
        {
            return _gridSquares;
        }

        private void CheckIfShapeCanBePlaced(Shape shape)
        {
            var squareIndexes = new List<int>();
            var occupiedIndexes = new HashSet<int>();

            foreach (var square in _gridSquares)
            {
                var component = square.GetComponent<GridSquare>();

                if (component.SquareOccupied)
                {
                    occupiedIndexes.Add(component.SquareIndex);
                }
            }

            foreach (var square in _gridSquares)
            {
                var component = square.GetComponent<GridSquare>();

                if (component.Selected && !component.SquareOccupied)
                {
                    squareIndexes.Add(component.SquareIndex);
                    component.Selected = false;
                }
            }

            // Проверяем, можно ли разместить фигуру
            if (shape.TotalSquareNumber == squareIndexes.Count &&
                !squareIndexes.Exists(index => occupiedIndexes.Contains(index)))
            {
                foreach (var squareIndex in squareIndexes)
                {
                    var squareComponent = _gridSquares[squareIndex].GetComponent<GridSquare>();
                    squareComponent.PlaceShapeOnBoard(shape); // Передаем фигуру
                    squareComponent.SquareOccupied = true;
                }
                
                shape.SetShapeInactive();
                
                // Воспроизводим звук
                if (shape.audioSource != null && shape.audioSource.clip != null)
                {
                    shape.audioSource.PlayOneShot(shape.audioSource.clip);
                }
                
                GameEvents.requestNewShapes(shape);
            }
            else
            {
                GameEvents.moveShapeToStartPosition(shape);
            }
        }

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
            var columnNumber = 0;
            var rowNumber = 0;
            var squareRect = _gridSquares[0].GetComponent<RectTransform>();

            _offset.x = squareRect.rect.width * squareScale + everySquareOffset;
            _offset.y = squareRect.rect.height * squareScale + everySquareOffset;

            foreach (var square in _gridSquares)
            {
                if (columnNumber >= columns)
                {
                    columnNumber = 0;
                    rowNumber++;
                }

                var posXOffset = _offset.x * columnNumber;
                var posYOffset = _offset.y * rowNumber;

                square.GetComponent<RectTransform>().localPosition = new Vector3(
                    startPosition.x + posXOffset,
                    startPosition.y - posYOffset,
                    0.0f
                );

                columnNumber++;
            }
        }

        private void SpawnGridSquares()
        {
            var squareIndex = 0;

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    _gridSquares.Add(Instantiate(gridSquare));
                    _gridSquares[^1].GetComponent<GridSquare>().SquareIndex = squareIndex;
                    _gridSquares[^1].transform.SetParent(transform);
                    _gridSquares[^1].transform.localScale = new Vector3(squareScale, squareScale, squareScale);
                    _gridSquares[^1].GetComponent<GridSquare>().SetImage(squareIndex % 2 == 0);
                    squareIndex++;
                }
            }
        }
    }
}
