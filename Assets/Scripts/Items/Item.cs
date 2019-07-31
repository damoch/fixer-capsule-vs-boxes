using UnityEngine;

namespace Assets.Scripts.Items
{
    public class Item : MonoBehaviour
    {
        #region Private variables
        [SerializeField]
        private string _name;
        #endregion

        #region Accessors
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }
        #endregion
    }
}
