<%@ Page Language="C#" AutoEventWireup="true" CodeFile="LayerDesign.aspx.cs" Inherits="LayerDesign" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Layer design page</title>
    <style>
        .switch {
    position: relative;
    display: inline-block;
    width: 50px;
    height: 25px;
  }
  
  .switch input { 
    opacity: 0;
    width: 0;
    height: 0;
  }
  
  .slider {
    position: absolute;
    cursor: pointer;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background-color: #ccc;
    -webkit-transition: .4s;
    transition: .4s;
  }
  
  .slider:before {
    position: absolute;
    content: "";
    height: 20px;
    width: 20px;
    left: 3px;
    bottom: 3px;
    background-color: white;
    -webkit-transition: .4s;
    transition: .4s;
  }
  
  input:checked + .slider {
    background-color: #2196F3;
  }
  
  input:focus + .slider {
    box-shadow: 0 0 1px #2196F3;
  }
  
  input:checked + .slider:before {
    -webkit-transform: translateX(26px);
    -ms-transform: translateX(26px);
    transform: translateX(26px);
  }
  
  /* Rounded sliders */
  .slider.round {
    border-radius: 34px;
  }
  
  .slider.round:before {
    border-radius: 50%;
  }

/*END OF TOGGLE SWITCH*/

    </style>
    <link type="text/css" href="css/default.css" rel="stylesheet" />
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/keyboard.css" rel="stylesheet" />
    <script type="text/javascript" src="javascript/jquery.min.js"></script>
    <script type="text/javascript" src="javascript/jquery-ui.min.js"></script>
    <script type="text/javascript" src="javascript/jquery.keyboard.js"></script>
    <script type="text/javascript" src="javascript/jquery.keyboard.extension-typing.js"></script>
    <script type="text/javascript" src="javascript/fabric.min.js"></script>
    <script type="text/javascript" src="javascript/layerDesignHelpers.js"></script>
    <script type="text/javascript">
        var snap = 3;
        var counter = 0;
        var rectLeft = 0;
        var rectTop = 0;
        var canvasWidth = 0;
        var canvasHeight = 0;
        var caseWidth = <%= this.javaSerial.Serialize(this._casePixelWidth)%>;
        var caseHeight = <%= this.javaSerial.Serialize(this._casePixelHeight)%>;
        var boxPositions = <%= this.javaSerial.Serialize(this._boxPositionsJS)%>;
        var palletDimensions = <%= this.javaSerial.Serialize(this._palletDims)%>

        $(window).load(function () {
            var canvas = new fabric.Canvas('canvas', {
                isDrawingMode: false,
                canvasWidth: document.getElementById('canvas').width,
                canvasHeight: document.getElementById('canvas').height,
                rectLeft: 0,
                rectTop: 0,
                perPixelTargetFind: true,
                targetFindTolerance: 1,
                selection: true,
            });

            document.getElementById('canvas').fabric = canvas;

            canvasWidth = canvas.width;
            canvasHeight = canvas.height;

            insertPallet(canvas, palletDimensions.X1, palletDimensions.Y1);

            counter = 0;
            var len = boxPositions.length;
            for (var i = 0; i < len; ++i)
            {
                insertCase(canvas, boxPositions[i].NumberCase, boxPositions[i].X, boxPositions[i].Y, boxPositions[i].Angle);
            }
            
            fabric.util.addListener(canvas.upperCanvasEl, 'click', function (e) {
                var target = canvas.findTarget(e);
                if (target.type === 'group') {
                    target.setCoords();
                    var text = target.item(1);
                    if (text.type === 'text')
                        text.rotate(text.get('angle') + 90);
                }
                target.setCoords();
                canvas.renderAll();
            });
            fabric.util.addListener(canvas.upperCanvasEl, 'dblclick', function (e) {
                var target = canvas.findTarget(e);
                if (target.type === 'group')
                {
                    target.setCoords();
                    target.rotate(mod(target.get('angle') - 90, 360));
                    var text = target.item(1);
                    if (text.type === 'text')
                        text.rotate(text.get('angle') + 90);
                }
                target.setCoords();
                canvas.renderAll();
            });
            canvas.on('object:moving', function (options) {
                options.target.setCoords();
                Move(canvas, options.target);
                options.target.setCoords();
            });            
        });
        function insertCases(number) {
            var canvas = document.getElementById('canvas').fabric;
            insertCase(canvas, number, canvasWidth - (caseWidth + 10), 0, 0);
            canvas.renderAll();
        }
        function rotate() {
            var canvas = document.getElementById('canvas').fabric;
            canvas.getActiveObjects().forEach((obj) => {
                if (obj.type === 'group') {
                    obj.setCoords();
                    obj.rotate(mod(obj.get('angle') - 90, 360));
                    var text = obj.item(1);
                    if (text.type === 'text')
                        text.rotate(text.get('angle') + 90);
                }
                obj.setCoords();
            });
            canvas.renderAll();
        }

        function remove() {
            var canvas = document.getElementById('canvas').fabric;
            canvas.getActiveObjects().forEach((obj) => {
                canvas.remove(obj)
            });
            canvas.discardActiveObject().renderAll();
        }
        function Save() {
            $(document).ready(function () {

                var canvas = document.getElementById('canvas').fabric;
                let boxPositions = [];
                var counter = 0;

                canvas.forEachObject(function (obj) {
                    if (obj.type !== 'group') return;

                    var name = obj.name;
                    var splitName = name.split("_");
                    var indexCase = splitName[2];
                    var numberCase = splitName[1];

                    boxPositions.push(
                        {
                            Index: indexCase,
                            X: obj.left,
                            Y: obj.top,
                            Angle: obj.get('angle'),
                            NumberCase: numberCase
                        }
                    );
                    ++counter;
                });
                $("#HFBoxArray").val(JSON.stringify(boxPositions));
            });
        }
        function snapOnOff() {
            snap = 0;
            var checkBox = document.getElementById("ChkbSnap");
            if (checkBox.checked == true) { snap = 3; } 
        }
    </script>
