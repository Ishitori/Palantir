namespace Ix.Palantir.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SeparatedStringBuilder
    {
        private readonly StringBuilder stringBuilder;
        private readonly bool encodeValues;
        private string separator;

        public SeparatedStringBuilder(string separator = ",")
            : this(separator, string.Empty)
        {
        }
        public SeparatedStringBuilder(IEnumerable<int> intList)
            : this(", ", intList.Select(i => i.ToString()), false)
        {
        }
        public SeparatedStringBuilder(IEnumerable<string> stringList)
            : this(", ", stringList, false)
        {
        }
        public SeparatedStringBuilder(string separator, IEnumerable objectList)
            : this(separator)
        {
            foreach (var item in objectList)
            {
                this.AppendWithSeparator(item.ToString());
            }
        }
        public SeparatedStringBuilder(string separator, IEnumerable<string> stringList)
            : this(separator, stringList, false)
        {
        }
        public SeparatedStringBuilder(string separator, IEnumerable<string> stringList, bool encodeValues)
            : this(separator)
        {
            this.encodeValues = encodeValues;

            foreach (string item in stringList)
            {
                this.AppendWithSeparator(item);
            }
        }
        public SeparatedStringBuilder(string separator, string initialValue)
        {
            this.separator = separator;

            initialValue = this.RemoveSeparatorsOnStart(initialValue);
            initialValue = this.RemoveSeparatorsOnEnd(initialValue);

            this.stringBuilder = new StringBuilder(initialValue);
        }

        public string Separator
        {
            get
            {
                return this.separator;
            }

            set
            {
                this.separator = value;
            }
        }

        public int Length
        {
            get
            {
                return this.stringBuilder.Length;
            }
            set
            {
                this.stringBuilder.Length = value;
            }
        }

        public void AppendWithSeparator(string value)
        {
            if (this.stringBuilder.Length > 0)
            {
                this.stringBuilder.Append(this.separator);
            }

            this.stringBuilder.Append(this.encodeValues ? value : value);
        }
        public void AppendFormatWithSeparator(string value, params object[] args)
        {
            if (this.stringBuilder.Length > 0)
            {
                this.stringBuilder.Append(this.separator);
            }

            this.stringBuilder.AppendFormat(value, args);
        }

        public override string ToString()
        {
            return this.stringBuilder.ToString();
        }

        private string RemoveSeparatorsOnStart(string initialValue)
        {
            if (string.IsNullOrEmpty(initialValue))
            {
                return initialValue;
            }

            while (initialValue.StartsWith(this.separator, StringComparison.InvariantCultureIgnoreCase))
            {
                initialValue = initialValue.Substring(this.separator.Length, initialValue.Length - this.separator.Length);
            }

            return initialValue;
        }
        private string RemoveSeparatorsOnEnd(string initialValue)
        {
            if (string.IsNullOrEmpty(initialValue))
            {
                return initialValue;
            }

            while (initialValue.EndsWith(this.separator, StringComparison.InvariantCultureIgnoreCase))
            {
                initialValue = initialValue.Substring(0, initialValue.Length - this.separator.Length);
            }

            return initialValue;
        }
    }
}