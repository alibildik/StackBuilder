#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Engine;
#endregion

namespace treeDiM.StackBuilder.TechnologyBSA_ASPNET
{
    public partial class LayerSelectionWebGL : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // clear output directory
                DirectoryHelpers.ClearDirectory(Output);

                try
                {
                    string filePath = Server.MapPath(@"~\Images\Texture.png");
                    if (File.Exists(filePath))
                        BitmapTexture = new Bitmap(filePath);
                }
                catch (Exception ex)
                {
                    string message = ex.ToString();
                }
                DimCaseCtrl = DimCase;
                WeightCaseCtrl = WeightCase;
                PalletIndexCtrl = PalletIndex;
                WeightPalletCtrl = WeightPallet;
                NumberOfLayersCtrl = NumberOfLayers;

                BTRefresh_Click(null, null);
            }
            ExecuteKeyPad();
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (dlLayers.SelectedIndex > -1) return;
            if (dlLayers.Items.Count < 1) return;
            var item = dlLayers.Items[0];
            if (DoSelectDataItem(item) == true)
            {
                // Get 1st ImageButton
                ImageButton imgBtn = dlLayers.Items[0].FindControl("Image1") as ImageButton;
                // Instantiate new DataListCommandEventArgs
                ListViewCommandEventArgs ev = new ListViewCommandEventArgs(dlLayers.Items[0], imgBtn, new CommandEventArgs(imgBtn.CommandName, imgBtn.CommandArgument));
                // Call ItemCommand handler
                OnLVLayersItemCommand(sender, ev);
            }
        }

        private bool DoSelectDataItem(ListViewDataItem item)
        {
            return item.DisplayIndex == 0; // selects the first item in the list (this is just an example after all; keeping it simple :D )
        }

        protected void BTRefresh_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (Page.IsValid)
            {
                DimCase = DimCaseCtrl;
                WeightCase = WeightCaseCtrl;
                PalletIndex = PalletIndexCtrl;
                WeightPallet = WeightPalletCtrl;
                NumberOfLayers = NumberOfLayersCtrl;

                if (DimCase.X < 50 || DimCase.Y < 50 || DimCase.Z < 50)
                    return;

                List<LayerDetails> listLayers = new List<LayerDetails>();
                PalletStacking.GetLayers(DimCaseCtrl, WeightCaseCtrl, PalletIndexCtrl, WeightPalletCtrl, NumberOfLayersCtrl, false, ref listLayers);

                dlLayers.DataSource = listLayers;
                dlLayers.DataBind();
                layersUpdate.Update();
                ExecuteKeyPad();
                PalletDetails.DataSource = null;
                PalletDetails.DataBind();
                dlLayers.SelectedIndex = -1;

                selectedLayer.Update();
            }
        }

        protected void OnNext(object sender, EventArgs e)
        {
            DimCase = DimCaseCtrl;
            WeightCase = WeightCaseCtrl;
            PalletIndex = PalletIndexCtrl;
            WeightPallet = WeightPalletCtrl;
            NumberOfLayers = NumberOfLayersCtrl;
            BoxPositions1 = BoxPositionsLayer[0];

            Session[SessionVariables.LayerEdited] = false;
            Response.Redirect(ConfigSettings.WebGLMode ? "ValidationWebGL.aspx" : "Validation.aspx");
        }

        protected void OnLVLayersItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "ImageButtonClick")
            {			
                dlLayers.Items.AsEnumerable().Foreach(a => { var item = (ImageButton)(a.Controls)[1]; item.Attributes["class"] = ""; });
                var selectedItem = (ImageButton)(e.Item.Controls)[1];
                selectedItem.Attributes["class"] = "border";
                // get layer description of selected button
                ViewState["LayerDescriptor"] = e.CommandArgument.ToString();

                dlLayers.SelectedIndex = e.Item.DisplayIndex;

                UpdateImage();
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "scroll", "ScrollTo();", true);
                ExecuteKeyPad();
            }
        }

        private void ExecuteKeyPad()
        {
            if (ConfigSettings.ShowVirtualKeyboard)
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "VKeyPad", "ActivateVirtualKeyboard();", true);
        }

        protected void UpdateImage()
        {
            // clear output directory
            DirectoryHelpers.ClearDirectory(Output);

            Vector3D caseDim = DimCaseCtrl;
            double caseWeight = WeightCaseCtrl;
            double palletWeight = WeightPalletCtrl;

            byte[] imageBytes = null;
            int caseCount = 0;
            int layerCount = 0;
            double weightLoad = 0.0, weightTotal = 0.0;
            Vector3D bbLoad = Vector3D.Zero;
            Vector3D bbTotal = Vector3D.Zero;
            string fileGuid = Guid.NewGuid().ToString() + ".glb";
            List<bool> interlayers = new List<bool>();
            List<int> ListLayerIndexes = new List<int>();

            
            PalletStacking.GenerateExport(
                caseDim, caseWeight, BitmapTexture,
                PalletIndexCtrl, palletWeight,
                BoxPositionsLayer, ListLayerIndexes,               
                interlayers,
                Path.Combine(Output, fileGuid),
                ref caseCount, ref layerCount,
                ref weightLoad, ref weightTotal,
                ref bbLoad, ref bbTotal
            );

            XModelDiv.InnerHtml = string.Format("<x-model class=\"x-model\" src=\"./Output/{0}\"/>", fileGuid);

            var palletDetails = new List<PalletDetails>
            {
                new PalletDetails("Number of cases", $"{caseCount}", ""),
                new PalletDetails("Layer count", $"{layerCount}", ""),
                new PalletDetails("Load weight", $"{weightLoad}", "kg"),
                new PalletDetails("Total weight", $"{weightTotal}", "kg"),
                new PalletDetails("Load dimensions", $"{bbLoad.X} x {bbLoad.Y} x {bbLoad.Z}", "mm"),
                new PalletDetails("Overall dimensions", $"{bbTotal.X} x {bbTotal.Y} x {bbTotal.Z}", "mm")
            };

            PalletDetails.DataSource = palletDetails;
            PalletDetails.DataBind();

            DimCase = caseDim;
            PalletIndex = PalletIndexCtrl;
            NumberOfLayers = NumberOfLayersCtrl;
            Session[SessionVariables.ImageWidth] = 500;
            Session[SessionVariables.ImageHeight] = 460;
            Session[SessionVariables.ImageBytes] = imageBytes;

            selectedLayer.Update();
        }
        protected void OnEditLayer(object sender, EventArgs e)
        {
            DimCase = DimCaseCtrl;
            WeightCase = WeightCaseCtrl;
            PalletIndex = PalletIndexCtrl;
            WeightPallet = WeightPalletCtrl;
            SelectedIndex = -1;
            BoxPositions1 = BoxPositionsLayer[0];

            Session[SessionVariables.LayerEdited] = true;
            Response.Redirect("LayerEdition.aspx");
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
        private int PalletIndexCtrl
        {
            get => DropDownPalletDim.SelectedIndex;
            set => DropDownPalletDim.SelectedIndex = value;
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
            set => Session[SessionVariables.BoxPositions1] = value;
        }
        private int SelectedIndex
        {
            set => Session[SessionVariables.SelectedIndex] = value;
        }
        private List<List<BoxPositionIndexed>> BoxPositionsLayer
        {
            get
            {
                var layerDesc = LayerDescBox.Parse(ViewState["LayerDescriptor"].ToString()) as LayerDescBox;
                LayerSolver solver = new LayerSolver();
                var layer = solver.BuildLayer(DimCaseCtrl, PalletStacking.PalletIndexToDim2D(PalletIndexCtrl), layerDesc, 0.0);

                List<List<BoxPositionIndexed>> listLayerTypes = new List<List<BoxPositionIndexed>>();
                var listPositions = new List<BoxPositionIndexed>();
                int counter = 0;
                foreach (var pos in layer.Positions)
                    listPositions.Add(new BoxPositionIndexed(pos, ++counter));

                for (int i = 0; i < 4; ++i)
                    listLayerTypes.Add(listPositions);

                return listLayerTypes;
            }
        }
        private Bitmap BitmapTexture
        {
            get => (Bitmap)Session[SessionVariables.BitmapTexture];
            set => Session[SessionVariables.BitmapTexture] = value;
        }
        private string Output => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
    }
}
