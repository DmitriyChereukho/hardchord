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
        
        void Start()
        {
            Selected = false;
            SquareOccupied = false;
        }

        public bool CanWeUseThisSquare()
        {
            return hooverImage.gameObject.activeSelf;
        }

        public void ActivateSquare()
        {
            hooverImage.gameObject.SetActive(false);
            activeImage.gameObject.SetActive(true);
            Selected = true;
            SquareOccupied = true;
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
        }

        public void PlaceShapeOnBoard()
        {
            ActivateSquare();
        }
    }
}
