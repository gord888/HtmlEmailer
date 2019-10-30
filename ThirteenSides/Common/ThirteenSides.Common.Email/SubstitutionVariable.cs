using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirteenSides.Common.Email
{
    public class SubstitutionVariable
    {
        public enum VariableType { Text, Html, Both };

        public String Variable { get; set; }
        public String Value { get; set; }
        public VariableType SubstitutionType { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType().ToString() == this.GetType().ToString())
            {
                return this.Variable == ((SubstitutionVariable)obj).Variable
                    && this.Value == ((SubstitutionVariable)obj).Value
                    && this.SubstitutionType == ((SubstitutionVariable)obj).SubstitutionType;
            }
            return false;
        }
    }
}
