using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NateW.Ssm
{
    public class PropertyDefinition
    {
        private string name;

        public PropertyDefinition(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.name;
        }
    }

    public class PropertyBag : IEnumerable<KeyValuePair<PropertyDefinition, object>>
    {
        private Dictionary<PropertyDefinition, object> properties;

        public PropertyBag()
        {
            this.properties = new Dictionary<PropertyDefinition, object>();
        }

        public object this[PropertyDefinition propertyDefinition]
        {
            get
            {
                return this.properties[propertyDefinition];
            }

            set
            {
                this.properties[propertyDefinition] = value;
            }
        }

        public void Add(PropertyBag source)
        {
            foreach (KeyValuePair<PropertyDefinition, object> pair in source)
            {
                this.properties.Add(pair.Key, pair.Value);
            }
        }

        public void Remove(PropertyBag source)
        {
            foreach (KeyValuePair<PropertyDefinition, object> kvp in source)
            {
                this.properties.Remove(kvp.Key);
            }
        }

        public IEnumerator<KeyValuePair<PropertyDefinition, object>> GetEnumerator()
        {
            return this.properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this.properties).GetEnumerator();
        }

        public bool TryGetValue(PropertyDefinition propertyDefinition, out object value)
        {
            return this.properties.TryGetValue(propertyDefinition, out value);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<PropertyDefinition, object> pair in properties)
            {
                builder.Append(pair.Key.ToString());
                builder.Append("=");
                builder.Append(pair.Value.ToString());
                builder.Append(", ");
            }
            return builder.ToString();
        }
    }
}
