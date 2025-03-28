using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GridSquare : MonoBehaviour
    {
        public Image hooverImage;
        public Image activeImage;
        public Image normalImage;
        public List<Sprite> normalImages;

        public bool Selected { get; set; }
        public int SquareIndex { get; set; }
        public bool SquareOccupied { get; set; }
        
        // Добавляем ссылку на фигуру, которая занимает эту клетку
        public Shape OccupyingShape { get; private set; }

        void Start()
        {
            Selected = false;
            SquareOccupied = false;
        }

        public bool CanWeUseThisSquare()
        {
            return hooverImage.gameObject.activeSelf;
        }

        public void ActivateSquare(Shape shape)
        {
            hooverImage.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(true);
            Selected = true;
            SquareOccupied = true;

            // Устанавливаем фигуру, которая заняла эту клетку
            OccupyingShape = shape;
        }

        public void DeactivateSquare()
        {
            
            hooverImage.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(false);
            Selected = false;
            SquareOccupied = false;

            // Снимаем занятость клетки
            OccupyingShape = null;
        }

        public void SetImage(bool setFirstImage)
        {
            normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (SquareOccupied == false)
            {
                Selected = true;
                hooverImage.gameObject.SetActive(true);
            }
            else if (collision.GetComponent<ShapeSquare>() != null)
            {
                collision.GetComponent<ShapeSquare>().SetOccupied();
            }
        }
        
        private void OnTriggerStay2D(Collider2D collision)
        {
            Selected = true;
            if (SquareOccupied == false)
            {
                hooverImage.gameObject.SetActive(true);
            }
            else if (collision.GetComponent<ShapeSquare>() != null)
            {
                collision.GetComponent<ShapeSquare>().SetOccupied();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (SquareOccupied == false)
            {
                Selected = false;
                hooverImage.gameObject.SetActive(false);
            }
            else if (collision.GetComponent<ShapeSquare>() != null)
            {
                collision.GetComponent<ShapeSquare>().UnSetOccupied();
            }
        }

        public void PlaceShapeOnBoard(Shape shape)
        {
            ActivateSquare(shape);
            activeImage.color = shape.shapeColor;
        }

        // Новый метод для снятия занятости клетки
        public void UnSetOccupied()
        {
            SquareOccupied = false;
            OccupyingShape = null;  // Убираем ссылку на фигуру
        }
    }
}
