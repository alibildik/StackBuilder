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
    public partial class Validation : Page
    {
        #region Page_Load 
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Angle = 45.0;
                TBFileName.Text = FileName;

                var interlayerArray = Interlayers.Select(p => p == '1' ? true : false).ToArray();
                var listInterlayers = new List<LayerDataShort>();
                PalletStacking.InitializeInterlayers(DimCase, PalletIndex, NumberOfLayers, string.Empty, ref listInterlayers);
                for (var i = 0; i < interlayerArray.Length; ++i)
                {
                    if (i < listInterlayers.Count)
                        listInterlayers[i].HasInterlayer = interlayerArray[i];
                }

                listInterlayers.Reverse();
                LVInterlayers.DataSource = listInterlayers;
                LVInterlayers.DataBind();
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
            byte[] imageBytes = null;
            int caseCount = 0;
            int layerCount = 0;
            double weightLoad = 0.0, weightTotal = 0.0;
            Vector3D bbLoad = Vector3D.Zero;
            Vector3D bbTotal = Vector3D.Zero;

            PalletStacking.GetSolution(
                DimCase, WeightCase, BitmapTexture,
                PalletIndex, WeightPallet,
                NumberOfLayers, BoxPositions,
                ChkbMirrorLength.Checked, ChkbMirrorWidth.Checked,
                InterlayersBoolArray,
                Angle,
                new Size(550, 550),
                ref imageBytes, ref caseCount, ref layerCount, ref weightLoad, ref weightTotal, ref bbLoad, ref bbTotal);

            Session[SessionVariables.ImageWidth] = 550;
            Session[SessionVariables.ImageHeight] = 550;
            Session[SessionVariables.ImageBytes] = imageBytes;

            ImagePallet.ImageUrl = "~/Handler.ashx?param=" + DateTime.Now.Ticks.ToString();

            loadedPallet.Update();
        }
        #endregion

        #region Event handlers
        protected void OnInputChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }
        protected void AngleIncrement(object sender, EventArgs e)
        {
            Angle += ConfigSettings.AngleStep;
            UpdateImage();
        }
        protected void AngleDecrement(object sender, EventArgs e)
        {
            Angle -= ConfigSettings.AngleStep;
            UpdateImage();
        }
        protected void OnExport(object sender, EventArgs e)
        {
            string fileName = TBFileName.Text;
            fileName = Path.ChangeExtension(fileName, "csv");
            List<int> ListLayerIndexes = new List<int>();
            List<List<BoxPositionIndexed>> listBoxPositions = new List<List<BoxPositionIndexed>>();


            byte[] fileBytes = null;
            byte[] imageFileBytes = null;
            PalletStacking.Export(
                DimCase, WeightCase,
                PalletIndex, WeightPallet,
                listBoxPositions,
                ListLayerIndexes,
                InterlayersBoolArray,
                LayerDesignMode,
                ref fileBytes,
                ParseImageFormat(ConfigSettings.ExportImageFormat),
                ref imageFileBytes);

            if (FtpHelpers.Upload(fileBytes, ConfigSettings.FtpDirectory, fileName, ConfigSettings.FtpUsername, ConfigSettings.FtpPassword)
                && FtpHelpers.Upload(fileBytes, ConfigSettings.FtpDirectory + "/Images/", Path.ChangeExtension(fileName, ConfigSettings.ExportImageFormat), ConfigSettings.FtpUsername, ConfigSettings.FtpPassword))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage", $"alert('{fileName} was successfully exported!');", true);
            }
        }
        protected void OnPrevious(object sender, EventArgs e)
        {
            if (LayerEdited)
                Response.Redirect("LayerEdition.aspx");
            else
                Response.Redirect("LayerSelection.aspx");
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
                foreach (var item in LVInterlayers.Items)
                {
                    if (item.FindControl("LayerCheckBox") is CheckBox chkBox)
                        list.Add(chkBox.Checked);
                }
                list.Reverse();
                return list;
            }
        }
        private Vector3D DimCase=> Vector3D.Parse((string)Session[SessionVariables.DimCase]);
        private double WeightCase => (double)Session[SessionVariables.WeightCase];
        private int PalletIndex => (int)Session[SessionVariables.PalletIndex];
        private double WeightPallet => (double)Session[SessionVariables.WeightPallet];
        private int NumberOfLayers => (int)Session[SessionVariables.NumberOfLayers];
        private bool LayerEdited => (bool)Session[SessionVariables.LayerEdited];
        private double Angle	{	get => (double)ViewState["Angle"];	set => ViewState["Angle"] = value;	}
        private string FileName => (string)Session[SessionVariables.FileName];
        private List<BoxPositionIndexed> BoxPositions => (List<BoxPositionIndexed>)Session[SessionVariables.BoxPositions1];
        private Bitmap BitmapTexture => (Bitmap)Session[SessionVariables.BitmapTexture];
        private string Interlayers => (string)Session[SessionVariables.Interlayers];
        private int LayerDesignMode => (int)Session[SessionVariables.LayerDesignMode];
        #endregion
    }
}