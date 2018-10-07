/**
 * Licensed under The MIT License
 * Redistributions of files must retain the above copyright notice.
 */

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Mamesaver.Config.Models
{
    [Serializable, XmlRoot("game")]
    public class Game  : INotifyPropertyChanged
    {
        private bool _selected = true;
        public string Category { get; set; }

        public Game()
        {
        }

        public Game(string name, string description, string year, string manufacturer, string rotation, string category = null)
        {
            Category = category;
            Name = name;
            Description = description;
            Year = year;
            Manufacturer = manufacturer;
            Rotation = rotation;
        }

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

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("year")]
        public string Year { get; set; }

        [XmlAttribute("manufacturer")]
        public string Manufacturer { get; set; }

        [XmlAttribute("rotation")]
        public string Rotation { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}