using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WhiteBoard
{
    //@author U095146E
    class SyntaxProvider
    {
        #region Private Fields
        private List<string> keywords;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor to instantiate keywords
        /// </summary>
        public SyntaxProvider()
        {
            keywords = new List<string>();
            keywords.Add("MODIFY");
            keywords.Add("UPDATE");
            keywords.Add("SEARCH:");
            keywords.Add("UNDO");
            keywords.Add("DELETE");
            keywords.Add("REMOVE");
            keywords.Add("MARK");
            keywords.Add("VIEW");
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns list of keywords for Syntax Highlighting
        /// </summary>
        public List<string> Keywords
        {
            get
            {
                Debug.Assert(keywords.Count() > 0, "No key words!");
                return keywords;
            }
        }
        #endregion
    }
}
