using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.CodeObjects
{
    internal class Gen
    {
        public Condition[] Conditions { get; private set; } = [];

        public Result[] Results { get; private set; } = [];

        public Gen(Condition[] conditions, Result[] results) 
        {
            if (conditions == null) throw new ArgumentException(nameof(conditions));
            if (results == null) throw new ArgumentException(nameof(results));

            Conditions = conditions;
            Results = results;
        }
    }
}
