<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:param name="lang"/>
  <!-- param set in command line -->
  <xsl:variable name="loc" select="document(concat( $lang, '.xml'), .)/strings" />
  <xsl:output method="html" indent="yes" />
  <xsl:template match="report">
    <html>
      <head>
        <title>
          <xsl:value-of select="name"/>
          <xsl:value-of select="$loc/str[@name='report']"/>          
        </title>
        <style type="text/css">
          .style1
          {
          width: 550.0pt;
          border-top: none;
          border-left: none;
          border-bottom: none;
          border-right: none;
          color: red;
          font-familly: Arial;
          font-size: 72px;
          text-align: center;
          font-style: bold;
          }
          .style2
          {
          width:550.0pt;
          border-top: solid blue 3.0pt;
          border-left: solid blue 3.0pt;
          border-bottom: solid blue 3.0pt;
          border-right: solid blue 3.0pt;
          color: black;
          font-familly: Arial;
          font-size: 24px;
          }
          body
          {
          font-family:Arial;
          font-size:10px;
          margin: 2%;
          width: 98%;
          padding: 0;
          }
          div {
          padding: 5px;
          margin: 5px;
          border: 5px solid #30c;
          width: 600px;
          float: left;
          height: 50px;
          }
          p{
          font-size: 25px;
          }
          #one {
          border-width: 5px;
          background: white;
          }
        </style>
      </head>
      <body lang="FR" style='word-wrap: break-word'>
        <xsl:apply-templates select="analysis"/>
      </body>
    </html>
  </xsl:template>
  <xsl:template match ="analysis">
    <table class="style1" cellpadding="4">
      <tr>
        <td colspan="3">
          <xsl:value-of select="name"/>
        </td>
        <td colspan="1">
          <img width="84" height="84" src="images\logo_CARGOLOG.jpg"/>
        </td>
      </tr>
    </table>
    <br/>
    <br/>
    <xsl:apply-templates select="pallet"/> 
    <xsl:apply-templates select="solution"/>
  </xsl:template>
  <xsl:template match="pallet">
    <table class="style2" cellpadding="4">
      <tr>
        <td colspan="4" >
          <p class="MsoNormal" align="center" style='text-align:center'>
            <span style='font-size:24pt; color:black;'>SUPPORT : </span>
            <span style='font-size:26pt; color:gold;'><xsl:value-of select="name"/></span>
          </p>
        </td>
      </tr>
    </table>
    <br/>
    <br/>
  </xsl:template>

  <xsl:template match="solution">
    <table class="style2" cellpadding="4">
      <tr>
        <td colspan="4" >
          <p class="MsoNormal" align="center" style='text-align:center'>
            <span style='font-size:24pt; color:black;'>Nombre de colis total : </span>
            <span style='font-size:48pt; color:CornflowerBlue;'><xsl:value-of select="item/value"/></span>
          </p>
        </td>
      </tr>
    </table>
    <br/>
    <br/>
    <table class="style2" cellpadding="4">
      <tr>
        <td colspan="4">
          <p class="MsoNormal" align="center" style='text-align:center'>
            <span style ='font-size:24.0pt;'>Valeur en Colis / couche : </span>          
            <span style='font-size:26.0pt'><xsl:value-of select="noLayersAndNoCases"/></span>
          </p>
        </td>
      </tr>
    </table>
    <br/>
    <br/>
    <table cellpadding="4">
      <tr>
        <td colspan="1">
          <xsl:apply-templates select="view_solution_front"/>
        </td>
        <td colspan="1">
          <xsl:apply-templates select="view_solution_left"/>
        </td>
        <td colspan="1">
          <xsl:apply-templates select="view_solution_right"/>
        </td>
        <td colspan="1">
          <xsl:apply-templates select="view_solution_back"/>
        </td>
      </tr>
      <tr>
        <td colspan="4" align="center">
          <xsl:apply-templates select="view_solution_iso"/>
        </td>
      </tr>
    </table>
  </xsl:template>

  <!--#### IMAGEGENERIC ####-->
  <xsl:template match="imageThumbSize">
    <img width="100" height="100" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_FRONT-->
  <xsl:template match="view_solution_front">
    <img width="150" height="150" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_LEFT-->
  <xsl:template match="view_solution_left">
    <img width="150" height="150" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_RIGHT-->
  <xsl:template match="view_solution_right">
    <img width="150" height="150" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_BACK-->
  <xsl:template match="view_solution_back">
    <img width="150" height="150" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_ISO-->
  <xsl:template match="view_solution_iso">
    <img width="400" height="400" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>

</xsl:stylesheet>
