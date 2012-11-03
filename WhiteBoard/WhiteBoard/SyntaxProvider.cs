using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WhiteBoard
{
    class SyntaxProvider
    {
        List<string> keyWords;

        public SyntaxProvider()
        {
            keyWords = new List<string>();
            keyWords.Add("MODIFY");
            keyWords.Add("UPDATE");
            keyWords.Add("SEARCH:");
            keyWords.Add("UNDO");
            keyWords.Add("DELETE");
            keyWords.Add("REMOVE");
            keyWords.Add("MARK");
            keyWords.Add("VIEW");
        }

        public List<string> Keywords
        {
            get
            {
                Debug.Assert(keyWords.Count() > 0, "No key words!");
                return keyWords;
            }
        }
    }
}
