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
            Conditions = conditions;
            Results = results;
        }
    }
}
