using System;
using System.Collections.Generic;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Grid = Game.Grid;
using Random = UnityEngine.Random;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler,
    IEndDragHandler,
    IPointerDownHandler
{
    [HideInInspector]
    public Color shapeColor;
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new(0f, 900f);
    public TextMeshProUGUI noteText;

    [HideInInspector] 
    public ShapeData CurrentShapeData;
    
    public bool IsPlaced { get; private set; } = false;
    public int TotalSquareNumber { get; set; }
    private List<GameObject> _currentShape = new();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    public Vector3 _startPosition;
    private Canvas _canvas;
    private bool _shapeActive = true;
    private NoteType _noteType;

    [HideInInspector]
    public AudioSource audioSource;

    public void Awake()
    {
        _shapeStartScale = GetComponent<RectTransform>().localScale;
        _transform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        
        audioSource = gameObject.AddComponent<AudioSource>();
        _startPosition = _transform.localPosition;
        _shapeActive = true;
    }

    private void OnEnable()
    {
        GameEvents.moveShapeToStartPosition += MoveShapeToStartPosition;
    }

    private void MoveShapeToStartPosition(Shape shape)
    {
        if (shape == this)
            _transform.transform.localPosition = _startPosition;
    }

    private void OnDisable()
    {
        GameEvents.moveShapeToStartPosition -= MoveShapeToStartPosition;
    }

    public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }

        _shapeActive = false;
    }
    
    public void SetShapeInactive()
    {
        if (IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }

    public void RemoveShape()
    {
        var grid = FindObjectOfType<Grid>();
        noteText.text = "";
        foreach (var square in grid.GetGridSquares())
        {
            var squareComponent = square.GetComponent<GridSquare>();

            // Проверяем, что клетка занята и что фигура, которая её занимает, совпадает с текущей
            if (squareComponent.SquareOccupied && squareComponent.OccupyingShape == this)
            {
                squareComponent.DeactivateSquare();
            }
        }
    }

    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }

        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        _transform.transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);
        (_noteType, noteText.text) = GetRandomNote();
        SetRandomLightColor();
        audioSource.clip = ChordDictionary.MusicFileByNote.GetValueOrDefault(noteText.text);
        
        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform));
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y);

        var currentIndexInList = 0;

        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.Board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition =
                        new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance),
                            GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    var image = _currentShape[currentIndexInList].GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                    {
                        image.color = shapeColor;
                    }
                    
                    currentIndexInList++;
                }
            }
        }
        
        noteText.transform.SetAsLastSibling();
    }
    
    private static (NoteType noteType, string noteName) GetRandomNote()
    {
        var random = new System.Random();
        var enumValues = Enum.GetValues(typeof(NoteType));
        var randomNoteType = (NoteType)enumValues.GetValue(random.Next(enumValues.Length));

        return (randomNoteType, randomNoteType.ToString()[0].ToString());
        //return randomNoteType.ToString().Contains("Sharp") 
        //    ? (randomNoteType, $"{randomNoteType.ToString()[0]}#") 
        //    : (randomNoteType, randomNoteType.ToString());
    }
    
    private void SetRandomLightColor()
    {
        var brightColors = new[]
        {
            new Color(1.0f, 0.6f, 0.6f),   // Красновато-розовый (pastel red)
            new Color(1.0f, 0.5f, 0.0f),   // Оранжевый
            new Color(1.0f, 1.0f, 0.2f),   // Ярко-жёлтый
            new Color(0.6f, 1.0f, 0.6f),   // Светло-зелёный
            new Color(0.2f, 1.0f, 0.2f),   // Ярко-зелёный
            new Color(0.0f, 0.6f, 1.0f),   // Голубой
            new Color(0.6f, 0.6f, 1.0f),   // Светло-синий (pastel blue)
            new Color(0.5f, 0.0f, 0.8f),   // Фиолетовый
            new Color(1.0f, 0.0f, 0.6f),   // Ярко-розовый
            new Color(1.0f, 0.8f, 0.0f),   // Золотисто-жёлтый
            new Color(1.0f, 0.0f, 1.0f),   // Магента
            new Color(0.0f, 1.0f, 1.0f)   // Бирюзовый
        };
        
        var index = Random.Range(0, brightColors.Length);
        shapeColor = brightColors[index];
    }
    
    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        var shiftOnY = 0f;
        var additionalGap = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleSquareIndex)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 : shapeData.rows / 2;
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    if (row == middleSquareIndex2)
                    {
                        shiftOnY = -moveDistance.y / 2;
                    }

                    if (row == middleSquareIndex1)
                    {
                        shiftOnY = moveDistance.y / 2;
                    }
                }

                if (row < middleSquareIndex1 && row < middleSquareIndex2)
                {
                    shiftOnY = moveDistance.y;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSquareIndex1 && row > middleSquareIndex2)
                {
                    shiftOnY = -moveDistance.y;
                    shiftOnY *= multiplier;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        var shiftOnX = 0f;
        var additionalGap = 5f;

        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex)
                {
                    shiftOnX = (moveDistance.x + additionalGap) * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex)
                {
                    shiftOnX = (moveDistance.x + additionalGap);
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : (shapeData.columns - 1);
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x / 2 + additionalGap;
                    }

                    if (column == middleSquareIndex1)
                    {
                        shiftOnX = moveDistance.x / -2 + additionalGap;
                    }
                }

                if (column < middleSquareIndex1 && column < middleSquareIndex2)
                {
                    shiftOnX = (moveDistance.x + additionalGap) * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2)
                {
                    shiftOnX = (moveDistance.x + additionalGap);
                    shiftOnX *= multiplier;
                }
            }
        }

        return shiftOnX;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        var number = 0;

        foreach (var rowData in shapeData.Board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = shapeSelectedScale;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            Camera.main,
            out var localPointerPosition
        );

        offset = _transform.localPosition - (Vector3)localPointerPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            Camera.main,
            out var localPointerPosition
        );

        _transform.localPosition = localPointerPosition + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.checkIfShapeCanBePlaced(this);

        // Проверяем соприкосновения с другими фигурами
        //if (CheckCollisionWithOtherShapes())
        //{
            RemoveCollidingShapes();
        //}
    }

    // Метод для проверки соприкосновений с другими фигурами
    private bool CheckCollisionWithOtherShapes()
    {
        var occupiedIndexes = new HashSet<int>();

        // Собираем индексы всех занятых клеток
        foreach (var otherShape in FindObjectsOfType<Shape>())
        {
            if (otherShape == this) continue;
            
            var otherShapeIndexes = otherShape.GetOccupiedCellIndexes();
            foreach (var index in otherShapeIndexes)
            {
                occupiedIndexes.Add(index);
            }
        }

        var thisShapeIndexes = GetOccupiedCellIndexes();

        // Теперь проверяем, есть ли у фигуры соседние клетки, которые соприкасаются по сторонам с клетками других фигур
        foreach (var index in thisShapeIndexes)
        {
            // Получаем соседей для каждой клетки
            var neighbors = GetNeighboringCellIndexes(index);

            // Если хотя бы один сосед из соседних клеток занят, то это столкновение
            foreach (var neighbor in neighbors)
            {
                if (occupiedIndexes.Contains(neighbor))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Метод для проверки соприкосновений с другими фигурами
    private bool CheckCollisionWithShape(Shape shape)
    {
        var occupiedIndexes = new HashSet<int>();


        if (shape != this)
        {
            var otherShapeIndexes = shape.GetOccupiedCellIndexes();
            foreach (var index in otherShapeIndexes)
            {
                occupiedIndexes.Add(index);
            }
        }

        var thisShapeIndexes = GetOccupiedCellIndexes();

        // Теперь проверяем, есть ли у фигуры соседние клетки, которые соприкасаются по сторонам с клетками других фигур
        foreach (var index in thisShapeIndexes)
        {
            // Получаем соседей для каждой клетки
            var neighbors = GetNeighboringCellIndexes(index);

            // Если хотя бы один сосед из соседних клеток занят, то это столкновение
            foreach (var neighbor in neighbors)
            {
                if (occupiedIndexes.Contains(neighbor))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Метод для проверки соприкосновений с другими фигурами
    private bool CheckCollisionBetweenShapes(Shape shape1, Shape shape2)
    {
        var occupiedIndexes = new HashSet<int>();

        var otherShapeIndexes1 = shape1.GetOccupiedCellIndexes();
        var otherShapeIndexes2 = shape2.GetOccupiedCellIndexes();
        foreach (var index in otherShapeIndexes2)
        {
            occupiedIndexes.Add(index);
        }

        // Теперь проверяем, есть ли у фигуры соседние клетки, которые соприкасаются по сторонам с клетками других фигур
        foreach (var index in otherShapeIndexes1)
        {
            // Получаем соседей для каждой клетки
            var neighbors = GetNeighboringCellIndexes(index);

            // Если хотя бы один сосед из соседних клеток занят, то это столкновение
            foreach (var neighbor in neighbors)
            {
                if (occupiedIndexes.Contains(neighbor))
                {
                    return true;
                }
            }
        }

        return false;
    }

// Метод для получения соседних клеток по сторонам
    private List<int> GetNeighboringCellIndexes(int index)
    {
        var neighbors = new List<int>();
        var grid = FindObjectOfType<Grid>();

        // Получаем координаты текущей клетки в сетке
        var row = index / grid.columns;
        var column = index % grid.columns;

        // Проверяем соседние клетки по сторонам (верх, низ, лево, право)
        // Вверх
        if (row > 0)
        {
            neighbors.Add(index - grid.columns);
        }

        // Вниз
        if (row < grid.rows - 1)
        {
            neighbors.Add(index + grid.columns);
        }

        // Влево
        if (column > 0)
        {
            neighbors.Add(index - 1);
        }

        // Вправо
        if (column < grid.columns - 1)
        {
            neighbors.Add(index + 1);
        }

        return neighbors;
    }

    private List<int> GetOccupiedCellIndexes()
    {
        var occupiedIndexes = new List<int>();

        // Получаем все клетки, которые эта фигура занимает
        var grid = FindObjectOfType<Grid>();
        foreach (var square in grid.GetGridSquares())
        {
            var squareComponent = square.GetComponent<GridSquare>();

            // Проверяем, что клетка занята и что фигура, которая её занимает, совпадает с текущей
            if (squareComponent.SquareOccupied && squareComponent.OccupyingShape == this)
            {
                occupiedIndexes.Add(squareComponent.SquareIndex);
            }
        }

        return occupiedIndexes;
    }

    // Метод для удаления, если они соприкасаются
    private void RemoveCollidingShapes()
    {
        // Деактивируем текущую фигуру

        // Деактивируем все пересекающиеся фигуры

        var combinations = GetCombinations(FindObjectsOfType<Shape>(), 3);

        foreach (var combination in combinations)
        {
            if (!combination.Contains(this))
            {
                continue;
            }

            if (CheckCollisionBetweenShapes(combination[0], combination[1]) &&
                CheckCollisionBetweenShapes(combination[1], combination[2]) &&
                CheckCollisionBetweenShapes(combination[0], combination[2]))
            {
                var isCorrectCombination = ChordDictionary.TryGetChordName(
                    combination[0].noteText.text,
                    combination[1].noteText.text,
                    combination[2].noteText.text, out var correctCombination);

                if (!isCorrectCombination) 
                    continue;
                
                foreach (var shape in combination)
                {
                    shape.RemoveShape();
                }
                
                GameEvents.onPointsEarned?.Invoke(1);
            
                RemoveShape();
            }
        }
    }

    public static List<List<T>> GetCombinations<T>(T[] array, int combinationSize)
    {
        var result = new List<List<T>>();
        GenerateCombinations(array, combinationSize, 0, new List<T>(), result);
        return result;
    }

    private static void GenerateCombinations<T>(T[] array, int combinationSize, int startIndex,
        List<T> currentCombination, List<List<T>> result)
    {
        // Когда размер комбинации достигнут, добавляем её в результат
        if (currentCombination.Count == combinationSize)
        {
            result.Add(new List<T>(currentCombination));
            return;
        }

        // Перебираем элементы массива, начиная с startIndex
        for (var i = startIndex; i < array.Length; i++)
        {
            currentCombination.Add(array[i]);
            GenerateCombinations(array, combinationSize, i + 1, currentCombination, result);
            currentCombination.RemoveAt(currentCombination.Count - 1); // Возвращаемся назад
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }
}