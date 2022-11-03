#region Using directives
using Microsoft.Office.Interop.Word;
using System;
using System.IO;
#endregion

namespace treeDiM.StackBuilder.Reporting
{
    #region Margins
    public class Margins
    {
        public Margins()
        {
            _top = Properties.Settings.Default.MarginTop;
            _bottom = Properties.Settings.Default.MarginBottom;
            _left = Properties.Settings.Default.MarginLeft;
            _right = Properties.Settings.Default.MarginRight;
        }
        public float Top { get { return _top > 0.0f ? _top : 0.0f; } }
        public float Bottom { get { return _bottom > 0.0f ? _bottom : 0.0f; } }
        public float Left { get { return _left > 0.0f ? _left : 0.0f; } }
        public float Right { get { return _right > 0.0f ? _right : 0.0f; } }
        private float _top = 10.0f, _bottom = 10.0f, _right = 10.0f, _left = 10.0f;
    }
    #endregion
    #region ReporterMSWord
    public class ReporterMSWord : Reporter
    {
        #region Constructor
        public ReporterMSWord(ReportData inputData, ref ReportNode rnRoot
            , string templatePath, string outputFilePath, Margins margins)
        {
            // absolute output file path
            string absOutputFilePath;
            if (Path.IsPathRooted(outputFilePath))
                absOutputFilePath = outputFilePath;
            else
                absOutputFilePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), outputFilePath));
            // absolute template path
            string absTemplatePath;
            if (Path.IsPathRooted(templatePath))
                absTemplatePath = templatePath;
            else
                absTemplatePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), templatePath));

            // does output directory exists
            string outDir = Path.GetDirectoryName(absOutputFilePath);
            if (!Directory.Exists(outDir))
            {
                try { Directory.CreateDirectory(outDir); }
                catch (UnauthorizedAccessException /*ex*/)
                { throw new UnauthorizedAccessException(string.Format("User not allowed to write under {0}", Directory.GetParent(outDir).FullName)); }
                catch (Exception ex)
                { throw new Exception(string.Format("Directory {0} does not exist, and could not be created.", outDir), ex); }
            }
            // html file path
            string htmlFilePath = Path.ChangeExtension(absOutputFilePath, "html");
            BuildAnalysisReport(inputData, ref rnRoot, absTemplatePath, htmlFilePath);
            // opens word
            Application wordApp = new Application
            {
                Visible = true
            };
            Document wordDoc = wordApp.Documents.Open(htmlFilePath, false, true, NoEncodingDialog: true);
            // set print view 
            wordApp.ActiveWindow.ActivePane.View.Type = WdViewType.wdPrintView;
            wordDoc.SaveAs(absOutputFilePath, WdSaveFormat.wdFormatDocumentDefault);
            _log.Info(string.Format("Saved doc report to {0}", outputFilePath));

            // embed pictures (unlinking images)
            for (int i = 1; i <= wordDoc.InlineShapes.Count; ++i)
            {
                if (null != wordDoc.InlineShapes[i].LinkFormat && !wordDoc.InlineShapes[i].LinkFormat.SavePictureWithDocument)
                    wordDoc.InlineShapes[i].LinkFormat.SavePictureWithDocument = true;
            }
            for (int i = 1; i <= wordDoc.Fields.Count; ++i)
            {
                var field = wordDoc.Fields[i];
                if (null != field && field.Type == WdFieldType.wdFieldIncludePicture)
                    field.LinkFormat.SavePictureWithDocument = true;
            }
 
            // set margins (unit?)
            wordDoc.PageSetup.TopMargin = wordApp.CentimetersToPoints(margins.Top);
            wordDoc.PageSetup.BottomMargin = wordApp.CentimetersToPoints(margins.Bottom);
            wordDoc.PageSetup.RightMargin = wordApp.CentimetersToPoints(margins.Right);
            wordDoc.PageSetup.LeftMargin = wordApp.CentimetersToPoints(margins.Left);

           if (Properties.Settings.Default.WordDeleteImage)
           {
                // wait before deleting image
                int timeBeforeDeletion = Properties.Settings.Default.WordDeleteImageDelay;
                System.Threading.Thread.Sleep(timeBeforeDeletion * 1000);
                // delete image directory
                DeleteImageDirectory();
                // delete html report
                File.Delete(htmlFilePath);
           }
        }
        #endregion
        #region Override Reporter
        public override bool WriteNamespace => false;
        public override bool WriteImageFiles => true;
        #endregion
    }
#endregion
}
