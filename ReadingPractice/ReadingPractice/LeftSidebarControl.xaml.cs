using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ReadingPractice
{
    public partial class LeftSidebarControl : UserControl
    {
        public WordDictionary wordDictionary;

        public LeftSidebarControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TODO
        /// getter returns empty string if no study focus is set (reviewing mode), returns study focus in
        /// the foreign language being studied otherwise.
        /// setter sets the study focus and updates GUI appropriately
        /// </summary>
        public string StudyFocus
        {
            get
            {
                return "";
            }
            set
            {
                value = "";
            }
        }

        /// <summary>
        /// TODO
        /// returns true if this word (given in the foreign language) is allowed to be displayed in sentences
        /// </summary>
        public bool isDisplayed(string foreignWord)
        {
            return true;
        }
    }
}
