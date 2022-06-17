#region Using directives
using System;
using System.Web.UI;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Drawing;
using System.IO;

using Newtonsoft.Json;

using Sharp3D.Math.Core;
using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.TechnologyBSA_ASPNET;
#endregion

public partial class LayerDesign : Page
{
    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            CanvasCoordConverter canvasCoord = new CanvasCoordConverter(850, 550, PtMin, PtMax);

            // pallet dimensions
            var palletTopLeft = canvasCoord.PtWorldToCanvas(new Vector2D(0.0, DimPallet.Y));
            var palletBottomRight = canvasCoord.PtWorldToCanvas(new Vector2D(DimPallet.X, 0.0));
            _palletDims.X1 = palletTopLeft.X;
            _palletDims.Y1 = palletTopLeft.Y;
            _palletDims.X2 = palletBottomRight.X;
            _palletDims.Y2 = palletBottomRight.Y;

            // generate images
            _casePixelWidth = (int)(canvasCoord.LengthWorldToCanvas(DimCase.X));
            _casePixelHeight = (int)(canvasCoord.LengthWorldToCanvas(DimCase.Y));


            int palletPixelWidth = (int)(canvasCoord.LengthWorldToCanvas(DimPallet.X));
            int palletPixelHeight = (int)(canvasCoord.LengthWorldToCanvas(DimPallet.Y));

            // generate box position from 
            MultiCaseImageGenerator.GenerateDefaultCaseImage(DimCase, new Size(_casePixelWidth, _casePixelHeight), 1, MultiCaseImageGenerator.CaseAlignement.SHARING_LENGTH, Path.Combine(Output, "case1.png"));
            MultiCaseImageGenerator.GenerateDefaultCaseImage(DimCase, new Size(_casePixelWidth, _casePixelHeight), 2, MultiCaseImageGenerator.CaseAlignement.SHARING_LENGTH, Path.Combine(Output, "case2.png"));
            MultiCaseImageGenerator.GenerateDefaultCaseImage(DimCase, new Size(_casePixelWidth, _casePixelHeight), 3, MultiCaseImageGenerator.CaseAlignement.SHARING_LENGTH, Path.Combine(Output, "case3.png"));
            MultiCaseImageGenerator.GenerateDefaultCaseImage(DimCase, new Size(_casePixelWidth, _casePixelHeight), 4, MultiCaseImageGenerator.CaseAlignement.SHARING_LENGTH, Path.Combine(Output, "case4.png"));
            MultiCaseImageGenerator.GenerateDefaultPalletImage(DimPallet, PalletIndex == 0 ? "EUR" : "EUR2", new Size(palletPixelWidth, palletPixelHeight), Path.Combine(Output, "pallet.png"));

            DropDownSelectLayer.SelectedIndex = 0;

