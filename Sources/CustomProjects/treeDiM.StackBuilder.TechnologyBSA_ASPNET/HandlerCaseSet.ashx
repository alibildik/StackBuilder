<%@ WebHandler Language="C#" Class="HandlerCaseSet" %>
#region Using directives
using System;
using System.Web;
using System.Drawing;
using System.Linq;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.TechnologyBSA_ASPNET;
#endregion

public class HandlerCaseSet : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        int number = int.Parse(context.Request["number"]);
        Vector3D dimCase = Vector3D.Parse((string)context.Session[SessionVariables.DimCase]);

        if (ImageWidth > 0 && ImageHeight > 0)
        {
            try
            {
                context.Response.Clear();
                context.Response.ContentType = "Image/Png";
                byte[] buffer = GetImage(dimCase,number, MultiCaseImageGenerator.CaseAlignement.SHARING_LENGTH);
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.End();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public bool IsReusable => true;
    private int ImageWidth { get; set; } = 150;
    private int ImageHeight { get; set; } = 150;
    private byte[] GetImage(Vector3D dimCase, int number, MultiCaseImageGenerator.CaseAlignement caseAlignment)
    {
        Vector2D ptMin = Vector2D.Zero;
        Vector2D ptMax = new Vector2D(dimCase.X, number * dimCase.Y);

        var bProperties = new BoxProperties(null, dimCase.X, dimCase.Y, dimCase.Z)
        {
            TapeWidth = new treeDiM.Basics.OptDouble(true, 50.0),
            TapeColor = Color.Tan,
            Facing = 1
        };
        bProperties.SetAllColors(Enumerable.Repeat(Color.Beige, 6).ToArray());

        MultiCaseImageGenerator imageGenerator = new MultiCaseImageGenerator(new Size(ImageWidth, ImageHeight), ptMin, ptMax);
        Bitmap bmp = imageGenerator.GenerateCaseImage(bProperties, number, caseAlignment);
        ImageConverter converter = new ImageConverter();
        return (byte[])converter.ConvertTo(bmp, typeof(byte[]));
    }
}