using System;

namespace SharedLibrary
{
    public class CartCount
    {
        private int count;
        public int Count
        {
            get { return count; }
            set
            {
                if (count != value)
                {
                    count = value;
                    OnCountChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler OnCountChanged;
    }
}
