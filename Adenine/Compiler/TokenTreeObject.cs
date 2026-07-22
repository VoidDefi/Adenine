using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adenine.Compiler
{
    internal class TokenTreeObject
    {
        public Token Token { get; set; }

        public List<TokenTreeObject> Branch { get; set; }

        public TokenTreeObject(Token token)
        {
            Token = token;
            Branch = new();
        }

        public override string ToString()
        {
            return $"(token: {Token}) (tokens in branch: {Branch.Count})";
        }
    }
}
