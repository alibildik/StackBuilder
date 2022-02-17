#region Using directives
using System.Windows.Forms;
using System.Collections.Generic;

using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormEditConveyorSetting : Form
    {
        #region Constructor
        public FormEditConveyorSetting()
        {
            InitializeComponent();
        }
        #endregion

        #region Data members
        public ConveyorSetting ItemSetting { get; set; }
        public List<ConveyorSetting> ConveyorSettings { get; set; }
        #endregion
    }
}
