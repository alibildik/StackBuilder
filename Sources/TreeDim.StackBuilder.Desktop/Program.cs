#region Using directives
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Diagnostics;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Resources;
// CrashReporter.NET
using CrashReporterDotNET;
// log4net
using log4net;
using log4net.Config;
// treeDiM
using treeDiM.Basics;
using treeDiM.StackBuilder.Desktop.Properties;
#endregion

#region File association
using Utilities;
using System.Threading;
using System.Reflection;
using System.IO;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    static class Program
    {
        #region Main
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // set up a simple logging configuration
            XmlConfigurator.Configure();
            if (!LogManager.GetRepository().Configured)
                Debug.Fail("Logging not configured!\n Press ignore to continue");

            // 
            MessageFilter oFilter = new MessageFilter();
            Application.AddMessageFilter((IMessageFilter)oFilter);

            // note: arguments are handled within FormMain constructor
            // using Environment.GetCommandLineArgs()
            // force CultureToUse culture if specified in config file
            string specifiedCulture = Settings.Default.CultureToUse;
            if (!string.IsNullOrEmpty(specifiedCulture))
            {
                try
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(specifiedCulture);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(specifiedCulture);
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("Specified culture in config file ({0}) appears to be invalid: {1}", specifiedCulture, ex.Message));
                }
            }

            // get current culture
            _log.Info(string.Format("Starting {0} with user culture {1}", Application.ProductName, Thread.CurrentThread.CurrentUICulture));

            // set unit system
            UnitsManager.CurrentUnitSystem = (UnitsManager.UnitSystem)Settings.Default.UnitSystem; 

            // file association
            RegisterFileType();

            // enable browser emulation (use Edge instead of IE?)
            EnsureBrowserEmulationEnabled();

            // *** crash reporting
            Application.ThreadException += (sender, threadargs) => SendCrashReport(threadargs.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, threadargs) =>
            {
                SendCrashReport((Exception)threadargs.ExceptionObject);
                Environment.Exit(0);
            };
            // *** crash reporting

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // set some static properties
            Graphics.ViewerSolution.DimCasePalletSol1 = Settings.Default.DimCasePalletSol1;
            Graphics.ViewerSolution.DimCasePalletSol2 = Settings.Default.DimCasePalletSol2;
            Graphics.ViewerSolution.DistanceAboveSelectedLayer = UnitsManager.ConvertLengthFrom(Settings.Default.DistanceAboveSelectedLayer, UnitsManager.UnitSystem.UNIT_METRIC1);

            // show main form
            Application.Run(new FormMain());

            _log.Info("Closing " + Application.ProductName);
        }
        #endregion

        #region Public static members
        public static bool IsWebSiteReachable
        {
            get
            {
                string testUrl = $"https://www.google.com/";
                try
                {
                    Uri uri = new Uri(testUrl);
                    System.Net.IPHostEntry objIPHE = System.Net.Dns.GetHostEntry(uri.DnsSafeHost);
                    return true;
                }
                catch (System.Net.Sockets.SocketException /*ex*/)
                {
                    _log.InfoFormat("Url '{0}' could not be accessed -> the computer is probably not connected to the web!", testUrl);
                    return false;
                }
                catch (Exception ex)
                {
                    _log.Error(ex.ToString());
                    return false;
                }
            }
        }
        public static bool UseDisconnected
        {
            get
            {
                // if not already working disconnected
                // see if we can work disconnected
                if (!_useDisconnected)
                    _useDisconnected = Settings.Default.AllowDisconnectedMode && !IsWebSiteReachable;
                return _useDisconnected;
            }
        }
        public static bool IsSubscribed { get; set; } = false;
        #endregion

        #region Culture
        public static bool IsCurrentCultureSupported()
        {
            // get list of available cultures
            ReadOnlyCollection<CultureInfo> listOfAvailableCultures = GetAvailableCultures();
            // check if current culture belongs to available cultures
            return listOfAvailableCultures.Contains(Thread.CurrentThread.CurrentUICulture);
        }

        private static ReadOnlyCollection<CultureInfo> GetAvailableCultures()
        {
            List<CultureInfo> list = new List<CultureInfo>();

            string startupDir = Application.StartupPath;
            Assembly asm = Assembly.GetEntryAssembly();

            CultureInfo neutralCulture = CultureInfo.InvariantCulture;
            if (asm != null)
            {
                NeutralResourcesLanguageAttribute attr = Attribute.GetCustomAttribute(asm, typeof(NeutralResourcesLanguageAttribute)) as NeutralResourcesLanguageAttribute;
                if (attr != null)
                    neutralCulture = CultureInfo.GetCultureInfo(attr.CultureName);
            }
            list.Add(neutralCulture);

            if (asm != null)
            {
                string baseName = asm.GetName().Name;
                foreach (string dir in Directory.GetDirectories(startupDir))
                {
                    // Check that the directory name is a valid culture
                    DirectoryInfo dirinfo = new DirectoryInfo(dir);
                    CultureInfo tCulture = null;
                    try
                    {
                        tCulture = CultureInfo.GetCultureInfo(dirinfo.Name);
                    }
                    // Not a valid culture : skip that directory
                    catch (ArgumentException)
                    {
                        continue;
                    }

                    // Check that the directory contains satellite assemblies
                    if (dirinfo.GetFiles(baseName + ".resources.dll").Length > 0)
                    {
                        list.Add(tCulture);
                    }

                }
            }
            return list.AsReadOnly();
        }
        #endregion

        #region Exception reporting
        /// <summary>
        /// Report an exception
        /// </summary>
        public static void SendCrashReport(Exception exception)
        {
            var reportCrash = new ReportCrash("treedim@gmail.com")
            {
                FromEmail = "treedim@gmail.com",
                SmtpHost = "smtp.gmail.com",
                Port = 587,
                UserName = "treedim@gmail.com",
                Password = "Knowledge_1",
                EnableSSL = true,
            };

            reportCrash.Send(exception);
        }
        #endregion

        #region Contextual help
        internal class MessageFilter : IMessageFilter
        {
            #region IMessageFilter Members
            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                // Use a switch so we can trap other messages in the future
                switch (m.Msg)
                {
                    case 0x100: // WM_KEYDOWN

                        if ((int)m.WParam == (int)Keys.F1)
                        {
                            HelpUtility.ProcessHelpRequest(Control.FromHandle(m.HWnd));
                            return true;
                        }
                        break;
                }
                return false;
            }
            #endregion
        }

        public static class HelpUtility
        {
            public static void ProcessHelpRequest(Control ctrContext)
            {
                ShowContextHelp(ctrContext);
            }
        }

        // Process a request to display help
        // for the context specified by ctrContext.
        public static void ShowContextHelp(Control ctrContext)
        {
            int i=0;
            Control ctr = ctrContext;
            Form form = null;
            while (i < 100 && null == form)
            {
                ctr = ctr.Parent;
                form = ctr as Form;
                ++i;
            }
            if (null != form)
            {
                string sHTMLHelp = form.GetType().Name + ".html";
                Help.ShowHelp(ctrContext
                    , Path.ChangeExtension(Application.ExecutablePath, "chm")
                    , HelpNavigator.Topic
                    , sHTMLHelp);
            }
        }
        #endregion

        #region File association
        private static void RegisterFileType()
        {
            try
            {
                FileAssociation FA = new FileAssociation()
                {
                    Extension = "stb",
                    ContentType = "application/xml",
                    FullName = "TreeDiM StackBuilder Files",
                    ProperName = "StackBuilder File"
                };
                FA.AddCommand("open", Assembly.GetExecutingAssembly().Location + " \"%1\"");
                FA.IconPath = Assembly.GetExecutingAssembly().Location;
                FA.IconIndex = 0;
                FA.Create();
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("File association failed! Exception : {0}", ex.Message));
            }
        }
        #endregion

        #region Webbrowser
        public static void EnsureBrowserEmulationEnabled(string exename = "treeDiM.StackBuilder.Desktop.exe", bool uninstall = false)
        {

            try
            {
                using (
                    var rk = Registry.CurrentUser.OpenSubKey(
                            @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION", true)
                )
                {
                    if (!uninstall)
                    {
                        dynamic vv = rk.GetValue(exename);
                        if (vv == null)
                            rk.SetValue(exename, (uint)11001, RegistryValueKind.DWord);
                    }
                    else
                        rk.DeleteValue(exename);
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Private members
        static readonly ILog _log = LogManager.GetLogger(typeof(Program));
        static bool _useDisconnected = false;
        #endregion
    }
}