            // Load layer
            LoadLayer(0);
        }
    }
    #endregion
    #region Event handlers
    protected void OnPrevious(object sender, EventArgs e)
    {
        Response.Redirect("LayerDesignIntro.aspx");
    }
    protected void OnNext(object sender, EventArgs e)
    {
        if (SaveLayer(DropDownSelectLayer.SelectedIndex))
            Response.Redirect("ValidationWebGL.aspx");        
    }
    protected bool LoadLayer(int iLayer)
    {
        List<BoxPositionIndexed> boxPositions;
        switch (iLayer)
        {
            case 0: boxPositions = BoxPositions1; break;
            case 1: boxPositions = BoxPositions2; break;
            case 2: boxPositions = BoxPositions3; break;
            case 3: boxPositions = BoxPositions4; break;
            default: boxPositions = null; break;
        }
        return LoadLayer(boxPositions);        
    }
    protected bool LoadLayer(List<BoxPositionIndexed> boxPositions)
    {
        CanvasCoordConverter canvasCoord = new CanvasCoordConverter(850, 550, PtMin, PtMax);

        // pallet dimensions
        var palletTopLeft = canvasCoord.PtWorldToCanvas(new Vector2D(0.0, DimPallet.Y));
        var palletBottomRight = canvasCoord.PtWorldToCanvas(new Vector2D(DimPallet.X, 0.0));
        _palletDims.X1 = palletTopLeft.X;
        _palletDims.Y1 = palletTopLeft.Y;
        _palletDims.X2 = palletBottomRight.X;
        _palletDims.Y2 = palletBottomRight.Y;

        // generate images
        _casePixelWidth = (int)(canvasCoord.LengthWorldToCanvas(DimCase.X));
        _casePixelHeight = (int)(canvasCoord.LengthWorldToCanvas(DimCase.Y));

        if (boxPositions != null)
        {
            // build reduced list of unique box positions indexed
            BoxPositionIndexed.ReduceListBoxPositionIndexed(boxPositions, out List<BoxPositionIndexed> listBPIReduced, out Dictionary<int, int> dictIndexNumber);
            _boxPositionsJS.Clear();
            foreach (var bp in listBPIReduced)
                _boxPositionsJS.Add(canvasCoord.BPosWorldToCanvas(bp, dictIndexNumber[bp.Index], DimCase));
            return true;
        }
        else
            return false;
    }
    protected bool SaveLayer(int iLayer)
    {
        // instantiate canvasCoord
        var canvasCoord = new CanvasCoordConverter(850, 550, PtMin, PtMax);
        // read box positions serialized as Json in field HFBoxArray
        string sValue = HFBoxArray.Value;
        var bposJS = JsonConvert.DeserializeObject<IList<BoxPositionJS>>(sValue);
        // convert array of BoxPositionJS to BoxPositionIndexed array
        if (null != bposJS)
        {
            var listBoxPositions = new List<BoxPositionIndexed>();
            int localIndex = 0;
            foreach (var bpjs in bposJS)
            {
                var listIndex = canvasCoord.ToBoxPositionIndexed(bpjs, DimCase, localIndex++);
                listBoxPositions.AddRange(listIndex);
            }
            // sort according to index
            BoxPositionIndexed.Sort(ref listBoxPositions);
            // save session wide
            switch (iLayer)
            {
                case 0:  BoxPositions1 = listBoxPositions; break;
                case 1:  BoxPositions2 = listBoxPositions; break;
                case 2:  BoxPositions3 = listBoxPositions; break;
                case 3:  BoxPositions4 = listBoxPositions; break;
                default: break;
            }
            return true;
        }
        else
            return false;
    }
    protected List<BoxPositionIndexed> ListLayerType(int iLayer)
    {
        switch (iLayer)
        {
            case 0: return BoxPositions1;
            case 1: return BoxPositions2;
            case 2: return BoxPositions3;
            case 3: return BoxPositions4;
            default: return null;
        }
    }
    protected List<BoxPositionIndexed> ListBoxPositions
    {
        get
        {
            // instantiate canvasCoord
            var canvasCoord = new CanvasCoordConverter(850, 550, PtMin, PtMax);
            // read box positions serialized as Json in field HFBoxArray
            string sValue = HFBoxArray.Value;
            var bposJS = JsonConvert.DeserializeObject<IList<BoxPositionJS>>(sValue);
            // convert array of BoxPositionJS to BoxPositionIndexed array
            var listBoxPositions = new List<BoxPositionIndexed>();
            if (null != bposJS)
            {
                int localIndex = 0;
                foreach (var bpjs in bposJS)
                {
                    var listIndex = canvasCoord.ToBoxPositionIndexed(bpjs, DimCase, localIndex++);
                    listBoxPositions.AddRange(listIndex);
                }
            }
            return listBoxPositions;
        }
    }
    protected void OnMirrorX(object sender, EventArgs e) => LoadLayer(BoxPositionIndexed.MirrorX(ListBoxPositions, DimPallet, DimCase));
    protected void OnMirrorY(object sender, EventArgs e) => LoadLayer(BoxPositionIndexed.MirrorY(ListBoxPositions, DimPallet, DimCase));
    protected void OnRotate180(object sender, EventArgs e) => LoadLayer(BoxPositionIndexed.Rotate180(ListBoxPositions, DimPallet, DimCase));
    protected void OnCopy(object sender, EventArgs e) => LoadLayer(ListLayerType(DropDownSelectLayerCopyFrom.SelectedIndex));
    protected void OnSelectedLayerTypeChanged(object sender, EventArgs e)
    {
        SaveLayer(CurrentLayerIndex);
        CurrentLayerIndex = DropDownSelectLayer.SelectedIndex;
        LoadLayer(CurrentLayerIndex);
    }
    #endregion
    #region Private properties
    private Vector3D DimCase => Vector3D.Parse((string)Session[SessionVariables.DimCase]);
    private int PalletIndex => (int)Session[SessionVariables.PalletIndex];
    private Vector3D DimPallet => PalletStacking.PalletIndexToDim3D(PalletIndex);
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
    #endregion
    #region Data members
    public PalletDimsJS _palletDims = new PalletDimsJS(); 
    public List<BoxPositionJS> _boxPositionsJS = new List<BoxPositionJS>();
    public int _casePixelWidth;
    public int _casePixelHeight;
    public JavaScriptSerializer javaSerial = new JavaScriptSerializer();
    private string Output => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output");
    private Vector2D PtMin => new Vector2D(-50.0, -50.0);
    private Vector2D PtMax => new Vector2D((DimPallet.Y + DimCase.X) * 850.0 / 550.0, DimPallet.Y + DimCase.X);
    private int CurrentLayerIndex
    {
        get => (int)Session[SessionVariables.CurrentLayerIndex];
        set => Session[SessionVariables.CurrentLayerIndex] = value;
    }
    #endregion
}