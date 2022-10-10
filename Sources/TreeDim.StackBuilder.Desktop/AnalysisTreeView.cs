#region Using directives
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Desktop.Properties;

using log4net;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    #region AnalysisTreeView
    /// <summary>
    /// AnalysisTreeView : left frame treeview control
    /// </summary>
    public partial class AnalysisTreeView
        : TreeView, IDocumentListener
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public AnalysisTreeView()
        {
            try
            {
                // build image list for tree
                ImageList = new ImageList();
                ImageList.Images.Add(CLSDFOLD);                    // 0
                ImageList.Images.Add(OPENFOLD);                    // 1
                ImageList.Images.Add(DOC);                         // 2
                ImageList.Images.Add(Box);                         // 3
                ImageList.Images.Add(Case);                        // 4
                ImageList.Images.Add(Bundle);                      // 5
                ImageList.Images.Add(Cylinder);                    // 6
                ImageList.Images.Add(Pallet);                      // 7
                ImageList.Images.Add(Interlayer);                  // 8
                ImageList.Images.Add(Truck);                       // 9
                ImageList.Images.Add(PalletCorners);               // 10
                ImageList.Images.Add(PalletCap);                   // 11
                ImageList.Images.Add(PalletFilm);                  // 12
                ImageList.Images.Add(Pack);                        // 13
                ImageList.Images.Add(AnalysisCasePallet);          // 14
                ImageList.Images.Add(AnalysisBundlePallet);        // 15
                ImageList.Images.Add(AnalysisTruck);               // 16
                ImageList.Images.Add(AnalysisCase);                // 17
                ImageList.Images.Add(AnalysisStackingStrength);    // 18
                ImageList.Images.Add(AnalysisCylinderPallet);      // 19
                ImageList.Images.Add(AnalysisHCylinderPallet);     // 20
                ImageList.Images.Add(AnalysisPackPallet);          // 21
                ImageList.Images.Add(Bottle);                      // 22
                ImageList.Images.Add(Bag);                         // 23
                ImageList.Images.Add(PalletLabel);                 // 24
                ImageList.Images.Add(PalletsOnPallet);             // 25
               // instantiate context menu
                ContextMenuStrip = new ContextMenuStrip();
                // attach event handlers
                NodeMouseClick += new TreeNodeMouseClickEventHandler(OnNodeMouseClick);
                NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(OnNodeMouseDoubleClick);
                ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
                DrawMode = TreeViewDrawMode.OwnerDrawText;
                DrawNode += new DrawTreeNodeEventHandler(OnDrawNode);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private int ToIconIndex(ItemBase item)
        {
            if (item is AnalysisCasePallet)
            {
                AnalysisCasePallet analysisCasePallet = item as AnalysisCasePallet;
                Packable content = analysisCasePallet.Content;
                if (content is BoxProperties || content is BagProperties || content is LoadedCase) return 14;
                else if (content is BundleProperties) return 15;
                else if (content is PackProperties) return 21;
                else return 0;
            }
            else if (item is AnalysisBoxCase) return 17;
            else if (item is AnalysisPalletTruck) return 16;
            else if (item is AnalysisCaseTruck) return 16;
            else if (item is AnalysisCylinderTruck) return 16;
            else if (item is AnalysisHCylTruck) return 16;
            else if (item is AnalysisCylinderPallet) return 19;
            else if (item is AnalysisHCylPallet) return 20;
            else if (item is AnalysisCylinderCase) return 17;
            else if (item is HAnalysisPallet) return 14;
            else if (item is HAnalysisCase) return 17;
            else if (item is HAnalysisTruck) return 16;
            else if (item is AnalysisPalletsOnPallet) return 25;
            else if (item is AnalysisPalletColumn) return 25;
            else
            {
                _log.Error($"Unexpected analysis type = {item.GetType()}");
                return 0;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // AnalysisTreeView
            // 
            DrawMode = TreeViewDrawMode.OwnerDrawText;
            ResumeLayout(false);

        }
        #endregion

        #region Context menu strip
        /// <summary>
        /// Handler for ContextMenu.Popup event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                // retrieve node which was clicked
                TreeNode node = GetNodeAt(PointToClient(Cursor.Position));
                if (node == null) return; // user might right click no valid node
                SelectedNode = node;
                // clear previous items
                ContextMenuStrip.Items.Clear();
                // let the provider do his work
                if (node.Tag is NodeTag nodeTag)
                    QueryContextMenuItems(nodeTag, ContextMenuStrip);
                // set Cancel to false. 
                // it is optimized to true based on empty entry.
                e.Cancel = !(ContextMenuStrip.Items.Count > 0);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void QueryContextMenuItems(NodeTag nodeTag, ContextMenuStrip contextMenuStrip)
        {
           var types = new List<NodeTag.NodeType>(
                new NodeTag.NodeType[]
                {
                NodeTag.NodeType.NT_BOX
                , NodeTag.NodeType.NT_CASE
                , NodeTag.NodeType.NT_BAG
                , NodeTag.NodeType.NT_PACK
                , NodeTag.NodeType.NT_CASEOFBOXES
                , NodeTag.NodeType.NT_CYLINDER
                , NodeTag.NodeType.NT_BOTTLE
                , NodeTag.NodeType.NT_PALLET
                , NodeTag.NodeType.NT_BUNDLE
                , NodeTag.NodeType.NT_INTERLAYER
                , NodeTag.NodeType.NT_TRUCK
                , NodeTag.NodeType.NT_PALLETCORNERS
                , NodeTag.NodeType.NT_PALLETCAP
                , NodeTag.NodeType.NT_PALLETFILM
                , NodeTag.NodeType.NT_PALLETLABEL
                }
                );

            if (nodeTag.Type == NodeTag.NodeType.NT_DOCUMENT)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOX, Box, new EventHandler(OnCreateNewBox)) { ImageTransparentColor = Color.White } );
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCASE, Case, new EventHandler(OnCreateNewCase)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBAG, Bag, new EventHandler(OnCreateNewBag)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBUNDLE, Bundle, new EventHandler(OnCreateNewBundle)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDER, Cylinder, new EventHandler(OnCreateNewCylinder)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOTTLE, Bottle, new EventHandler(OnCreateNewCylinder)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLET, Pallet, new EventHandler(OnCreateNewPallet)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWINTERLAYER, Interlayer, new EventHandler(OnCreateNewInterlayer)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCORNERS, PalletCorners, new EventHandler(OnCreateNewPalletCorners)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCAP, PalletCap, new EventHandler(OnCreateNewPalletCap)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETFILM, PalletFilm, new EventHandler(OnCreateNewPalletFilm)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETLABEL, PalletLabel, new EventHandler(OnCreateNewPalletLabel)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCK, Truck, new EventHandler(OnCreateNewTruck)) { ImageTransparentColor = Color.White });

                if (((DocumentSB)nodeTag.Document).CanCreateAnalysisCasePallet || ((DocumentSB)nodeTag.Document).CanCreateOptiCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripSeparator());
                if (((DocumentSB)nodeTag.Document).CanCreateAnalysisCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWANALYSIS, AnalysisCasePallet, new EventHandler(OnCreateNewAnalysisCasePallet)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_CLOSE, null, new EventHandler(OnDocumentClose)));
            }
 
            if (types.Contains(nodeTag.Type))
            {
                string message = string.Format(Resources.ID_DELETEITEM, nodeTag.ItemProperties.Name);
                contextMenuStrip.Items.Add(new ToolStripMenuItem(message, DELETE, new EventHandler(OnDeleteBaseItem)));
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTBOX)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOX, Box, new EventHandler(OnCreateNewBox)) { ImageTransparentColor = Color.White });
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTCASE)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCASE, Case, new EventHandler(OnCreateNewCase)) { ImageTransparentColor = Color.White });
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTCYLINDER)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDER, Cylinder, new EventHandler(OnCreateNewCylinder)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBOTTLE, Bottle, new EventHandler(OnCreateNewBottle)) { ImageTransparentColor = Color.White });
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLET)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLET, Pallet, new EventHandler(OnCreateNewPallet)) { ImageTransparentColor = Color.White });
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTBUNDLE)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWBUNDLE, Bundle, new EventHandler(OnCreateNewBundle)) { ImageTransparentColor = Color.White });
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTTRUCK)
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCK, Truck, new EventHandler(OnCreateNewTruck)) { ImageTransparentColor = Color.White });
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTPALLETACCESSORIES)
            {
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWINTERLAYER, Interlayer, new EventHandler(OnCreateNewInterlayer)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCORNERS, PalletCorners, new EventHandler(OnCreateNewPalletCorners)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETCAP, PalletCap, new EventHandler(OnCreateNewPalletCap)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETFILM, PalletFilm, new EventHandler(OnCreateNewPalletFilm)) { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWPALLETLABEL, PalletFilm, new EventHandler(OnCreateNewPalletLabel)) { ImageTransparentColor = Color.White });
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_LISTANALYSIS)
            {
                if (nodeTag.Document.CanCreateAnalysisCasePallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWANALYSIS, AnalysisCasePallet, new EventHandler(OnCreateNewAnalysisCasePallet)) { ImageTransparentColor = Color.White });
                if (nodeTag.Document.CanCreateAnalysisCylinderPallet)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWCYLINDERANALYSIS, AnalysisCylinderPallet, new EventHandler(OnCreateNewAnalysisCylinderPallet)) { ImageTransparentColor = Color.White });
                if (nodeTag.Document.CanCreateAnalysisPalletTruck)
                    contextMenuStrip.Items.Add(new ToolStripMenuItem(Resources.ID_ADDNEWTRUCKANALYSIS, AnalysisTruck, new EventHandler(OnCreateNewAnalysisPalletTruck)) { ImageTransparentColor = Color.White });
            }
            else if (nodeTag.Type == NodeTag.NodeType.NT_ANALYSIS)
            {
                string analysisName = string.Empty;
                if (null != nodeTag.Analysis)
                    analysisName = nodeTag.Analysis.Name;
                else if (null != nodeTag.HAnalysis)
                    analysisName = nodeTag.HAnalysis.Name;

                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_EDIT, analysisName), null, new EventHandler(OnEditAnalysis))
                { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_DELETEITEM, analysisName), DELETE, new EventHandler(OnDeleteBaseItem))
                { ImageTransparentColor = Color.White });
                contextMenuStrip.Items.Add(new ToolStripMenuItem(
                    string.Format(Resources.ID_GENERATEREPORT, analysisName), WORD, new EventHandler(OnAnalysisReport))
                { ImageTransparentColor = Color.White });
            }
        }
        #endregion

        #region Handling context menus
        private void OnDeleteBaseItem(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;                
                tag.Document.RemoveItem(tag.ItemProperties);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnAnalysisReport(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                SolutionReportClicked(this, new AnalysisTreeViewEventArgs(tag));
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnAnalysisExportCollada(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                SolutionColladaExportClicked(this, new AnalysisTreeViewEventArgs(tag));
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBox(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBoxUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewCase(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewCaseUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBag(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBagUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPack(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPackUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewCylinder(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewCylinderUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBottle(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBottleUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewInterlayer(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewInterlayerUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewBundle(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewBundleUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewTruck(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewTruckUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletCorners(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletCornersUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletCap(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletCapUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletFilm(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletFilmUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewPalletLabel(object sender, EventArgs e)
        { 
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewPalletLabelUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }       
        }
        private void OnCreateNewAnalysisCasePallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisCasePalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewAnalysisCylinderPallet(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisCylinderPalletUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnCreateNewAnalysisPalletTruck(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                ((DocumentSB)tag.Document).CreateNewAnalysisPalletTruckUI();
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnDocumentClose(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                CancelEventArgs cea = new CancelEventArgs();
                FormMain.GetInstance().CloseDocument((DocumentSB)tag.Document, cea);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        private void OnEditAnalysis(object sender, EventArgs e)
        {
            try
            {
                NodeTag tag = SelectedNode.Tag as NodeTag;
                DocumentSB doc = tag.Document as DocumentSB;
                if (null != tag.Analysis)
                    doc.EditAnalysis(tag.Analysis);
                else if (null != tag.AnalysisPalletsOnPallet)
                    doc.EditAnalysis(tag.AnalysisPalletsOnPallet);
                else if (null != tag.AnalysisPalletColumn)
                    doc.EditAnalysis(tag.AnalysisPalletColumn);
            }
            catch (Exception ex) { _log.Error(ex.ToString()); }
        }
        #endregion

        #region Event handlers
        void OnNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                SelectedNode = e.Node;
                // handle only left mouse button click
                if (e.Button != MouseButtons.Left) return;
                NodeTag tag = CurrentTag;
                NodeTag.NodeType tagType = tag.Type;
                if (null != AnalysisNodeClicked &&
                    (tag.Type == NodeTag.NodeType.NT_ANALYSIS)
                    || (tag.Type == NodeTag.NodeType.NT_ECTANALYSIS)
                    || (tag.Type == NodeTag.NodeType.NT_BOX)
                    || (tag.Type == NodeTag.NodeType.NT_CASE)
                    || (tag.Type == NodeTag.NodeType.NT_BAG)
                    || (tag.Type == NodeTag.NodeType.NT_PACK)
                    || (tag.Type == NodeTag.NodeType.NT_BUNDLE)
                    || (tag.Type == NodeTag.NodeType.NT_CYLINDER)
                    || (tag.Type == NodeTag.NodeType.NT_BOTTLE)
                    || (tag.Type == NodeTag.NodeType.NT_CASEOFBOXES)
                    || (tag.Type == NodeTag.NodeType.NT_PALLET)
                    || (tag.Type == NodeTag.NodeType.NT_INTERLAYER)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETCORNERS)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETCAP)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETFILM)
                    || (tag.Type == NodeTag.NodeType.NT_PALLETLABEL)
                    || (tag.Type == NodeTag.NodeType.NT_TRUCK)
                    )
                {
                    AnalysisNodeClicked(this, new AnalysisTreeViewEventArgs(tag));
                    e.Node.Expand();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        void OnNodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
        }
        void OnDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            try
            {
                // get NodeTag
                if (!(e.Node.Tag is NodeTag tag))
                    throw new Exception(string.Format("Node {0} has null tag", e.Node.Text));
                Rectangle nodeBounds = e.Node.Bounds;
                if (null != tag.ItemProperties)
                    TextRenderer.DrawText(e.Graphics, tag.ItemProperties.Name, Font, nodeBounds, Color.Black, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.NoClipping);
                else
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, Font, nodeBounds, Color.Black, Color.Transparent, TextFormatFlags.VerticalCenter | TextFormatFlags.NoClipping);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        #endregion

        #region Helpers
        internal NodeTag CurrentTag
        {
            get
            {
                TreeNode currentNode = SelectedNode;
                if (null == currentNode)
                    throw new Exception("No node selected");
                return currentNode.Tag as NodeTag;
            }
        }
        internal TreeNode FindNode(TreeNode node, NodeTag nodeTag)
        {
            // check with node itself
            if (null != node)
            {
                if (!(node.Tag is NodeTag tag))
                {
                    _log.Error(string.Format("Node {0} has no valid NodeTag", node.Text));
                    return null;
                }
                if (tag.Equals(nodeTag))
                    return node;
            }
            // check with child nodes
            TreeNodeCollection tnCollection = null == node ? Nodes : node.Nodes;
            foreach (TreeNode tn in tnCollection)
            {
                TreeNode tnResult = FindNode(tn, nodeTag);
                if (null != tnResult)
                    return tnResult;
            }
            return null;
        }
        #endregion

        #region Delegates
        /// <summary>
        /// is a prototype for event handlers of AnalysisNodeClicked / SolutionReportNodeClicked
        /// </summary>
        /// <param name="sender">sending object (tree)</param>
        /// <param name="eventArg">contains NodeTag to identify clicked TreeNode</param>
        public delegate void AnalysisNodeClickHandler(object sender, AnalysisTreeViewEventArgs eventArg);
        #endregion

        #region Events
        public event AnalysisNodeClickHandler AnalysisNodeClicked;
        public event AnalysisNodeClickHandler SolutionReportClicked;
        public event AnalysisNodeClickHandler SolutionColladaExportClicked;
        #endregion

        #region IDocumentListener implementation
        /// <summary>
        /// handles new document creation
        /// </summary>
        /// <param name="doc"></param>
        public void OnNewDocument(Document doc)
        {
            doc.DocumentClosed += OnDocumentClosed;

            // add document node
            TreeNode nodeDoc = new TreeNode(doc.Name, 2, 2)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_DOCUMENT, doc)
            };
            Nodes.Add(nodeDoc);
            // add case list node
            TreeNode nodeCases = new TreeNode(Resources.ID_NODE_CASES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTCASE, doc)
            };
            nodeDoc.Nodes.Add(nodeCases);
            // add pack list node
            TreeNode nodePacks = new TreeNode(Resources.ID_NODE_PACKS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPACK, doc)
            };
            nodeDoc.Nodes.Add(nodePacks);
            // add bundle list node
            TreeNode nodeBundles = new TreeNode(Resources.ID_NODE_BUNDLES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTBUNDLE, doc)
            };
            nodeDoc.Nodes.Add(nodeBundles);
            // add cylinder list node
            TreeNode nodeCylinders = new TreeNode(Resources.ID_NODE_CYLINDERS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTCYLINDER, doc)
            };
            nodeDoc.Nodes.Add(nodeCylinders);
            // add pallet list node
            TreeNode nodePallets = new TreeNode(Resources.ID_NODE_PALLETS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLET, doc)
            };
            nodeDoc.Nodes.Add(nodePallets);
            // add pallet accessories list node
            TreeNode nodePalletAccessories = new TreeNode(Resources.ID_NODE_PALLETACCESSORIES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTPALLETACCESSORIES, doc)
            };
            nodeDoc.Nodes.Add(nodePalletAccessories);
            // add truck list node
            TreeNode nodeTrucks = new TreeNode(Resources.ID_NODE_TRUCKS, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTTRUCK, doc)
            };
            nodeDoc.Nodes.Add(nodeTrucks);
            // add analysis list node
            TreeNode nodeAnalyses = new TreeNode(Resources.ID_NODE_ANALYSES, 0, 1)
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc)
            };
            nodeDoc.Nodes.Add(nodeAnalyses);
            nodeDoc.Expand();
        }
        /// <summary>
        /// handles new type creation
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="itemProperties"></param>
        public void OnNewTypeCreated(Document doc, ItemBase itemProperties)
        {
            NodeTag.NodeType nodeType;
            NodeTag.NodeType parentNodeType;
            int iconIndex;
            if (itemProperties.GetType() == typeof(CaseOfBoxesProperties))
            {
                iconIndex = 17;
                nodeType = NodeTag.NodeType.NT_CASEOFBOXES;
                parentNodeType = NodeTag.NodeType.NT_LISTCASE;
            }
            else if (itemProperties is BoxProperties boxProperties)
            {
                iconIndex = boxProperties.CAType == BoxProperties.CreatedAsType.Case ? 4 : 3;
                nodeType = NodeTag.NodeType.NT_CASE;
                parentNodeType = NodeTag.NodeType.NT_LISTCASE;
            }
            else if (itemProperties.GetType() == typeof(BagProperties))
            {
                iconIndex = 23;
                nodeType = NodeTag.NodeType.NT_BAG;
                parentNodeType = NodeTag.NodeType.NT_LISTCASE;
            }
            else if (itemProperties.GetType() == typeof(BundleProperties))
            {
                iconIndex = 5;
                nodeType = NodeTag.NodeType.NT_BUNDLE;
                parentNodeType = NodeTag.NodeType.NT_LISTBUNDLE;
            }
            else if (itemProperties.GetType() == typeof(CylinderProperties))
            {
                iconIndex = 6;
                nodeType = NodeTag.NodeType.NT_CYLINDER;
                parentNodeType = NodeTag.NodeType.NT_LISTCYLINDER;
            }
            else if (itemProperties.GetType() == typeof(BottleProperties))
            {
                iconIndex = 22;
                nodeType = NodeTag.NodeType.NT_BOTTLE;
                parentNodeType = NodeTag.NodeType.NT_LISTCYLINDER;
            }
            else if (itemProperties.GetType() == typeof(PalletProperties))
            {
                iconIndex = 7;
                nodeType = NodeTag.NodeType.NT_PALLET;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLET;
            }
            else if (itemProperties.GetType() == typeof(InterlayerProperties))
            {
                iconIndex = 8;
                nodeType = NodeTag.NodeType.NT_INTERLAYER;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETACCESSORIES;
            }
            else if (itemProperties.GetType() == typeof(TruckProperties))
            {
                iconIndex = 9;
                nodeType = NodeTag.NodeType.NT_TRUCK;
                parentNodeType = NodeTag.NodeType.NT_LISTTRUCK;
            }
            else if (itemProperties.GetType() == typeof(PalletCornerProperties))
            {
                iconIndex = 10;
                nodeType = NodeTag.NodeType.NT_PALLETCORNERS;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETACCESSORIES;
            }
            else if (itemProperties.GetType() == typeof(PalletCapProperties))
            {
                iconIndex = 11;
                nodeType = NodeTag.NodeType.NT_PALLETCAP;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETACCESSORIES;
            }
            else if (itemProperties.GetType() == typeof(PalletFilmProperties))
            {
                iconIndex = 12;
                nodeType = NodeTag.NodeType.NT_PALLETFILM;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETACCESSORIES;
            }
            else if (itemProperties.GetType() == typeof(PalletLabelProperties))
            {
                iconIndex = 24;
                nodeType = NodeTag.NodeType.NT_PALLETLABEL;
                parentNodeType = NodeTag.NodeType.NT_LISTPALLETACCESSORIES;

            }
            else if (itemProperties.GetType() == typeof(PackProperties))
            {
                iconIndex = 13;
                nodeType = NodeTag.NodeType.NT_PACK;
                parentNodeType = NodeTag.NodeType.NT_LISTPACK;
            }
            else
            {
                Debug.Assert(false);
                _log.Error("AnalysisTreeView.OnNewTypeCreated() -> unknown type!");
                return;
            }
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(parentNodeType, doc));
            if (null == parentNode)
            { 
                _log.Error(string.Format("Failed to load parentNode for {0}", itemProperties.Name));
                return;
            }
            // instantiate node
            TreeNode nodeItem = new TreeNode(itemProperties.Name, iconIndex, iconIndex)
            {
                // set node tag
                Tag = new NodeTag(nodeType, doc, itemProperties)
            };
            // insert
            parentNode.Nodes.Add(nodeItem);
            parentNode.Expand();
            // if item is CaseOfBoxesProperties
            if (itemProperties is CaseOfBoxesProperties)
            {
                // insert sub node
                CaseOfBoxesProperties caseOfBoxesProperties = itemProperties as CaseOfBoxesProperties;
                TreeNode subNode = new TreeNode(caseOfBoxesProperties.InsideBoxProperties.Name, 3, 3)
                {
                    Tag = new NodeTag(NodeTag.NodeType.NT_BOX, doc, caseOfBoxesProperties.InsideBoxProperties)
                };
                nodeItem.Nodes.Add(subNode);
            }
        }
        public void OnNewAnalysisCreated(Document doc, Analysis analysis)
        {
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc));
            // instantiate analysis node
            TreeNode nodeAnalysis = new TreeNode(analysis.Name, ToIconIndex(analysis), ToIconIndex(analysis))
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis)
            };
            // insert context menu
            parentNode.Nodes.Add( nodeAnalysis );
            parentNode.Expand();
        }
        public void OnAnalysisUpdated(Document doc, Analysis analysis)
        {
            // get parent node
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            if (null != analysisNode)
                analysisNode.Name = analysis.Name;
        }
        public void OnNewAnalysisCreated(Document doc, AnalysisHetero analysis)
        {
            // get parent node
            TreeNode parentNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_LISTANALYSIS, doc));
            // instantiate analysis node
            TreeNode nodeAnalysis = new TreeNode(analysis.Name, ToIconIndex(analysis), ToIconIndex(analysis))
            {
                Tag = new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis)
            };
            // insert context menu
            parentNode.Nodes.Add( nodeAnalysis );
            parentNode.Expand();
        }
        public void OnAnalysisUpdated(Document doc, AnalysisHetero analysis)
        {
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            if (null != analysisNode)
                analysisNode.Name = analysis.Name;
        }
        #endregion

        #region Remove functions
        /// <summary>
        /// handles new type removed
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="itemBase"></param>
        public void OnTypeRemoved(Document doc, ItemBase itemBase)
        {
            NodeTag.NodeType nodeType = NodeTag.NodeType.NT_UNKNOWN;
            if (itemBase.GetType() == typeof(BoxProperties))
            {
                BoxProperties box = itemBase as BoxProperties;
                if (box.IsCase)
                    nodeType = NodeTag.NodeType.NT_CASE;
                else
                    nodeType = NodeTag.NodeType.NT_BOX;
            }
            else if (itemBase.GetType() == typeof(BagProperties))
                nodeType = NodeTag.NodeType.NT_BAG;
            else if (itemBase.GetType() == typeof(BundleProperties))
                nodeType = NodeTag.NodeType.NT_BUNDLE;
            else if (itemBase.GetType() == typeof(PackProperties))
                nodeType = NodeTag.NodeType.NT_PACK;
            else if (itemBase.GetType() == typeof(CaseOfBoxesProperties))
                nodeType = NodeTag.NodeType.NT_CASEOFBOXES;
            else if (itemBase.GetType() == typeof(InterlayerProperties))
                nodeType = NodeTag.NodeType.NT_INTERLAYER;
            else if (itemBase.GetType() == typeof(PalletCornerProperties))
                nodeType = NodeTag.NodeType.NT_PALLETCORNERS;
            else if (itemBase.GetType() == typeof(PalletCapProperties))
                nodeType = NodeTag.NodeType.NT_PALLETCAP;
            else if (itemBase.GetType() == typeof(PalletFilmProperties))
                nodeType = NodeTag.NodeType.NT_PALLETFILM;
            else if (itemBase.GetType() == typeof(PalletLabelProperties))
                nodeType = NodeTag.NodeType.NT_PALLETLABEL;
            else if (itemBase.GetType() == typeof(PalletProperties))
                nodeType = NodeTag.NodeType.NT_PALLET;
            else if (itemBase.GetType() == typeof(TruckProperties))
                nodeType = NodeTag.NodeType.NT_TRUCK;
            else if (itemBase.GetType() == typeof(CylinderProperties))
                nodeType = NodeTag.NodeType.NT_CYLINDER;
            else if (itemBase.GetType() == typeof(BottleProperties))
                nodeType = NodeTag.NodeType.NT_BOTTLE;
            Debug.Assert(nodeType != NodeTag.NodeType.NT_UNKNOWN);
            if (nodeType == NodeTag.NodeType.NT_UNKNOWN)
                return; // ->not found exit
            // get node
            TreeNode typeNode = FindNode(null, new NodeTag(nodeType, doc, itemBase));
            // remove node
            if (null != typeNode)
                Nodes.Remove(typeNode);
        }

        public void OnAnalysisRemoved(Document doc, ItemBase analysis)
        {
            // get node
            TreeNode analysisNode = FindNode(null, new NodeTag(NodeTag.NodeType.NT_ANALYSIS, doc, analysis));
            // test
            if (null == analysisNode)
            {
                _log.Error(
                    string.Format("Failed to find a valid tree node for analysis {0}"
                    , analysis.Name));
                return;
            }
            // remove node
            Nodes.Remove(analysisNode);
        }
        /// <summary>
        /// handles document closing event by removing the corresponding document node in TreeView
        /// </summary>
        public void OnDocumentClosed(Document doc)
        {
            NodeTag.NodeType nodeType = NodeTag.NodeType.NT_DOCUMENT;
            // get node
            TreeNode docNode = FindNode(null, new NodeTag(nodeType, doc));
            // remove node
            Nodes.Remove(docNode);

            doc.DocumentClosed -= OnDocumentClosed;
        }
        #endregion

        #region Data members
        static readonly ILog _log = LogManager.GetLogger(typeof(AnalysisTreeView));
        #endregion
    }
    #endregion

    #region NodeTag class
    /// <summary>
    /// NodeTag will be used for each TreeNode.Tag
    /// </summary>
    public class NodeTag
    {
        #region Enums
        /// <summary>
        /// AnalysisTreeView node types
        /// </summary>
        public enum NodeType
        {
            /// <summary>
            /// document
            /// </summary>
            NT_DOCUMENT,
            /// <summary>
            /// list of boxes
            /// </summary>
            NT_LISTBOX,
            /// <summary>
            /// list of cases
            /// </summary>
            NT_LISTCASE,
            /// <summary>
            /// list of pack
            /// </summary>
            NT_LISTPACK,
            /// <summary>
            /// list of cylinders
            /// </summary>
            NT_LISTCYLINDER,
            /// <summary>
            /// list of bundles
            /// </summary>
            NT_LISTBUNDLE,
            /// <summary>
            /// list of palets
            /// </summary>
            NT_LISTPALLET,
            /// <summary>
            /// list of trucks
            /// </summary>
            NT_LISTTRUCK,
            /// <summary>
            /// list of analyses
            /// </summary>
            NT_LISTANALYSIS,
            /// <summary>
            /// list of pallet accessories
            /// </summary>
            NT_LISTPALLETACCESSORIES,
            /// <summary>
            /// box
            /// </summary>
            NT_BOX,
            /// <summary>
            /// case
            /// </summary>
            NT_CASE,
            /// <summary>
            /// bag / rounded box
            /// </summary>
            NT_BAG,
            /// <summary>
            /// pack
            /// </summary>
            NT_PACK,
            /// <summary>
            /// case of boxes
            /// </summary>
            NT_CASEOFBOXES,
            /// <summary>
            /// bundle
            /// </summary>
            NT_BUNDLE,
            /// <summary>
            /// cylinder
            /// </summary>
            NT_CYLINDER,
            /// <summary>
            /// bottle
            /// </summary>
            NT_BOTTLE,
            /// <summary>
            /// pallet
            /// </summary>
            NT_PALLET,
            /// <summary>
            /// interlayer
            /// </summary>
            NT_INTERLAYER,
            /// <summary>
            /// truck
            /// </summary>
            NT_TRUCK,
            /// <summary>
            /// pallet corners
            /// </summary>
            NT_PALLETCORNERS,
            /// <summary>
            /// pallet cap
            /// </summary>
            NT_PALLETCAP,
            /// <summary>
            /// pallet film
            /// </summary>
            NT_PALLETFILM,
            /// <summary>
            /// pallet label
            /// </summary>
            NT_PALLETLABEL,
            /// <summary>
            /// analysis
            /// </summary>
            NT_ANALYSIS,
            /// <summary>
            /// analysis box
            /// </summary>
            NT_ANALYSISBOX,
            /// <summary>
            /// analysis pack
            /// </summary>
            NT_ANALYSISPACK,
            /// <summary>
            /// analysis pallet
            /// </summary>
            NT_ANALYSISPALLET,
            /// <summary>
            /// analysis interlayer
            /// </summary>
            NT_ANALYSISINTERLAYER,
            /// <summary>
            /// analysis pallet corners
            /// </summary>
            NT_ANALYSISPALLETCORNERS,
            /// <summary>
            /// analysis pallet cap
            /// </summary>
            NT_ANALYSISPALLETCAP,
            /// <summary>
            /// analysis pallet film
            /// </summary>
            NT_ANALYSISPALLETFILM,
            /// <summary>
            /// analysis solution
            /// </summary>
            NT_CASEPALLETANALYSISSOLUTION,
            /// <summary>
            /// pack/pallet analysis solution
            /// </summary>
            NT_PACKPALLETANALYSISSOLUTION,
            /// <summary>
            /// cylinder pallet analysis solution
            /// </summary>
            NT_CYLINDERPALLETANALYSISSOLUTION,
            /// <summary>
            /// hcylinder pallet analysis solution
            /// </summary>
            NT_HCYLINDERPALLETANALYSISSOLUTION,
            /// <summary>
            /// analysis report
            /// </summary>
            NT_ANALYSISSOLREPORT,
            /// <summary>
            /// truck analysis
            /// </summary>
            NT_TRUCKANALYSIS,
            /// <summary>
            /// truck analysis solution
            /// </summary>
            NT_TRUCKANALYSISSOL,
            /// <summary>
            /// case analysis
            /// </summary>
            NT_BOXCASEPALLETANALYSIS,
            /// <summary>
            /// case analysis solution
            /// </summary>
            NT_CASESOLUTION,
            /// <summary>
            /// ECT analysis (Edge Crush Test)
            /// </summary>
            NT_ECTANALYSIS,
            /// <summary>
            /// box/case analysis
            /// </summary>
            NT_ANALYSISBOXCASE,
            /// <summary>
            /// box/case analysis case
            /// </summary>
            NT_BOXCASEANALYSISCASE,
            /// <summary>
            /// box/case analysis box
            /// </summary>
            NT_BOXCASEANALYSISBOX,
            /// <summary>
            /// box/case analysis solution
            /// </summary>
            NT_BOXCASEANALYSISSOLUTION,
            /// <summary>
            /// unknown
            /// </summary>
            NT_UNKNOWN
        }
        #endregion

        #region Constructor
        public NodeTag(NodeType type, Document document)
        {
            Type = type;
            Document = document;       
        }
        public NodeTag(NodeType type, Document document, ItemBase itemBase)
        {
            Type = type;
            Document = document;
            ItemProperties = itemBase;
        }
        #endregion

        #region Object method overrides
        public override bool Equals(object obj)
        {
            return obj is NodeTag nodeTag
                && Type == nodeTag.Type
                && Document == nodeTag.Document
                && ItemProperties == nodeTag.ItemProperties;
        }
        public override int GetHashCode()
        {
            return Type.GetHashCode()
                ^ Document.GetHashCode()
                ^ ItemProperties.GetHashCode();
        }
        #endregion

        #region Public properties
        /// <summary>
        /// returns node type
        /// </summary>
        public NodeType Type { get; }
        /// <summary>
        /// returns document adressed 
        /// </summary>
        public Document Document { get; }
        /// <summary>
        /// returns itempProperties (box/palet/interlayer)
        /// </summary>
        public ItemBase ItemProperties { get; }
        /// <summary>
        /// returns analysis if any
        /// </summary>
        public AnalysisHomo Analysis => ItemProperties as AnalysisHomo;
        public AnalysisPalletsOnPallet AnalysisPalletsOnPallet => ItemProperties as AnalysisPalletsOnPallet;
        public AnalysisPalletColumn AnalysisPalletColumn => ItemProperties as AnalysisPalletColumn;
        public AnalysisHetero HAnalysis => ItemProperties as AnalysisHetero;
        #endregion
    }
    #endregion

    #region AnalysisTreeViewEventArgs class
    /// <summary>
    /// EventArg inherited class used as AnalysisNodeClickHandler delegate argument
    /// Encapsulates a reference to a NodeTag
    /// </summary>
    public class AnalysisTreeViewEventArgs : EventArgs
    {
        #region Constructor
        public AnalysisTreeViewEventArgs(NodeTag nodeTag)  { NodeTag = nodeTag; }
        #endregion
        #region Public properties
        public Document Document => NodeTag.Document;
        public AnalysisHomo Analysis => NodeTag.Analysis;
        public AnalysisHetero HAnalysis => NodeTag.HAnalysis;
        public AnalysisPalletsOnPallet AnalysisPalletsOnPallet => NodeTag.AnalysisPalletsOnPallet;
        public AnalysisPalletColumn AnalysisPalletColumn => NodeTag.AnalysisPalletColumn;
        public ItemBase ItemBase => NodeTag.ItemProperties;
        #endregion
        #region Private properties
        private NodeTag NodeTag { get; set; }
        #endregion
    }
    #endregion
}
