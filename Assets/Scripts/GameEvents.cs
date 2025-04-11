using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action<Shape> checkIfShapeCanBePlaced;
    public static Action<Shape> moveShapeToStartPosition;
    public static Action<Shape> requestNewShapes;
    public static Action<int> onPointsEarned;
}
