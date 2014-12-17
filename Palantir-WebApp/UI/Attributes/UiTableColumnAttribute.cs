namespace Ix.Palantir.UI.Attributes
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Атрибут, обозначающий колонки таблицы UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UiTableColumnAttribute : Attribute
    {
        private readonly string displayName;
        private PropertyInfo nameProperty;
        private Type resourceType;

        public UiTableColumnAttribute(string displayName)
        {
            this.displayName = displayName;
            this.AutoNumeric = false;
        }

        /// <summary>
        /// Наименование колонки.
        /// </summary>
        public string DisplayName
        {
            get
            {
                // проверяет,nameProperty null и возвращает исходный значения отображаемого имени
                if (this.nameProperty == null)
                {
                    return this.displayName;
                }

                return (string)this.nameProperty.GetValue(this.nameProperty.DeclaringType, null);
            }
        }

        /// <summary>
        /// Тип ресурса для локализации.
        /// </summary>
        public Type ResourceType
        {
            get
            {
                return this.resourceType;
            }
            set
            {
                this.resourceType = value;

                // инициализация nameProperty, когда тип свойства устанавливается set'ром
                this.nameProperty = this.resourceType.GetProperty(this.displayName, BindingFlags.Static | BindingFlags.Public);
            }
        }

        /// <summary>
        /// Доступное направление сортировки по данному полю.
        /// </summary>
        public SortableBy Sortable { get; set; }

        /// <summary>
        /// Сортировка по-умолчанию.
        /// </summary>
        public SortableBy DefaultSorted { get; set; }

        /// <summary>
        /// Id Html элемента колонки.
        /// </summary>
        public string ElementId { get; set; }

        /// <summary>
        /// Автонумерация поля.
        /// </summary>
        public bool AutoNumeric { get; set; }
    }
}