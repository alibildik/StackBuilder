#region Using directives
using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Excel = Microsoft.Office.Interop.Excel;

using log4net;
using Syroot.Windows.IO;

using treeDiM.PLMPack.DBClient.PLMPackSR;
using treeDiM.PLMPack.DBClient;

using treeDiM.Basics;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics.Controls;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormExcelMassAnalysis : Form
    {
        #region Constructor
        public FormExcelMassAnalysis()
        {
            InitializeComponent();
        }
        #endregion
        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitializeInputFields();
            LoadSettings();
        }
        #endregion
        #region Protected properties
        protected string InputFilePath
        {
            get => fileSelectExcel.FileName;
            set => fileSelectExcel.FileName = value;
        }
        protected string SheetName => cbSheets.SelectedItem.ToString();
        #endregion
        #region Event handlers
        private void OnFilePathChanged(object sender, EventArgs e)
        {
            if (File.Exists(InputFilePath))
            {
                try
                {
                    cbSheets.Items.Clear();
                    Excel.Application xlApp = new Excel.Application() { Visible = false, DisplayAlerts = false };
                    Excel.Workbook xlWbk = xlApp.Workbooks.Open(InputFilePath, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    foreach (Excel.Worksheet worksheet in xlWbk.Worksheets)
                    {
                        cbSheets.Items.Add(worksheet.Name);
                    }
                    xlWbk.Close();

                    if (cbSheets.Items.Count > 0)
                        cbSheets.SelectedIndex = 0;
                    xlApp.Quit();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
            }
            UpdateStatus(sender, e);
        }
        protected void UpdateStatus(object sender, EventArgs e)
        {
            string message = GetStatusMessage();

            statusStripLabel.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            statusStripLabel.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
            bnCompute.Enabled = string.IsNullOrEmpty(message);
        }
        private void OnCompute(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            SaveSettings();
            StringBuilder sbErrors = new StringBuilder();

            try
            {
                string inputPath = InputFilePath;
                string outputPath = Path.Combine(Path.GetDirectoryName(inputPath), Path.GetFileNameWithoutExtension(inputPath) + "_output");
                string filePathCopy = Path.ChangeExtension(outputPath, Path.GetExtension(inputPath));

                File.Copy(inputPath, filePathCopy, true);

                // get the collection of work sheets
                Excel.Application xlApp = new Excel.Application() { Visible = true, DisplayAlerts = false };
                Excel.Workbook xlWbk = xlApp.Workbooks.Open(filePathCopy, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                Excel.Worksheet xlSheet = (Excel.Worksheet)xlApp.Sheets[SheetName];
                Excel.Range range = xlSheet.UsedRange;
                int rowCount = range.Rows.Count;

                Compute(xlSheet, sbErrors);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                switch ((uint)ex.ErrorCode)
                {
                    case 0x800A03EC:
                        MessageBox.Show("NAME_NOT_FOUND : Could not find cell with given name!");
                        break;
                    default:
                        MessageBox.Show(ex.Message);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = Cursors.Arrow;
            }
        }
        private void OnDownloadSampleSheet(object sender, EventArgs e)
        {
            var knownFolder = new KnownFolder(KnownFolderType.Downloads);
            string downloadPath = Path.Combine(knownFolder.Path, Path.GetFileName(SampleFileURL));
            if (DownloadHelper.DownloadFile(SampleFileURL, false, downloadPath))
                fileSelectExcel.FileName = downloadPath;
            InitializeInputFields();
        }
        private void OnSheetChanged(object sender, EventArgs e)
        {
            UpdateStatus(sender, e);
        }
        #endregion
        #region Virtual properties and methods
        protected virtual string SampleFileURL => string.Empty;
        protected virtual void InitializeInputFields() {}
        protected virtual void SaveSettings() { }
        protected virtual void LoadSettings() { }
        protected virtual void Compute(Excel.Worksheet xlSheet, StringBuilder sbErrors) { }
        protected virtual string GetStatusMessage()
        {
            string message = string.Empty;
            if (!File.Exists(InputFilePath))
                message = string.Format(Resources.ID_ERROR_INVALIDINPUTFILEPATH, InputFilePath);
            else if (cbSheets.Items.Count < 1)
                message = Resources.ID_ERROR_NOSHEETLOADED;
            return message;
        }
        protected bool UserIsSubscribed => DesignMode || Program.IsSubscribed;
        protected int MaxNumberRowFree { get; set; } = 5;
        #endregion
        #region Pallet listbox
        public static void FillPalletList(CheckedListBox listBoxPallets)
        {
            int rangeIndex = 0;
            int numberOfItems = 0;
            listBoxPallets.Items.Clear();

            using (WCFClient wcfClient = new WCFClient())
            {
                do
                {

                    DCSBPallet[] dcsbPallets = wcfClient.Client.GetAllPallets(rangeIndex++, ref numberOfItems);
                    foreach (var dcsbPallet in dcsbPallets)
                    {
                        UnitsManager.UnitSystem us = (UnitsManager.UnitSystem)dcsbPallet.UnitSystem;
                        var palletProperties = new PalletProperties(null,
                            dcsbPallet.PalletType,
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M0, us),
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M1, us),
                            UnitsManager.ConvertLengthFrom(dcsbPallet.Dimensions.M2, us)
                            )
                        {
                            Color = Color.FromArgb(dcsbPallet.Color),
                            Weight = UnitsManager.ConvertMassFrom(dcsbPallet.Weight, us),
                            AdmissibleLoadWeight = dcsbPallet.AdmissibleLoad.HasValue ? UnitsManager.ConvertMassFrom(dcsbPallet.AdmissibleLoad.Value, us) : 0.0
                        };
                        palletProperties.ID.SetNameDesc(dcsbPallet.Name, dcsbPallet.Description);

                        listBoxPallets.Items.Add(new ItemBaseWrapper(palletProperties), true);
                    }
                }
                while (rangeIndex * 20 < numberOfItems);
            }
        }
        public static List<PalletProperties> SelectedPallets(CheckedListBox listBox)
        {
            List<PalletProperties> list = new List<PalletProperties>();
            foreach (var item in listBox.CheckedItems)
            {
                if (item is ItemBaseWrapper ibw)
                {
                    if (ibw.ItemBase is PalletProperties palletProp)
                        list.Add(palletProp);
                }
            }
            return list;
        }
        #endregion
        #region Logging
        protected ILog _log = LogManager.GetLogger(typeof(FormExcelMassAnalysis));
        #endregion
    }
}
#region ExcelHelpers
internal class ExcelHelpers
{
    public static double ReadDouble(string name, Excel.Worksheet worksheet, string cellName)
    {
        try { return (double)worksheet.Range[cellName, cellName].Value; }
        catch (Exception ex) { throw new ExceptionCellReading(name, cellName, ex.Message); }
    }
    public static string ReadString(string name, Excel.Worksheet worksheet, string cellName)
    {
        try { return worksheet.Range[cellName, cellName].Value.ToString(); }
        catch (Exception ex) { throw new ExceptionCellReading(name, cellName, ex.Message); }
    }
    public static void FillComboWithColumnName(ComboBox cb)
    {
        cb.Items.Clear();
        char[] az = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char)i).ToArray();
        foreach (var c in az)
            cb.Items.Add(c.ToString());
    }
    public static string ColumnIndexToColumnLetter(int colIndex)
    {
        var div = colIndex;
        var colLetter = string.Empty;
        while (div > 0)
        {
            var mod = (div - 1) % 26;
            colLetter = (char)(65 + mod) + colLetter;
            div = (div - mod) / 26;
        }
        return colLetter;
    }
    public static int ColumnLetterToColumnIndex(string columnLetter, int max = 26)
    {
        string columnLetterUpper = columnLetter.ToUpper();
        int sum = 0;
        foreach (var t in columnLetterUpper)
        {
            sum *= 26;
            sum += t - 'A' + 1;
        }
        if (sum < 0) sum = 0;
        if (sum > max) sum = max;
        return sum;
    }
}
#endregion
#region ExceptionCellReading
internal class ExceptionCellReading : Exception
{
    public ExceptionCellReading()
        : base()
    {
    }
    public ExceptionCellReading(string message)
        : base(message)
    {
    }
    public ExceptionCellReading(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    public ExceptionCellReading(string vName, string cellName, string sMessage)
        : base(sMessage)
    {
        VName = vName;
        CellName = cellName;
    }
    public string VName { get; set; }
    public string CellName { get; set; }
    public override string Message => $"{VName} expected in cell {CellName}";
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat(Message);
        sb.Append(base.ToString());
        return sb.ToString();
    }
}
#endregion