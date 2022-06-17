<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ValidationWebGL.aspx.cs" Inherits="treeDiM.StackBuilder.TechnologyBSA_ASPNET.ValidationWebGL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style2 {
            width: 550px;
            text-align: center;
        }
        .auto-style3 {
            width: 500px;
            text-align: left;
        }
        .auto-style4 {
            text-align: right;
        }
        .border {
            border: solid 8px;
            color: lightblue;
        }
        .switch {
            position: relative;
            display: inline-block;
            width: 50px;
            height: 24px;
        }
        .switch input {
            opacity: 0;
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
            height: 16px;
            width: 16px;
            left: 4px;
            bottom: 4px;
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
        .slider.round {
            border-radius: 34px;
        }
        .slider.round:before {
            border-radius: 50%;
        }
    </style>
    <link type="text/css" href="css/default.css" rel="stylesheet" />
    <link href="css/jquery-ui.css" rel="stylesheet" />
    <link href="css/keyboard.css" rel="stylesheet" />
    <script type="text/javascript" src="javascript/jquery.min.js"></script>
    <script type="text/javascript" src="javascript/jquery-ui.min.js"></script>
    <script type="text/javascript" src="javascript/jquery.keyboard.js"></script>
    <script type="text/javascript" src="javascript/jquery.keyboard.extension-typing.js"></script>
    <script type="text/javascript" src="javascript/three.min.js"></script>
    <script type="text/javascript" src="javascript/three-gltf-loader.min.js"></script>
    <script type="text/javascript" src="javascript/model-element.min.js"></script>
    <script type="text/javascript">
        function ActivateVirtualKeyboard() {
            $('#TBFileName').keyboard({
                layout: 'qwerty',
                autoAccept: true,
                enterNavigation: true
            }).addTyping();
            /* Code to get jQuery UI to work with jQuery 3.4+ ... */
            $.isArray = function (a) {
                return Object.prototype.toString.call(a) === '[object Array]';
            }
            $.isFunction = function (f) {
                return typeof f === 'function';
            }
            $.isWindow = function (w) {
                var toString = Object.prototype.toString.call(w);
                return toString == '[object global]' ||
                    toString == '[object Window]' ||
                    toString == '[object DOMWindow]';
            }
        }
    </script>
    <script type="text/javascript">
        function updateStyles() {
            var css;
            if (!css) {
                css = document.createElement('style');
                document.body.appendChild(css);
            }
            let cssText = `
              x-model {
                width: 300px;
                height: 150px;
                transform: rotateX(60deg) rotateY(0deg) rotateZ(${rotateZ.value}deg);
              }`;
            css.textContent = cssText;
        }
    </script>


    <style type="text/css">
    </style>

</head>
<body>
    <form runat="server">
        <div class="div980x780">
            <asp:ScriptManager ID="pageUpdates" runat="server" EnablePartialRendering="true"></asp:ScriptManager>
            <asp:UpdatePanel ID="loadedPallet" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <table class="style100pct">
                        <tr>
                            <td colspan="3" class="auto-style2">
                                <table>
                                    <tr>
                                        <td style="height: 400px; width: 300px; overflow: hidden">
                                            <div id="XModelDiv" class="wrap" runat="server">
                                                <x-model id="XMODEL" class="x-model" src="./Output/Analysis.glb" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input  class="custom-slider" id="rotateZ" type="range" min="0" max="359" value="45" oninput="updateStyles()">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td colspan="2" class="auto-style3">
                                <asp:Panel ID="PanelInterlayer" runat="server" GroupingText="Interlayers" Width="100%" BorderColor="LightGray" BackColor="Transparent" CssClass="groupBox">
                                    <div style="height: 350px; overflow-x:hidden; overflow-y:visible">
                                        <table>
                                            <tr>
                                                <td id="td1" runat="server">
                                                    <asp:Label ID="LayerLabel" Text="Top" Width="100px" runat="server" CssClass="labelRight"/>
                                                </td>
                                                <td id="td2" runat="server">
                                                    <asp:Label ID="Label1" Text="" Width="100px" runat="server" CssClass="labelRight"/>
                                                </td>
                                                <td id="td3" runat="server">
                                                        <label class="switch">
                                                            <asp:CheckBox ID="LayerCheckBoxTop" AutoPostBack="true" Checked="false" runat="server" />
                                                            <span class="slider round" />
                                                        </label>
                                                    </td>
                                            </tr>
                                        </table>
                                        <asp:ListView ID="LVInterlayers" runat="server">
                                            <LayoutTemplate>
                                                <table id="tableInterlayers" runat="server">
                                                    <tr id="itemPlaceholder" runat="server" />
                                                </table>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <tr id="ITInterlayer" runat="server">
                                                    <td id="td1" runat="server">
                                                        <asp:Label ID="LayerLabel" Text='<%#Eval("Name") %>' Width="100px" runat="server" CssClass="labelRight"/>
                                                    </td>
                                                    <td id="td2" runat="server">
                                                        <asp:DropDownList ID="LayerDropDown" AutoPostBack="true" Width="100px" OnSelectedIndexChanged="OnSelectedLayerTypeChanged" SelectedIndex='<%#Eval("LayerIndex") %>' runat="server">
                                                            <asp:ListItem Text="Layer type 1" />
                                                            <asp:ListItem Text="Layer type 2" />
                                                            <asp:ListItem Text="Layer type 3" />
                                                            <asp:ListItem Text="Layer type 4" />
                                                        </asp:DropDownList>
                                                    </td>
                                                    <td id="td3" runat="server">
                                                        <label class="switch">
                                                            <asp:CheckBox ID="LayerCheckBox" AutoPostBack="true" Checked='<%#Eval("HasInterlayer") %>' runat="server" />
                                                            <span class="slider round" />
                                                        </label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="BTPrev" runat="server" Text="&lt; Previous" CssClass="buttonPrev" OnClick="OnPrevious" />
                            </td>
                            <td />
                            <td class="styleTextAlignRight">
                                <asp:Label ID="lbFileName" runat="server" Text="File name" CssClass="labelRight" />
                            </td>
                            <td class="auto-style4">
                                <asp:TextBox ID="TBFileName" runat="server" Width="100%" CssClass="textbox" />
                            </td>
                            <td class="styleTextAlignRight">
                                <asp:Button ID="btExport" runat="server" OnClick="OnExport" Text="Export" CssClass="button" Style="background-image: url('Images/Export.png'); background-repeat: no-repeat" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </form>
</body>
</html>
