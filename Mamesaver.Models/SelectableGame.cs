/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Mamesaver.Models.Properties;

namespace Mamesaver.Models
{
    [Serializable, XmlRoot("SelectableGame")]
    public class SelectableGame : Game
    {
        private bool _selected;

    
        // FIXME this should be in a separate model
        // OR find a way to notify this has changed
        [XmlAttribute("selected")]
        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected) return;
                _selected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
