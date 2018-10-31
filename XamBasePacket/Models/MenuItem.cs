using System;
using Prism.Mvvm;
using Xamarin.Forms;

namespace XamBasePacket.Models
{
    public class MenuItem : BindableBase
    {
        public string Text { get; set; }
        public string SelectedIcon { get; set; }
        public Color SelectedTextColor { get; set; }
        public Color DisabledTextColor { get; set; }

        private Color _normalTextColor;
        public Color NormalTextColor
        {
            get => _normalTextColor;
            set => SetProperty(ref _normalTextColor, value);
        }

        private Color _textColor;
        public Color TextColor
        {
            get => _textColor;
            set => SetProperty(ref _textColor, value);
        }

        private string _normalIcon;
        public string NormalIcon
        {
            get => _normalIcon;
            set
            {
                SetProperty(ref _normalIcon, value);
                OnIsSelectedChanged();
            }
        }

        public Action NavigationCallback { get; set; }
        public string NavigationPath { get; set; }

        private string _displayIcon;

        public string DisplayIcon
        {
            get => _displayIcon;
            private set => SetProperty(ref _displayIcon, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value, OnIsSelectedChanged);
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value, OnIsEnabledChanged);
        }

        private void OnIsEnabledChanged()
        {

        }


        private void OnIsSelectedChanged()
        {
            DisplayIcon = IsSelected ? SelectedIcon : NormalIcon;
            TextColor = IsSelected ? SelectedTextColor : NormalTextColor;
        }
    }
}
