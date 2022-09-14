#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using log4net;
using WeifenLuo.WinFormsUI.Docking;
using SourceGrid;

using treeDiM.StackBuilder.WCFService.Test.Properties;
using JJA.InputData;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Serialization;
using static treeDiM.StackBuilder.WCFService.Test.FormTestHeterogeneous;
using treeDiM.StackBuilder.WCFService.Test.SB_SR;
#endregion

namespace treeDiM.StackBuilder.WCFService.Test
{
    public partial class FormTestJJA : DockContent
    {
        #region Constructor
        public FormTestJJA()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _log.Info("Loading form FormTestJJA");
            tbFilePath.Text = DefaultInputFilePath;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DefaultInputFilePath = tbFilePath.Text;
            Settings.Default.Save();
        }
        #endregion
        #region Grid
        private void FillGridContent1()
        {
            try
            {
                gridPallets.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular)
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridPallets.BorderStyle = BorderStyle.FixedSingle;
                gridPallets.ColumnsCount = 4;
                gridPallets.FixedRows = 1;

                // header
                int iCol = 0;
                gridPallets.Rows.Insert(0);
                gridPallets[0, iCol] = new SourceGrid.Cells.ColumnHeader(string.Empty)
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
                for (int configId = 0; configId < 3; ++configId)
                    gridPallets[0, ++iCol] = new SourceGrid.Cells.ColumnHeader($"Config {configId + 1}")
                    {
                        AutomaticSortEnabled = false,
                        View = viewColumnHeader
                    };
                // pallet row
                // content
                int iIndex = 0;
                for (int iCase = 0; iCase < Cases.Count; ++iCase)
                {
                    var crate = Cases[iCase];

                    for (int iPallet = 0; iPallet < Pallets.Count; ++iPallet)
                    {
                        // insert row
                        gridPallets.Rows.Insert(++iIndex);
                        iCol = 0;

                        var pallet = Pallets[iPallet];

                        // name
                        gridPallets[iIndex, iCol] = new SourceGrid.Cells.Cell($"{crate.name} on {pallet.name}") { View = viewColumnHeader, Tag = $"{crate.name} on {pallet.name}" };

                        int indexStart = iIndex;

                        for (int configId = 0; configId < 3; ++configId)
                        {
                            iIndex = indexStart;
                            using (StackBuilderClient client = new StackBuilderClient())
                            {
                                var loadResult = client.JJA_GetLoadResultSinglePallet(
                                    new DCSBDim3D() { M0 = crate.dimensions[0], M1 = crate.dimensions[1], M2 = crate.dimensions[2] },
                                    crate.weight,
                                    crate.pcb,
                                    new DCSBPalletWHeight()
                                    {
                                        ID = iCase,
                                        Name = pallet.name,
                                        Dimensions = new DCSBDim3D()
                                        {
                                            M0 = pallet.dimensions[0],
                                            M1 = pallet.dimensions[1],
                                            M2 = pallet.dimensions[2]
                                        },
                                        Color = pallet.color,
                                        Weight = 0.0,
                                        PalletType = pallet.type,
                                        MaxPalletHeight = pallet.maxPalletHeight
                                    },
                                    (DCSBConfigId)(configId + 1),
                                    new DCCompFormat()
                                    {
                                        ShowCotations = true,
                                        Size = new DCCompSize() { CX = 250, CY = 250 }
                                    }
                                    );

                                // case count per layer
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per layer") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberPerLayer) { View = viewNormal };
                                // no of layers
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of layers") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.NumberOfLayers) { View = viewNormal };
                                // pallet map phrase
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet map phrase") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.PalletMapPhrase) { View = viewNormal };
                                // no of case/pallet
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of case per pallet") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalCase) { View = viewNormal };
                                // no of items/pallet
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Number of items per pallet") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.UpalItem) { View = viewNormal };
                                // iso base
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Area efficiency (1st layer)") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoBasePercentage) { View = viewNormal };
                                // iso pallet
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Pallet volume efficiency (%)") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Cell(loadResult.Result.IsoVolPercentage) { View = viewNormal };
                                // image
                                ++iIndex;
                                if (configId == 0) { gridPallets.Rows.Insert(iIndex); gridPallets[iIndex, 0] = new SourceGrid.Cells.Cell($"Image") { View = viewNormal }; }
                                gridPallets[iIndex, configId + 1] = new SourceGrid.Cells.Image(ByteArrayToImage(loadResult.OutFile.Bytes));


                            } // using
                        } // for (configId)

                    }
                }
                gridPallets.AutoSizeCells();
                gridPallets.Columns.StretchToFit();
                gridPallets.AutoStretchColumnsToFitWidth = true;
                gridPallets.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Warn(ex.ToString());
            }
        }

        private void FillGridContent2()
        {
            gridPallets.Rows.Clear();
            // *** viewColumnHeader
            SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
            {
                Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                {
                    BackColor = Color.LightGray,
                    Border = DevAge.Drawing.RectangleBorder.NoBorder
                },
                ForeColor = Color.Black,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
            // *** viewColumnHeader
            // *** viewNormal
            var viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
            // ***
            // set first row
            gridPallets.BorderStyle = BorderStyle.FixedSingle;
            gridPallets.ColumnsCount = 4;
            gridPallets.FixedRows = 1;
            // *** header : begin
            int iCol = 0;
            gridPallets.Rows.Insert(0);
            gridPallets[0, iCol] = new SourceGrid.Cells.ColumnHeader(string.Empty)
            {
                AutomaticSortEnabled = false,
                View = viewColumnHeader
            };
            for (int configId = 0; configId < 3; ++configId)
                gridPallets[0, ++iCol] = new SourceGrid.Cells.ColumnHeader($"Config {configId + 1}")
                {
                    AutomaticSortEnabled = false,
                    View = viewColumnHeader
                };
            // *** header : end

            // build pallet array
            var dcsbPallets = new DCSBPalletWHeight[Pallets.Count];
            for (int i = 0; i < Pallets.Count; i++)
            {
                var pallet = Pallets[i];
                dcsbPallets[i] = new DCSBPalletWHeight()
                {
                    Name = pallet.name,
                    Color = pallet.color,
                    Dimensions = new DCSBDim3D() { M0 = pallet.dimensions[0], M1 = pallet.dimensions[1], M2 = pallet.dimensions[2] },
                    Weight = 20.0,
                    MaxPalletWeight = pallet.maxLoadWeight
                };
            }
            // *** proceed with each crate
            for (int iCase = 0; iCase < Cases.Count; ++iCase)
            {
                var crate = Cases[iCase];
                // *** call WS
                using (StackBuilderClient client = new StackBuilderClient())
                {
                    var loadResults = client.JJA_GetMultiPalletResults(
                        new DCSBDim3D()
                        {
                            M0 = crate.dimensions[0],
                            M1 = crate.dimensions[1],
                            M2 = crate.dimensions[2]
                        }, crate.weight, crate.pcb, dcsbPallets);
                }
                // *** call WS
            }
            // ***

            gridPallets.AutoSizeCells();
            gridPallets.Columns.StretchToFit();
            gridPallets.AutoStretchColumnsToFitWidth = true;
            gridPallets.Invalidate();
        }

        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            Image img = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                img = Image.FromStream(ms, true);
            }
            catch { }
            return img;
        }
        #endregion
        #region Private properties
        public static string DefaultInputFilePath
        {
            get => Settings.Default.JJA_InputFilePath;
            set => Settings.Default.JJA_InputFilePath = value; 
        }
        #endregion
        #region Handlers
        private void OnSelectInputDataFile(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                tbFilePath.Text = openFileDialog.FileName;
            }
        }

        private void OnLoadMultipleCalls(object sender, EventArgs e)
        {
            _log.Info("Calling FillGridContent1()...");
            LoadInputData();
            FillGridContent1();
        }
        private void OnLoadSingleCall(object sender, EventArgs e)
        {
            _log.Info("Calling FillGridContent2()...");
            LoadInputData();
            FillGridContent2();
        }

        private void OnFileChanged(object sender, EventArgs e)
        {
            bool fileExists = File.Exists(tbFilePath.Text);
            bnEditInputFile.Enabled = fileExists;
            bnLoad1.Enabled = fileExists;
            bnLoad2.Enabled = fileExists;
        }
        private void OnEditFile(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("notepad.exe", tbFilePath.Text);
        }
        #endregion
        #region Loading data
        private void LoadInputData()
        { 
            bool result = false;
            string filePath = tbFilePath.Text;
            if (File.Exists(filePath))
            {
                using (var fStream = new FileStream(filePath, FileMode.Open))
                {
                    result = DeserializeInput(fStream, ref Containers, ref Pallets, ref Cases); 
                }
            }        
        }
        private bool DeserializeInput(FileStream fStream, ref List<inputContainer> containers, ref List<inputPallet> pallets, ref List<inputCase> cases)
        {
            // Construct an instance of the XmlSerializer with the type  of object that is being deserialized.  
            XmlSerializer inputSerializer = new XmlSerializer(typeof(input));
            // Call the Deserialize method and cast to the object type.  
            var inputRoot = (input)inputSerializer.Deserialize(fStream);
            foreach (var inputCont in inputRoot.containers)
            {   containers.Add(inputCont); }
            foreach (var inputPallet in inputRoot.pallets)
            {   pallets.Add(inputPallet); }
            foreach (var inputCase in inputRoot.cases)
            {   cases.Add(inputCase);  }
            return containers.Count > 0 && pallets.Count > 0 && cases.Count > 0;
        }
        #endregion
        #region Data members
        private List<inputPallet> Pallets = new List<inputPallet>();
        private List<inputContainer> Containers = new List<inputContainer>(); 
        private List<inputCase> Cases = new List<inputCase>();

        protected ILog _log = LogManager.GetLogger(typeof(FormTestJJA));
        #endregion

    }
}
