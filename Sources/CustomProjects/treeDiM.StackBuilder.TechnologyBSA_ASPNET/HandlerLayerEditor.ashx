<%@ WebHandler Language="C#" Class="HandlerLayerEditor" %>
#region Using directives
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.TechnologyBSA_ASPNET;
#endregion

public class HandlerLayerEditor : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        var sz = new Size((int)context.Session[SessionVariables.ImageWidth], (int)context.Session[SessionVariables.ImageHeight]);
        int selectedIndex =  (int)context.Session[SessionVariables.SelectedIndex];
        var dimCase = Vector3D.Parse((string)context.Session[SessionVariables.DimCase]);
        var dimPallet = PalletStacking.PalletIndexToDim3D((int)context.Session[SessionVariables.PalletIndex]);
        var dimContainer = new Vector2D(dimPallet.X, dimPallet.Y);

        var boxProperties = new BoxProperties(null, dimCase.X, dimCase.Y, dimCase.Z)
        {
            TapeWidth = new treeDiM.Basics.OptDouble(true, 50.0),
            TapeColor = Color.Tan
        };
        boxProperties.SetAllColors(Enumerable.Repeat(Color.Beige, 6).ToArray());

        var layerEditorHelpers = new LayerEditorHelpers(sz, dimCase, dimContainer)
        {
            Positions = BoxPositionIndexed.ToListBoxPosition((List<BoxPositionIndexed>)context.Session[SessionVariables.BoxPositions1]),
            SelectedIndex = selectedIndex,
            FontSizeRatio = ConfigSettings.FontSizeRatio
        };

        ImageConverter converter = new ImageConverter();
        byte[] buffer = (byte[])converter.ConvertTo(layerEditorHelpers.GetLayerImage(boxProperties), typeof(byte[]));
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.End();
    }

    public bool IsReusable => false;
}