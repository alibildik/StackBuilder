#region Using directives
using System;
using System.Web.UI;
using System.Collections.Generic;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
#endregion


namespace treeDiM.StackBuilder.TechnologyBSA_ASPNET
{
    public partial class LayerDesignIntro : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DimCaseCtrl = DimCase;
                WeightCaseCtrl = WeightCase;
                PalletIndexCtrl = PalletIndex;
                WeightPalletCtrl = WeightPallet;
                NumberOfLayersCtrl = NumberOfLayers;
            }
            OnRefresh(this, null);

            ExecuteKeyPad();
        }
        protected void OnNext(object sender, EventArgs e)
        {
            DimCase = DimCaseCtrl;
            WeightCase = WeightCaseCtrl;
            PalletIndex = PalletIndexCtrl;
            WeightPallet = WeightPalletCtrl;
            NumberOfLayers = NumberOfLayersCtrl;
            // initialize layers with a single box
            BoxPositions1 = new List<BoxPositionIndexed> { new BoxPositionIndexed(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, 1) };
            BoxPositions2 = new List<BoxPositionIndexed> { new BoxPositionIndexed(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, 1) };
            BoxPositions3 = new List<BoxPositionIndexed> { new BoxPositionIndexed(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, 1) };
            BoxPositions4 = new List<BoxPositionIndexed> { new BoxPositionIndexed(Vector3D.Zero, HalfAxis.HAxis.AXIS_X_P, HalfAxis.HAxis.AXIS_Y_P, 1) };

            Session[SessionVariables.LayerEdited] = true;
            Response.Redirect("LayerDesign.aspx");
        }

        private void ExecuteKeyPad()
        {
            if (ConfigSettings.ShowVirtualKeyboard)
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "VKeyPad", "ActivateVirtualKeyboard();", true);
        }

        private Vector3D DimCaseCtrl
        {
            get => new Vector3D(double.Parse(TBCaseLength.Text), double.Parse(TBCaseWidth.Text), double.Parse(TBCaseHeight.Text));
            set
            {
                TBCaseLength.Text = value.X.ToString();
                TBCaseWidth.Text = value.Y.ToString();
                TBCaseHeight.Text = value.Z.ToString();
            }
        }
        private double WeightCaseCtrl
        {
            get => double.Parse(TBCaseWeight.Text);
            set => TBCaseWeight.Text = value.ToString();
        }
        private Vector3D DimPalletCtrl
        {
            get
            {
                int nSel = DropDownPalletDimensions.SelectedIndex;
                switch (nSel)
                {
                    case 0: return new Vector3D(1200.0, 800.0, 144.0);
                    default: return new Vector3D(1200.0, 1000.0, 144.0);
                }
            }
        }
        private int PalletIndexCtrl
        {
            get => DropDownPalletDimensions.SelectedIndex;
            set => DropDownPalletDimensions.SelectedIndex = value;        
        }
        private double WeightPalletCtrl
        {
            get => double.Parse(TBPalletWeight.Text);
            set => TBPalletWeight.Text = value.ToString();
        }
        private int NumberOfLayersCtrl
        {
            get => int.Parse(TBNumberOfLayers.Text);
            set => TBNumberOfLayers.Text = value.ToString();
        }
        private Vector3D DimCase
        {
            get => Vector3D.Parse((string)Session[SessionVariables.DimCase]);
            set => Session[SessionVariables.DimCase] = value.ToString();
        }
        private double WeightCase
        {
            get => (double)Session[SessionVariables.WeightCase];
            set => Session[SessionVariables.WeightCase] = value;
        }
        private int PalletIndex
        {
            get => (int)Session[SessionVariables.PalletIndex];
            set => Session[SessionVariables.PalletIndex] = value;
        }
        private double WeightPallet
        {
            get => (double)Session[SessionVariables.WeightPallet];
            set => Session[SessionVariables.WeightPallet] = value;
        }
        private int NumberOfLayers
        {
            get => (int)Session[SessionVariables.NumberOfLayers];
            set => Session[SessionVariables.NumberOfLayers] = value;
        }
        private List<BoxPositionIndexed> BoxPositions1
        {
            get => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions1];
            set => Session[SessionVariables.BoxPositions1] = value;
        }
        private List<BoxPositionIndexed> BoxPositions2
        {
            get => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions2];
            set => Session[SessionVariables.BoxPositions2] = value;
        }
        private List<BoxPositionIndexed> BoxPositions3
        {
            get => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions3];
            set => Session[SessionVariables.BoxPositions3] = value;
        }
        private List<BoxPositionIndexed> BoxPositions4
        {
            get => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions4];
            set => Session[SessionVariables.BoxPositions4] = value;
        }
        protected void OnRefresh(object sender, EventArgs e)
        {
            Page.Validate();

            DimCase = DimCaseCtrl;
            CaseSetsConfiguration.Update();

            ImageCase1.ImageUrl = "~/HandlerCaseSet.ashx?number=1&Date=" + DateTime.Now.ToString();
            ImageCase2.ImageUrl = "~/HandlerCaseSet.ashx?number=2&Date=" + DateTime.Now.ToString();
            ImageCase3.ImageUrl = "~/HandlerCaseSet.ashx?number=3&Date=" + DateTime.Now.ToString();
            ImageCase4.ImageUrl = "~/HandlerCaseSet.ashx?number=4&Date=" + DateTime.Now.ToString();
        }
    }
}