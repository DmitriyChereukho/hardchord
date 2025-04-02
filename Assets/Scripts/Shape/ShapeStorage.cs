using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = Random.Range(0, shapeData.Count);
            shape.CreateShape(shapeData[shapeIndex]);
        }
    }
    
    private void OnEnable()
    {
        GameEvents.requestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.requestNewShapes -= RequestNewShapes;
    }

    private void RequestNewShapes(Shape placedShape)
    {
        Shape newShape = Instantiate(placedShape, placedShape.transform.parent);
        newShape._startPosition = placedShape._startPosition;
        shapeList.Remove(placedShape);
        shapeList.Add(newShape);
        foreach (var shape in shapeList)
        {
            var shapeIndex = Random.Range(0, shapeData.Count);
            shape.RequestNewShape(shapeData[shapeIndex]);
        }
    }
}
