using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    // Изменяем событие, чтобы оно принимало параметр типа Shape
    public static Action<Shape> checkIfShapeCanBePlaced;

    // Оставляем без изменений, так как оно не требует параметров
    public static Action<Shape> moveShapeToStartPosition;

    public static Action<Shape> requestNewShapes;

    public static Action setShapeInactive;
}
