#region Using directives
using System;

using log4net;

using treeDiM.StackBuilder.Basics;
using Microsoft.Office.Interop.Excel;
#endregion

namespace treeDiM.StackBuilder.ExcelExporter
{
    public class ExporterProjetSummary
    {
        public static void ExportProjectSummaryToExcel(Document document)
        {
            // analysis exporters
            var analysisExporters = new AnalysisExporter[]
                {
                    new AnalysisExporterCasePallet(),
                    new AnalysisExporterCylinderPallet(),
                    new AnalysisExporterBoxCase(),
                    new AnalysisExporterCylinderCase(),
                    new AnalysisExporterCaseTruck(),
                    new AnalysisExporterCylinderTruck()
                };

            // open excel file
            Application xlApp = new Application
            {
                Visible = true,
                DisplayAlerts = false
            };
            Workbooks xlWorkBooks = xlApp.Workbooks;
            Workbook xlWorkBook = xlWorkBooks.Add(Type.Missing);

            foreach (var analysis in document.Analyses)
            {
                foreach (var analysisExporter in analysisExporters)
                {
                    try
                    {
                        analysisExporter.ExportAnalysis(xlWorkBook, analysis);
                    }
                    catch (Exception ex)
                    {
                        _log.Error($"Failed to export analysis '{analysis.Name}' with exception:\n {ex}");
                    }
                }
            }
        }
        #region Data members
        protected static ILog _log = LogManager.GetLogger(typeof(ExporterProjetSummary));
        #endregion
    }
}
