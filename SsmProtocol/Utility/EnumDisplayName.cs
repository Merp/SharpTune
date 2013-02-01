using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NateW.Ssm
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; private set; }

        public DisplayNameAttribute(string name)
        {
            this.DisplayName = name;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes")]
    class ParameterIdAttribute : DisplayNameAttribute
    {
        public ParameterIdAttribute(string name) : base(name) { }
    }

    class EnumDisplayName
    {
        private static Dictionary<Type, Dictionary<object, string>> types = new Dictionary<Type, Dictionary<object, string>>();

        public static string GetName(Type attributeType, Enum e)
        {
            if (!types.ContainsKey(attributeType))
            {
                types[attributeType] = new Dictionary<object, string>();
            }

            Dictionary<object, string> names = types[attributeType];

            if (names.ContainsKey(e))
            {
                return names[e];
            }

            string displayName = null;

            MemberInfo[] memberInfo = e.GetType().GetMember(e.ToString());
            if ((memberInfo != null) && (memberInfo.Length > 0))
            {
                object[] attributes = memberInfo[0].GetCustomAttributes(attributeType, false);

                if ((attributes != null) && (attributes.Length > 0))
                {
                    displayName = ((DisplayNameAttribute)attributes[0]).DisplayName;
                }
            }

            names[e] = displayName;
            return names[e];
        }
    }
}
