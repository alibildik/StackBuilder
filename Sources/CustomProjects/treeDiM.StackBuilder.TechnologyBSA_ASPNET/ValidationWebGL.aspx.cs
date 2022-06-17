#region Using directives
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
#endregion

namespace treeDiM.StackBuilder.TechnologyBSA_ASPNET
{
    public partial class ValidationWebGL : Page
    {
        #region Page_Load 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                TBFileName.Text = FileName;
                var interlayerArray = Interlayers.Select(p => p == '1' ? true : false).ToArray();
                ListLayerIndexes = LayerIndexes.Split(' ').Select(n => Convert.ToInt32(n)).ToList();


                var listInterlayers = new List<LayerDataShort>();
                PalletStacking.InitializeInterlayers(DimCase, PalletIndex, NoLayers, string.Empty, ref listInterlayers);
                for (var i = 0; i < interlayerArray.Length; ++i)
                {
                    if (i < listInterlayers.Count)
                    {
                        listInterlayers[i].HasInterlayer = interlayerArray[i];
                        listInterlayers[i].LayerIndex = ListLayerIndexes[i];
                    }
                }

                listInterlayers.Reverse();
                LVInterlayers.DataSource = listInterlayers;
                LVInterlayers.DataBind();


                // clear output directory
                DirectoryHelpers.ClearDirectory(Output);
            }
            ExecuteKeyPad();
            UpdateImage();
        }
        private void ExecuteKeyPad()
        {
            if (ConfigSettings.ShowVirtualKeyboard)
                ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "VKeyPad", "ActivateVirtualKeyboard();", true);
        }
        #endregion

        #region Update image
        protected void UpdateImage()
        {
            // clear output directory
            DirectoryHelpers.ClearDirectory(Output);

            int caseCount = 0, layerCount = 0;
            double weightLoad = 0.0, weightTotal = 0.0;
            var bbLoad = Vector3D.Zero;
            var bbTotal = Vector3D.Zero;
            string fileGuid = Guid.NewGuid().ToString() + ".glb";

            PalletStacking.GenerateExport(
                DimCase, WeightCase, BitmapTexture,
                PalletIndex, WeightPallet,
                ListLayerTypes,
                LayerIndexesIntArray,
                InterlayersBoolArray,
                Path.Combine(Output, fileGuid),
                ref caseCount, ref layerCount,
                ref weightLoad, ref weightTotal,
                ref bbLoad, ref bbTotal
            );
            XModelDiv.InnerHtml = $"<x-model class=\"x-model\" src=\"./Output/{fileGuid}\"/>";
            loadedPallet.Update();
        }
        #endregion

        #region Event handlers
        protected void OnSelectedLayerTypeChanged(object sender, EventArgs e)
        {
            if (sender is DropDownList dropDownList)
            {
                UpdateImage();
            }
        }
        protected void OnInputChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }
        protected void OnExport(object sender, EventArgs e)
        {
            try
            {
                string fileName = TBFileName.Text;
                fileName = Path.ChangeExtension(fileName, "csv");

                byte[] fileBytes = null;
                byte[] imageFileBytes = null;
                PalletStacking.Export(
                    DimCase, WeightCase,
                    PalletIndex, WeightPallet,
                    ListLayerTypes,
                    LayerIndexesIntArray,
                    InterlayersBoolArray,
                    LayerDesignMode,
                    ref fileBytes,
                    ParseImageFormat(ConfigSettings.ExportImageFormat),
                    ref imageFileBytes);

                if (FtpHelpers.Upload(fileBytes, ConfigSettings.FtpDirectory, fileName, ConfigSettings.FtpUsername, ConfigSettings.FtpPassword)
                    && FtpHelpers.Upload(imageFileBytes, ConfigSettings.FtpDirectory + "Images/", Path.ChangeExtension(fileName, ConfigSettings.ExportImageFormat), ConfigSettings.FtpUsername, ConfigSettings.FtpPassword))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('{fileName} was successfully exported!');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('{ex.Message}');", true);
            }
        }
        protected void OnPrevious(object sender, EventArgs e)
        {
            // clear output directory
            DirectoryHelpers.ClearDirectory(Output);

            if (0 == LayerDesignMode)
                Response.Redirect("LayerDesign.aspx");
            else
            {
                if (LayerEdited)
                    Response.Redirect("LayerEdition.aspx");
                else
                    Response.Redirect("LayerSelectionWebGL.aspx");
            }
        }
        #endregion

        #region Private variables
        public static ImageFormat ParseImageFormat(string str)
        {
            return (ImageFormat)typeof(ImageFormat)
                    .GetProperty(str, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .GetValue(null);
        }
        private List<bool> InterlayersBoolArray
        {
            get
            {
                var list = new List<bool>();

                list.Add(HasTopInterlayer);

                foreach (var item in LVInterlayers.Items)
                {
                    if (item.FindControl("LayerCheckBox") is CheckBox chkBox)
                        list.Add(chkBox.Checked);
                }
                list.Reverse();
                return list;
            }
        }
        private List<int> LayerIndexesIntArray
        {
            get
            {
                var list = new List<int>();
                foreach (var item in LVInterlayers.Items)
                {
                    if (item.FindControl("LayerDropDown") is DropDownList dropDownList)
                        list.Add(dropDownList.SelectedIndex);
                }
                list.Reverse();
                return list;
            }
        }

        private List<List<BoxPositionIndexed>> ListLayerTypes
        {
            get
            {
                var listLayerTypes = new List<List<BoxPositionIndexed>>();
                if (BoxPositions1.Count > 0) listLayerTypes.Add(BoxPositions1);
                if (BoxPositions2.Count > 0) listLayerTypes.Add(BoxPositions2);
                if (BoxPositions3.Count > 0) listLayerTypes.Add(BoxPositions3);
                if (BoxPositions4.Count > 0) listLayerTypes.Add(BoxPositions4);
                return listLayerTypes;
            }
        }
        private bool HasTopInterlayer => LayerCheckBoxTop.Checked;
        private Vector3D DimCase => Vector3D.Parse((string)Session[SessionVariables.DimCase]);
        private double WeightCase => (double)Session[SessionVariables.WeightCase];
        private int PalletIndex => (int)Session[SessionVariables.PalletIndex];
        private double WeightPallet => (double)Session[SessionVariables.WeightPallet];
        private int NoLayers => (int)Session[SessionVariables.NumberOfLayers];
        private string LayerIndexes
        { 
            get => (string) Session[SessionVariables.LayerIndexes];
            set => Session[SessionVariables.LayerIndexes] = value;
        }
        private List<int> ListLayerIndexes
        {
            get => LayerIndexes.Split(' ').Select(n => Convert.ToInt32(n)).ToList();
            set => LayerIndexes = string.Join(" ", value.Select(n => n.ToString()).ToArray());
        } 
        private bool LayerEdited => (bool)Session[SessionVariables.LayerEdited];
        private string FileName => (string)Session[SessionVariables.FileName];
        private List<BoxPositionIndexed> BoxPositions1 => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions1];
        private List<BoxPositionIndexed> BoxPositions2 => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions2];
        private List<BoxPositionIndexed> BoxPositions3 => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions3];
        private List<BoxPositionIndexed> BoxPositions4 => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions4];
        private Bitmap BitmapTexture => (Bitmap)Session[SessionVariables.BitmapTexture];
        private string Output => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
        private string Interlayers => (string) Session[SessionVariables.Interlayers];
        private int LayerDesignMode => (int)Session[SessionVariables.LayerDesignMode];
        #endregion
    }
}