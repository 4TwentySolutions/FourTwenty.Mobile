using Prism.Mvvm;

namespace XamBasePacket.Models
{
    public class SelectableItem<T> : BindableBase
    {
        public SelectableItem(T item)
        {
            Item = item;
        }

        public SelectableItem()
        {

        }

        private T _item;
        public T Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

    }
}
