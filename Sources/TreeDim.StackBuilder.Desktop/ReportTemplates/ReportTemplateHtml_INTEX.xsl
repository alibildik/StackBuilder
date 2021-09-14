<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
	xsi:noNamespaceSchemaLocation=".\ReportSchema.xsd">
  <xsl:param name="lang"/>
  <!-- param set in command line -->
  <xsl:variable name="loc" select="document(concat( $lang, '.xml'), .)/strings"/>
  <xsl:output method="html" indent="yes"/>
  <xsl:template match="report">
    <html>
      <head>
        <title>
          Rapport : <xsl:value-of select="name"/>
        </title>
        <style type="text/css">
          .style1
          {
          width:200mm;
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
      <body>
        <table class="style1" cellpadding="4">
          <xsl:if test="companyLogo">
            <tr>
              <td class="style2" colspan="1"/>
              <td class="style3" colspan="1"/>
              <td class="style2" colspan="1"/>
              <td colspan="1" align="middle">
                <xsl:apply-templates select="companyLogo"/>
              </td>
            </tr>
          </xsl:if>
          <tr>
            <td colspan="4">
              <h1 style="text-align:center">
                <b>
                  Produit <xsl:value-of select="name"/>
                </b>
              </h1>
            </td>
          </tr>
          <tr>
            <td class="style2" colspan="1">
              <xsl:value-of select="dateOfCreation"/>
            </td>
            <td class="style3" colspan="3">
            </td>
          </tr>
        </table>
        <xsl:apply-templates select="analysis"/>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="analysis">
    <h2>
      <xsl:value-of select="$loc/str[@name='Analysis']"/>: <xsl:value-of select="name"/>
    </h2>
    <table class="style1" cellpadding="3">
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:value-of select="description"/>
          </td>
        </tr>
      </xsl:if>
    </table>
    <xsl:apply-templates select="box"/>
    <xsl:apply-templates select="caseWithInnerDims"/>
    <xsl:apply-templates select="bundle"/>
    <xsl:apply-templates select="pack"/>
    <xsl:apply-templates select="cylinder"/>
    <xsl:apply-templates select="truck"/>
    <xsl:apply-templates select="interlayer"/>
    <xsl:apply-templates select="palletCorner"/>
    <xsl:apply-templates select="palletCap"/>
    <xsl:apply-templates select="palletFilm"/>
    <xsl:apply-templates select="solution"/>
  </xsl:template>

  <!-- Splitting templates : templates -->
  <xsl:template match="description">
    <xsl:call-template name="print-lines"/>
  </xsl:template>

  <xsl:template name="print-lines">
    <!-- If we are not passed text as a param, use the node's text. -->
    <xsl:param name="text" select="text()"/>

    <!-- If there is no (more) text, we are finished. -->
    <xsl:if test="string-length(normalize-space($text)) > 0">
      <xsl:choose>
        <!-- If the text contains a newline... -->
        <xsl:when test="contains($text, ';')">
          <!-- Split text into the first line and the remainder.  We search
                for the newline char using the '&#10;' entity instead of '\n'. -->
          <xsl:variable name="line" select="substring-before($text, ';')"/>
          <xsl:variable name="remainder" select="substring-after($text, ';')"/>

          <!-- Output the line, a HTML <br/> tag and a newline. -->
          <xsl:value-of select="normalize-space($line)"/>
          <xsl:element name="br"/>
          <xsl:text>&#10;</xsl:text>

          <!-- Recurse using the remaining text. -->
          <xsl:call-template name="print-lines">
            <xsl:with-param name="text" select="$remainder"/>
          </xsl:call-template>
        </xsl:when>
        <!-- Otherwise no more newlines, output the remaining text. -->
        <xsl:otherwise>
          <xsl:value-of select="normalize-space($text)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:template>
  <!-- Splitting templates : templates -->
  <!--#### CONSTRAINT SET ####-->
  <xsl:template match="constraintSet">
    <h3>
      <xsl:value-of select="$loc/str[@name='Constraint set']"/>
    </h3>
    <table class="style1">
      <xsl:if test="overhangX">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Overhang Length / Width']"/> (<xsl:value-of select="overhangX/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="overhangX/value"/> / <xsl:value-of select="overhangY/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="maximumHeight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Maximum pallet height']"/> (<xsl:value-of select="maximumHeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="maximumHeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="maximumWeight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Maximum weight']"/> (<xsl:value-of select="maximumWeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="maximumWeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="maximumNumber">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Maximum number']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="maximumNumber"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="allowedOrthoAxis">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Allowed ortho axes']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="allowedOrthoAxis"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### SOLUTION ####-->
  <xsl:template match="solution">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet']"/>
    </h3>
    <table class="style1">
      <xsl:apply-templates select="item"/>
      <xsl:if test="noLayersAndNoCases">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Layers x Cases']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="noLayersAndNoCases"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="netWeight">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Net weight']"/> (<xsl:value-of select="netWeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="netWeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weightLoad">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Load weight']"/> (<xsl:value-of select="weightLoad/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="weightLoad/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weightTotal">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weightTotal/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="weightTotal/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="efficiencyVolume">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Volume efficiency']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="efficiencyVolume"/>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <td colspan="4" align="middle">
          <xsl:apply-templates select="view_solution_iso"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--### ITEM ####-->
  <xsl:template match="item">
    <tr>
      <td class="style2" colspan="1">
        <b>
          <xsl:value-of select="name"/>
        </b>
      </td>
      <td class="style3" colspan="3">
        <xsl:value-of select="value"/>
      </td>
    </tr>
  </xsl:template>
  <!--#### COMPANYLOGO ####-->
  <xsl:template match="companyLogo" >
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
  <!--#### view_layer ####-->
  <xsl:template match ="view_layer">
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
  <!--#### IMAGEGENERIC ####-->
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
    <img align="middle">
      <xsl:attribute name="width">
        <xsl:value-of select="width"/>
      </xsl:attribute>
      <xsl:attribute name="height">
        <xsl:value-of select="height"/>
      </xsl:attribute>
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### LAYERS ####-->
  <xsl:template match="layers">
    <xsl:apply-templates select="layer"/>
  </xsl:template>
  <!--#### LAYER ####-->
  <xsl:template match="layer">
    <table class="style1" cellpadding="2">
      <tr>
        <td class="style3" colspan="1">
          <xsl:value-of select="layerIndexes"/>
        </td>
        <td>
          <xsl:apply-templates select="item"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--LAYER PACK SOLUTION-->
  <xsl:template match="layerPack">
    <tr>
      <td class="style3" colspan="1">
        <xsl:value-of select="layerPackCount"/>/<xsl:value-of select="layerCSUCount"/>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="layerWeight/value"/> / <xsl:value-of select="layerNetWeight/value"/>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="layerLength/value"/>*<xsl:value-of select="layerWidth/value"/>*<xsl:value-of select="layerHeight/value"/>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="maximumSpace/value"/>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="layerIndexes"/>
      </td>
      <td class="style3" colspan="4">
        <xsl:element name="img">
          <xsl:attribute name="src">
            <xsl:value-of select="imagePackLayer"/>
          </xsl:attribute>
        </xsl:element>
      </td>
    </tr>
  </xsl:template>
  <!--#### #### #### #### #### #### #### #### ####-->
  <!--#### CASE ####-->
  <xsl:template match="case">
    <h3>Case</h3>
    <table class="style1" cellpadding="3">
      <xsl:if test="name">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Name']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:value-of select="name"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:apply-templates select="description"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="dimensions">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Length']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="dimensions/unitVector3"/>
          </td>
          <td rowspan="5" align="middle">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unitValue"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="weight/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="admissibleLoad">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Admissible load on top']"/> (<xsl:value-of select="admissibleLoad/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="admissibleLoad/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### PACK ####-->
  <xsl:template match="pack">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pack']"/>
    </h3>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="name"/>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="dimensions">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Dimensions']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="dimensions/unitVector3"/>
        </td>
        <td rowspan="5" colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      </xsl:if>
      <xsl:if test="netweight">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Net weight']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="netWeight/unitValue"/>
        </td>
      </tr>
      </xsl:if>
      <xsl:if test="wrapperWeight">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Wrapper weight']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="wrapperWeight/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weight">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="weight/unitValue"/>
        </td>
      </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### CYLINDER ####-->
  <xsl:template match="cylinder">
    <h3>
      <xsl:value-of select="$loc/str[@name='Cylinder']"/>
    </h3>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="radius">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Radius']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="radius/unitValue"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="height">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Height']"/> (<xsl:value-of select="height/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="height/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="weight">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="weight/value"></xsl:value-of>
          </td>
        </xsl:if>
        <td colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"></xsl:apply-templates>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### CASE WITH INNER DIMS #### -->
  <xsl:template match="caseWithInnerDims">
    <h3>
      <xsl:value-of select="$loc/str[@name='Case']"/>
    </h3>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:apply-templates select="description"/>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="weight">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="weight/value"></xsl:value-of>
          </td>
        </xsl:if>
        <xsl:if test="imageThumbSize">
          <td colspan="2" align="middle">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </xsl:if>
      </tr>
    </table>
  </xsl:template>
  <!--#### LAYERS ####-->
  <xsl:template match="layers">
    <xsl:apply-templates select="layer"/>
  </xsl:template>
  <!--#### LAYER ####-->
  <xsl:template match="layer">
    <table class="style1" cellpadding="2">
      <tr>
        <td class="style3" colspan="1">
          <xsl:value-of select="$loc/str[@name='layer(s)']"/>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="layerIndexes"/>
        </td>
        <td>
          <xsl:apply-templates select="item"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### PALLET ####-->
  <xsl:template match="pallet">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet']"/>
    </h3>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
        <td rowspan="5" colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="weight">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="weight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="admissibleLoad">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Admissible load weight']"/> (<xsl:value-of select="admissibleLoad/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="admissibleLoad/value"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### BOX ####-->
  <xsl:template match="box">
    <h3>
      <xsl:value-of select="$loc/str[@name='Box']"/>
    </h3>
    <table class="style1">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="weight/unitValue"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### INTERLAYER ####-->
  <xsl:template match="interlayer">
    <h3>
      <xsl:value-of select="$loc/str[@name='Interlayer']"/>
    </h3>
    <table class="style1">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="weight/unitValue"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### PALLET CORNER ####-->
  <xsl:template match="palletCorner">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet corner']"/>
    </h3>
    <table class="style1">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      <xsl:if test="description">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="2">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
        <td align="middle" colspan="2" rowspan="4">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="width/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2"  colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Thickness']"/> (<xsl:value-of select="thickness/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3"  colspan="1">
          <xsl:value-of select="thickness/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2"  colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/>
          </b>
        </td>
        <td class="style3"  colspan="1">
          <xsl:value-of select="weight/unitValue"></xsl:value-of>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### PALLET CAP ####-->
  <!--#### PALLET CAP ####-->
  <xsl:template match="palletCap">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet cap']"/>
    </h3>
    <table class="style1" cellpadding="4">
      <xsl:if test="name">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Name']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="name"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="description">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="dimensions">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="dimensions"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <xsl:if test="innerDimensions">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Inner dimensions']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="innerDimensions/unitVector3"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="weight">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="weight/unitValue"/>
          </td>
        </xsl:if>
        <xsl:if test="imageThumbSize">
          <td colspan="2" align="middle">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </xsl:if>
      </tr>
    </table>
  </xsl:template>
  <!--#### PALLET FILM ####-->
  <xsl:template match ="palletFilm">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet film']"/>
    </h3>
    <table class="style1"  cellpadding="4">
      <xsl:if test="name">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Name']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="name"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="description">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="numberOfTurns">
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='NumberOfTurns']"/>
          </b>
        </td>
        <td class="style3" colspan="3">
          <xsl:value-of select="numberOfTurns"></xsl:value-of>
        </td>
        <tr>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### BUNDLE ####-->
  <xsl:template match="bundle">
    <h3>
      <xsl:value-of select="$loc/str[@name='Bundle']"/>
    </h3>
    <table class="style1">
      <xsl:if test="name">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Name']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="name"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="description">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Description']"/>
            </b>
          </td>
          <td class="style3" colspan="3">
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="dimensions">
        <tr>
          <td class="style2">
            <strong>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </strong>
          </td>
          <td class="style3">
            <xsl:apply-templates select="dimensions/unitVector2"/>
          </td>
          <td rowspan="6" align="middle">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="numberOfFlats">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Number of flats']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="numberOfFlats"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="unitThickness">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Unit thickness']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="unitThickness/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="unitWeight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Unit weight']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="unitWeight/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="totalThickness">
        <tr>
          <td class="style3">
            <b>
              <xsl:value-of select="$loc/str[@name='Total thickness']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="totalThickness/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weightTotal">
        <tr>
          <td class="style3">
            <b>
              <xsl:value-of select="$loc/str[@name='Total weight']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="weightTotal/unitValue"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### TRUCK ####-->
  <xsl:template match="truck">
    <h3>
      <xsl:value-of select="$loc/str[@name='Truck']"/>
    </h3>
    <table class="style1" cellpadding="3">
      <xsl:if test="name">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
      </tr>
      </xsl:if>
      <xsl:if test="description">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Description']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="description"></xsl:value-of>
        </td>
      </tr>
      </xsl:if>
      <xsl:if test="dimensions">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Dimensions']"/>
          </b>
        </td>
        <td class="style3">
          <xsl:apply-templates select="dimensions/unitVector3"/>
        </td>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      </xsl:if>
      <xsl:if test="admissibleLoad">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Admissible load']"/>
          </b>
        </td>
        <td class="style3">
          <xsl:apply-templates select="admissibleLoad/unitValue"/>
        </td>
      </tr>
      </xsl:if>
    </table>
  </xsl:template>

  <xsl:template match="unitValue">
    <xsl:if test="valueM">
      <xsl:value-of select="valueM"/> (<xsl:value-of select="unitM"/>)
    </xsl:if>
    <xsl:if test="(v0M!='' and v0I!='')">
      <br/>
    </xsl:if>
    <xsl:if test="valueI">
      <xsl:value-of select="valueI"/> (<xsl:value-of select="unitI"/>)
    </xsl:if>
  </xsl:template>
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
</xsl:stylesheet>