</head>

<body>
    <form runat="server">
        <div class="div980x780">
            <asp:ScriptManager ID="pageUpdates" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
            <table>
                <tr>
                    <td>
                        <asp:DropDownList ID="DropDownSelectLayer" AutoPostBack="True" Width="100px" OnChange="Save()" OnSelectedIndexChanged="OnSelectedLayerTypeChanged" runat="server">
                            <asp:ListItem Text="Layer type 1" />
                            <asp:ListItem Text="Layer type 2" />
                            <asp:ListItem Text="Layer type 3" />
                            <asp:ListItem Text="Layer type 4" />
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td class="style250px">
                        <asp:Button ID="ButtonMirrorX" CssClass="buttonMirrorX" runat="server" Text="MirrorX" OnClientClick="javascript:Save()" OnClick="OnMirrorX" Style="background-image:url('Images/BnMirrorX.png'); background-repeat:no-repeat"/>
                        <asp:Button ID="ButtonMirrorY" CssClass="buttonMirrorY" runat="server" Text="MirrorY" OnClientClick="javascript:Save()" OnClick="OnMirrorY" Style="background-image:url('Images/BnMirrorY.png'); background-repeat:no-repeat"/>
                        <asp:Button ID="ButtonRotate180" CssClass="buttonRotate180" Text="Rotate 180" runat="server" OnClientClick="javascript:Save()" OnClick="OnRotate180" Style="background-image:url('Images/BnRotate180.png'); background-repeat:no-repeat"/>
                    </td>
                    <td class="style100px">
                        <asp:DropDownList ID="DropDownSelectLayerCopyFrom" AutoPostBack="True" Width="100px" runat="server">
                            <asp:ListItem Text="Layer type 1" />
                            <asp:ListItem Text="Layer type 2" />
                            <asp:ListItem Text="Layer type 3" />
                            <asp:ListItem Text="Layer type 4" />
                        </asp:DropDownList>
                        <asp:Button ID="ButtonCopyFrom" CssClass="buttonCopyFrom" runat="server" Text="Copy From"  OnClientClick="javascript:Save()" OnClick="OnCopy" Style="background-image:url('Images/BnCopy.png'); background-repeat:no-repeat"/>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div class="canvas-container" style="width: 850px; height: 550px; position: relative; user-select: none;">
                            <canvas id="canvas" width="850" height="550" style="border: 1px solid #ccc" runat="server" />
                        </div>
                    </td>
                    <td colspan="1">
                        <table class="style100pct">
                            <tr>
                                <td>
                                    <input id="ButtonAdd1" type="buttonadd1" onclick="JavaScript: insertCases(1);" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="ButtonAdd2" type="buttonadd2" onclick="JavaScript: insertCases(2);" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="ButtonAdd3" type="buttonadd3" onclick="JavaScript: insertCases(3);" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="ButtonAdd4" type="buttonadd4" onclick="JavaScript: insertCases(4);" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="ButtonRotate" type="buttonrotate" onclick="JavaScript: rotate();" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <input id="Remove" type="buttonremove" onclick="JavaScript: remove();" />
                                </td>
                            </tr>
                            <tr>
                                <td><asp:Label id="LblSnap" runat="server" Text="SNAP" CssClass="label"></asp:Label></td>
                            </tr>
                            <tr>
                                <td></td>
                            </tr>
                            <tr>
                                <td>
                                    <label class="switch">
                                        <input id="ChkbSnap" type="checkbox" onclick="JavaScript: snapOnOff();" />
                                        <span class="slider round hide-off"></span>
                                    </label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <div>
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <table class="style100pct">
                            <tr>
                                <td class="style100pct" />
                                &nbsp;
                                <td class="style100pct" />
                                &nbsp;
                            </tr>
                            <tr>
                                <td class="style100pct" />
                                <asp:Button ID="bnPrev" runat="server" CssClass="buttonPrev" OnClick="OnPrevious" Text="&lt; Previous" />
                                <td class="style100pct" />
                                <asp:Button ID="bnNext" runat="server" CssClass="buttonNext" OnClientClick="javascript:Save()" OnClick="OnNext" Text="Next &gt;" />
                                <asp:HiddenField ID="HFBoxArray" ClientIDMode="Static" runat="server" />
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </form>
</body>
</html>
