using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Mamesaver.Models.Extensions
{
    /// <summary>
    /// Provide metadata about enumerations
    /// </summary>
    public class EnumerationExtension : MarkupExtension
    {
        private Type _enumType;
        
        public EnumerationExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }

        public Type EnumType
        {
            get => _enumType;
            private set
            {
                if (_enumType == value) return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;
                if (enumType.IsEnum == false) throw new ArgumentException("Type must be an Enum");

                _enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
                from object enumValue in enumValues
                select new EnumerationMember
                {
                    Value = enumValue,
                    Description = GetDescription(enumValue)
                }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = EnumType
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute != null
                ? descriptionAttribute.Description
                : enumValue.ToString();
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}
