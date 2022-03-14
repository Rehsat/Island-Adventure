using UnityEngine;
using System;

namespace PixelCrew.Model
{
    [Serializable]
    public class PlayerData 
    {
        [SerializeField] private InventoryData _inventory;
        public int Hp;

        public InventoryData Inventory => _inventory;
    }
}
