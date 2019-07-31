using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Armor : Item
    {
        #region Private variables
        [SerializeField]
        private int _damageDecreaseValue;
        #endregion

        #region Accessors
        public int DamageDecreaseValue
        {
            get
            {
                return _damageDecreaseValue;
            }

            set
            {
                _damageDecreaseValue = value;
            }
        }
        #endregion
    }
}
