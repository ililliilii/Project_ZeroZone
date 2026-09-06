using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Object_Item;
public interface IObjectItem
{
    Item_Information ClickItem();
}
public class Object_Item : MonoBehaviour, IObjectItem
{
    // Start is called before the first frame update
    
        [Header("아이템")]
        public Item_Information item;
        [Header("아이템 이미지")]
        public SpriteRenderer itemImage;

        void Start()
        {
            itemImage.sprite = item.itemImage;
        }
        public Item_Information ClickItem()
        {
            return this.item;
        }
  }
