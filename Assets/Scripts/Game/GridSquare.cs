using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GridSquare : MonoBehaviour
    {
        public Image normalImage;
        public List<Sprite> normalImages;

        void Start()
        {
            
        }

        public void SetImage(bool setFirstImage)
        {
            normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
        }
    }
}
