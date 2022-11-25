<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="lang"/>
  <!-- param set in command line -->
  <xsl:variable name="loc" select="document(concat( $lang, '.xml'), .)/strings"/>
  <xsl:output method="html" indent="yes"/>
  <xsl:template match="report">
    <html>
      <head>
        <title>
          <xsl:value-of select="name"></xsl:value-of>
          <xsl:value-of select="$loc/str[@name='report']"/>
        </title>
        <style type="text/css">
          .style1
          {
          color:blue;
          }
          .style2
          {
          width: 50mm;
          color:black;
          font-family:Arial;
          font-size:10px;
          }
          .style3
          {
          color:black;
          font-family:Arial;
          font-size:10px;
          }
          body
          {
          font-family:Arial;
          font-size:11px;
          margin: 5%;
          width: 90%;
          padding: 0;
          }
          h1
          {
          color:black;
          font-size:20px;
          font-family:Arial;
          width:200mm
          }
          h2
          {
          color:red;
          font-size:16px;
          font-family:Arial;
          }
          h3
          {
          color:blue;
          font-size:12px;
          font-family:Arial;
          }
          table
          {
          border:solid grey 1px;
          width:200mm;
          border-spacing: 0px;
          cell-spacing: 0px;
          }
          td
          {
          padding: 0px;
          }
        </style>
      </head>
      <!-- HEADER -->
      <body>
        <p>
          <img width="566" height="74" src="images\image001.jpg"/>
        </p>
        <table class="style3" cellpadding="2">
          <tr>
            <td>Customer</td>
            <td>
              <xsl:value-of select="customer"/>
            </td>
          </tr>
          <tr>
            <td>Description/SKU</td>
            <td>
              <xsl:value-of select="description"/>
            </td>
          </tr>
          <tr>
            <td>Date</td>
            <td>
              <xsl:value-of select="dateOfCreation"/>
            </td>
          </tr>
        </table>
        <xsl:apply-templates select="analysis"/>
        <xsl:apply-templates select="analysisPalletsOnPallet"/>
        <table class="style3" colspan="1">
          <tr>
            <td>
              <xsl:text>MENASHA</xsl:text>
              <br/>
              <xsl:text>35 PRECIDIO COURT</xsl:text>
              <br/>
              <xsl:text>BRAMPTON, ON L6S 6B6</xsl:text>
              <br/>
              <xsl:text>www.menasha.com</xsl:text>
              <br/>
              <xsl:text>T: 905.792.7092 F: 905.790.9431</xsl:text>
            </td>
          </tr>
        </table>
        <table class="style3" colspan="9">
          <tr>
            <td>
              Pallet can be double stacked
              (Leave blank if testing is required)
            </td>
            <td>
              <img width="36" height="36" src="images\image003.gif" align="left" hspace="12"/>
            </td>
            <td>Warehouse</td>
            <td>
              <img width="36" height="36" src="images\image003.gif" align="left" hspace="12"/>
            </td>
            <td>Truck</td>
            <td>
              <img width="36" height="36" src="images\image003.gif" align="left" hspace="12"/>
            </td>
            <td>Do not stack</td>
            <td>
              <img width="36" height="36" src="images\image003.gif" align="left" hspace="12"/>
            </td>
            <td>TBD</td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
  <!--#### ANALYSIS ####-->
  <xsl:template match="analysis">
    <table class="style3" colspan="5">
      <xsl:apply-templates select="box"/>
      <xsl:apply-templates select="caseWithInnerDims"/>
      <xsl:apply-templates select="pallet"/>
      <xsl:apply-templates select="solution"/>
    </table>
  </xsl:template>
  <xsl:template match="analysisPalletsOnPallet">
    <table class="style3" colspan="5">
      <xsl:apply-templates select="pallet"/>
      <xsl:apply-templates select="solutionPalletsOnPallet"/>
    </table>
  </xsl:template>
  <!--#### BOX ####-->
  <xsl:template match="box">
    <tr>
      <td class="style2" colspan="1">
        <b>Shipper OD</b>
      </td>
      <td class="style3" colspan="1">
        <xsl:apply-templates select="dimensions/unitVector3"/>
      </td>
      <td class="style2" colspan="1">
        <b>Weight</b>
      </td>
      <td>
        <xsl:apply-templates select="weight/unitValue"/>
      </td>
      <td colspan="1" align="middle">
        <xsl:apply-templates select="imageThumbSize"/>
      </td>
    </tr>
  </xsl:template>
  <xsl:template match="caseWithInnerDims">
    <tr>
      <td class="style2" colspan="1">
        <b>Shipper OD</b>
      </td>
      <td class="style3" colspan="1">
        <xsl:apply-templates select="dimensions/unitVector3"/>
      </td>
      <td class="style2" colspan="1">
        <b>Weight</b>
      </td>
      <td>
        <xsl:apply-templates select="weight/unitValue"/>
      </td>
      <td colspan="1" align="middle">
        <xsl:apply-templates select="imageThumbSize"/>
      </td>
    </tr>    
  </xsl:template>
  <!-- pallet -->
  <xsl:template match="pallet">
    <tr>
      <td class="style2" colspan="1">
        <b>Product OD</b>
      </td>
      <td class="style3" colspan="1">
        <xsl:apply-templates select="dimensions/unitVector3"/>
      </td>
      <td class="style2" colspan="1">
        <b>Weight</b>
      </td>
      <td>
        <xsl:apply-templates select="weight/unitValue"/>
      </td>
      <td colspan="1" align="middle">
        <xsl:apply-templates select="imageThumbSize"/>
      </td>
    </tr>
  </xsl:template>
  <!-- solution -->
  <xsl:template match="solution">
    <xsl:apply-templates select="item"/>
    <xsl:if test="noLayersAndNoCases">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Cases x Layers']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="noLayersAndNoCases"/>
        </td>
      </tr>
    </xsl:if>
    <tr>      
    </tr>
    <tr>
      <td>
        <b>Load</b>
      </td>
      <td>
        <xsl:apply-templates select="bboxLoad/unitVector3"/>
      </td>
      <td>
        <b>Weight</b>
      </td>
      <td>
        <xsl:apply-templates select="weightLoad/unitValue"/>
      </td>
    </tr>
    <tr>
      <td>
        <b>Pallet</b>
      </td>
      <td>
        <xsl:apply-templates select="bboxTotal/unitVector3"/>
      </td>
      <td>
        <b>Weight</b>
      </td>
      <td>
        <xsl:apply-templates select="weightTotal/unitValue"/>
      </td>
    </tr>
    <tr>
      <td colspan="5" align="middle">
        <xsl:apply-templates select="view_solution_iso"/>
      </td>
    </tr>
  </xsl:template>

  <!--#### solutionPalletsOnPallet ####-->
  <xsl:template match="solutionPalletsOnPallet">
    <tr>
      <td>
        <xsl:apply-templates select="item"/>
      </td>
    </tr>
    <tr>
      <td>
        <b>Load</b>
      </td>
      <td>
        <xsl:apply-templates select="bboxLoad/unitVector3"/>
      </td>
      <td>
        <b>
          Weight
        </b>
      </td>
      <td>
        <xsl:apply-templates select="weightLoad/unitValue"/>
      </td>
    </tr>
    <tr>
      <td>
        <b>
          Overall
        </b>
      </td>
      <td>
        <xsl:apply-templates select="bboxTotal/unitVector3"/>
      </td>
      <td>
        <b>
          Weight
        </b>
      </td>
      <td>
        <xsl:apply-templates select="weightTotal/unitValue"/>
      </td>
    </tr>
    <tr>
      <td colspan="4" align="middle">
        <xsl:apply-templates select="view_solution_iso"/>
      </td>
    </tr>
  </xsl:template>
  <!--#### ITEM ####-->
  <xsl:template match="item">
    <tr>
      <td class="style2" colspan="1">
        <b>
          <xsl:value-of select="name"/>
        </b>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="value"/>
      </td>
    </tr>
  </xsl:template>
  <!--#### VIEW SOLUTION ISO ####-->
  <xsl:template match="view_solution_iso">
    <img align="middle">
      <xsl:attribute name="width">
        <xsl:value-of select="200"/>
      </xsl:attribute>
      <xsl:attribute name="height">
        <xsl:value-of select="200"/>
      </xsl:attribute>
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### UNITVECTOR3 ####-->
  <xsl:template match ="unitVector2">
    <xsl:if test="v0M">
      <xsl:value-of select="v0M"/> x <xsl:value-of select="v1M"/> (<xsl:value-of select="unitM"/>)
    </xsl:if>
    <xsl:if test="(v0M!='' and v0I!='')">
      <br/>
    </xsl:if>
    <xsl:if test="v0I">
      <xsl:value-of select="v0I"/> x <xsl:value-of select="v1I"/> (<xsl:value-of select="unitI"/>)
    </xsl:if>
  </xsl:template>
  <xsl:template match ="unitVector3">
    <xsl:if test="v0M">
      <xsl:value-of select="v0M"/> x <xsl:value-of select="v1M"/> x <xsl:value-of select="v2M"/>  (<xsl:value-of select="unitM"/>)
    </xsl:if>
    <xsl:if test="(v0M!='' and v0I!='')">
      <br/>
    </xsl:if>
    <xsl:if test="v0I">
      <xsl:value-of select="v0I"/> x <xsl:value-of select="v1I"/> x <xsl:value-of select="v2I"/>  (<xsl:value-of select="unitI"/>)
    </xsl:if>
  </xsl:template>
  <xsl:template match="unitValue">
    <xsl:if test="valueM">
      <xsl:value-of select="valueM"/> (<xsl:value-of select="unitM"/>)
    </xsl:if>
    <xsl:if test="((normalize-space(valueM)!='') and (normalize-space(valueI)!=''))">
      <br/>
    </xsl:if>
    <xsl:if test="valueI">
      <xsl:value-of select="valueI"/> (<xsl:value-of select="unitI"/>)
    </xsl:if>
  </xsl:template>
  <!--#### IMAGETHUMBSIZE ####-->
  <xsl:template match="imageThumbSize">
    <img align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
      <xsl:attribute name="width">
        <xsl:value-of select="width"/>
      </xsl:attribute>
      <xsl:attribute name="height">
        <xsl:value-of select="height"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  
</xsl:stylesheet>
