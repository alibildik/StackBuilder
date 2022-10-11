<?xml version="1.0" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:param name="lang" />
  <!-- param set in command line -->
  <xsl:variable name="loc" select="document(concat( $lang, '.xml'), .)/strings" />
  <xsl:output method="html" indent="yes" />
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
          padding:0;
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
          font-size:10px;
          margin: 2%;
          width: 98%;
          padding: 0;
          }
          h1
          {
          color:black;
          font-size:20px;
          font-family:Arial;
          width:200mm;
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
          font-size:10px;
          font-family:Arial;
          padding: 0;
          }

          <!--
                /* Font Definitions */
                @font-face {
                    font-family: "Cambria Math";
                    panose-1: 2 4 5 3 5 4 6 3 2 4;
                }

                @font-face {
                    font-family: Calibri;
                    panose-1: 2 15 5 2 2 2 4 3 2 4;
                }
                /* Style Definitions */
                p.MsoNormal, li.MsoNormal, div.MsoNormal {
                    margin-top: 0cm;
                    margin-right: 0cm;
                    margin-bottom: 8.0pt;
                    margin-left: 0cm;
                    line-height: 107%;
                    font-size: 11.0pt;
                    font-family: "Calibri",sans-serif;
                    color: black;
                }

                .MsoPapDefault {
                    margin-bottom: 8.0pt;
                    line-height: 107%;
                }

                @page WordSection1 {
                    size: 842.25pt 595.5pt;
                    margin: 35.95pt 72.0pt 36.45pt 72.0pt;
                }

                div.WordSection1 {
                    page: WordSection1;
                }
                -->

        </style>

      </head>

      <body lang="FR" style='word-wrap: break-word'>

        <div class="WordSection1">
          <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 0cm'/>
          <table class="TableGrid" border="0" cellspacing="0" cellpadding="0" width="1126" style='width: 700pt; margin-left: 0pt; border-collapse: collapse'>
            <tr style='height: 391.55pt'>
              <td width="600" colspan="4" valign="top" style='width: 700pt; border: solid black 1.0pt; border-bottom: none; padding: 0cm 0cm 0cm 0cm; height: 600pt'>
                <p class="MsoNormal" style='margin-top: 0cm; margin-right: -.1pt; margin-bottom: 0cm; margin-left: 0cm'>
                  <xsl:apply-templates select="analysis" />
                </p>
              </td>
            </tr>
            <tr style='height: 33.35pt'>
              <td width="150" rowspan="5" valign="bottom" style='width: 200pt; border: solid black 1.0pt; border-top: none; padding: 0cm 0cm 0cm 0cm; height: 33.35pt'>
                <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 2.1pt'>
                  <span style='font-size: 8.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    This drawing belongs to DESJARDIN and cannot be transferred to anyone without DESJARDIN authorization.
                  </span>
                </p>
                <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 2.1pt'>
                  <span style='font-size: 8.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    Les poids sont donn&#233;s &#224; titre indicatif avec +/-5%.
                  </span>
                </p>
              </td>
              <td width="400" colspan="3" style='width: 300pt; border: solid black 1.0pt; border-left: none; padding: 0cm 0cm 0cm 0cm; height: 33.35pt'>
                <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 5.7pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                  <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    PLAN DE PALETTISATION
                  </span>
                </p>
              </td>
            </tr>
            <tr style='height: 23.8pt'>
              <td width="297" colspan="2" rowspan="3" valign="top" style='width: 222.0pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 7.05pt'>
                  <img width="250" height="65" src="images\logo_DESJARDIN.gif"/>
                </p>
              </td>
              <td width="200" style='width: 200.0pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                <p class="MsoNormal" style='margin-bottom: 0cm'>
                  <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    Drawned by: <xsl:apply-templates select="/report/author"/>
                  </span>
                </p>
              </td>
            </tr>
            <tr style='height: 23.8pt'>
              <td width="200" style='width: 200.0pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                <p class="MsoNormal" style='margin-bottom: 0cm'>
                  <span style='font-size: 9.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    Checked by:
                  </span>
                </p>
              </td>
            </tr>
            <tr style='height: 22.15pt'>
              <td width="200" style='width: 200.0pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 22.15pt'>
                <p class="MsoNormal" style='margin-bottom: 0cm'>
                  <span style='font-size: 9.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    Date: <xsl:apply-templates select="/report/dateOfCreation"/>
                  </span>
                </p>
              </td>
            </tr>
            <tr style='height: 28.4pt'>
              <td width="173" style='width: 130.1pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 3.2pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                  <span style='font-size: 9.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    PAPER FORMAT: A4
                  </span>
                </p>
              </td>
              <td width="174" style='width: 200.0pt; border: solid black 1.0pt; border-left: none; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                <p class="MsoNormal" style='margin-bottom: 0cm'>
                  <span style='font-size: 9.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    Ref.:
                  </span>
                </p>
              </td>
              <td width="174" valign="top" style='width: 200.0pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 3.15pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                  <span style='font-size: 18.0pt; line-height: 107%; font-family: "Times New Roman",serif'>
                    1/1
                  </span>
                </p>
              </td>
            </tr>
            <tr height="0">
              <td width="506" style='border: none'></td>
              <td width="173" style='border: none'></td>
              <td width="174" style='border: none'></td>
              <td width="174" style='border: none'></td>
            </tr>
          </table>
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="analysis">
    <xsl:apply-templates select="box"/>
    <xsl:apply-templates select="caseWithInnerDims"/>
    <xsl:apply-templates select="pallet"/>
    <xsl:apply-templates select="solution"/>
  </xsl:template>
  <xsl:template match="unitValue">
    <xsl:if test="valueM">
      <xsl:value-of select="valueM"/> (<xsl:value-of select="unitM"/>)
    </xsl:if>
    <xsl:if test="(valueM!='' and valueI!='')">
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
  <!--#### SOLUTION ####-->
  <xsl:template match="solution">
    <b class="style2">Solution</b>
    <table class="style1">
      <tr>
        <td rowspan="2" align="middle">
          <table>
            <tr>
              <td>
                <xsl:apply-templates select="view_solution_iso"/>
              </td>
            </tr>
          </table>
        </td>
        <td>
          <table>
            <tr>
              <td width="100px" colspan="1"/>
              <td align="middle" colspan="1">
                <xsl:apply-templates select="view_solution_front"/>
              </td>
              <td align="middle" colspan="1">
                <xsl:apply-templates select="view_solution_left"/>
              </td>
            </tr>
            <tr>
              <td width="100px" colspan="1"/>
              <td align="middle" colspan="1">
                <xsl:apply-templates select="view_solution_right"/>
              </td>
              <td align="middle" colspan="1">
                <xsl:apply-templates select="view_solution_back"/>
              </td>
            </tr>
          </table>
        </td>
      </tr>        
    </table>
    <table>
      <tr>
        <td class="style2" colspan="1">
          <b>Hauteur maximale de la palette (hors tout)</b>
        </td>
        <td class="style3" colspan="3">
          <xsl:apply-templates select="/report/analysis/constraintSet/maximumHeight"/> 
        </td>
      </tr>
      <xsl:apply-templates select="item"/>
      <xsl:if test="weightLoad">
        <tr>
          <td class="style2" colspan="1">
            <b>Masse totale des cartons</b>
          </td>
          <td class="style3" colspan="3">
            <xsl:apply-templates select="weightLoad/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="bboxLoad">
        <tr>
          <td class="style2" colspan="1">
            <b>Dimensions chargement (LxlxH)</b>
          </td>
          <td class="style3" colspan="3">
            <xsl:apply-templates select="bboxLoad/unitVector3"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weightTotal">
        <tr>
          <td class="style2" colspan="1">
            <b>Masse totale</b>
          </td>
          <td class="style3" colspan="3">
            <xsl:apply-templates select="weightTotal/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="bboxTotal">
        <tr>
          <td class="style2" colspan="1">
            <b>Dimensions hors tout (LxlxH)</b>
          </td>
          <td class="style3" colspan="3">
            <xsl:apply-templates select="bboxTotal/unitVector3"/>
          </td>
        </tr>
      </xsl:if>
    </table>
    <xsl:apply-templates select="layers"/>
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
    <img width="100" height="100" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### VIEW_SOLUTION_FRONT-->
  <xsl:template match="view_solution_front">
    <img width="125" height="125" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
    <p class="style3">Gauche</p>
  </xsl:template>
  <!--#### VIEW_SOLUTION_LEFT-->
  <xsl:template match="view_solution_left">
    <img width="125" height="125" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
    <p class="style3">Face</p>
  </xsl:template>
  <!--#### VIEW_SOLUTION_RIGHT-->
  <xsl:template match="view_solution_right">
    <img width="125" height="125" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
    <p class="style3">Arrière</p>
  </xsl:template>
  <!--#### VIEW_SOLUTION_BACK-->
  <xsl:template match="view_solution_back">
    <img width="125" height="125" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
    <p class="style3">Droite</p>
  </xsl:template>
  <!--#### VIEW_SOLUTION_ISO-->
  <xsl:template match="view_solution_iso">
    <img width="300" height="300" align="middle">
      <xsl:attribute name="src">
        <xsl:value-of select="imagePath"/>
      </xsl:attribute>
    </img>
  </xsl:template>
  <!--#### ITEM ####-->
  <xsl:template match="item">
    <tr>
      <td class="style2" colspan="1">
        <b>Nombre total de cartons</b>
      </td>
      <td class="style3" colspan="1">
        <xsl:value-of select="value"/>
      </td>
    </tr>
  </xsl:template>
  <!--#### LAYERS ####-->
  <xsl:template match="layers">
    <table width="100%">
      <tbody>
        <xsl:apply-templates select="layer[position() mod 2 = 1]" mode="row"/>
      </tbody>
    </table>
  </xsl:template>
  <xsl:template match="layer" mode="row">
    <tr>
      <!-- apply current and next layer -->
      <xsl:apply-templates select="self::layer | following-sibling::layer[1]" mode="cell" />
    </tr>
  </xsl:template>
  <!--#### LAYER ####-->
  <xsl:template match="layer" mode="cell">
    <td class="style2">
      <table width="100%">
        <tr>
          <td colspan="2" align="middle" width="400px">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
          <td class="style3" colspan="1"/>
        </tr>
        <tr>
          <td class="style2" colspan="1">
            <b>Couche(s):</b>
          </td>
          <td class="style3" colspan="1" width="200px" align="middle">
            <xsl:value-of select="layerIndexes"/>
          </td>
          <td class="style3" colspan="1" width="200px"/>
        </tr>
      </table>
    </td>
    <!-- re-apply in case this is the last layer -->
    <xsl:apply-templates select="self::layer" mode="last" />
  </xsl:template>

  <!-- the last, if and only if it is uneven -->
  <xsl:template match="layer[last()][position() mod 2 = 1]" mode="last">
    <td class="style2"></td>
  </xsl:template>

  <!-- ignore other layers that are not last -->
  <xsl:template match="node()" mode="last" />

  <!--#### CASE ####-->
  <xsl:template match="case">
    <b class="style2">Carton</b>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">Nom</td>
        <td class="style3" colspan="2" width="300">
          <xsl:value-of select="name"/>
        </td>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">Nombre de pi&#232;ces par carton</td>
        <td class="style3" colspan="1" width="300">
          <xsl:value-of select="description"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">Dimensions (LxlxH)</td>
        <td class="style3" colspan="1" width="300">
          <xsl:apply-templates select="dimensions/unitVector3"/>
        </td>
      </tr>
      <tr>
        <td class="style2">Poids d'un carton</td>
        <td class="style3" colspan="1" width="300">
          <xsl:apply-templates select="weight/unitValue"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### CASE WITH INNER DIMS #### -->
  <xsl:template match="caseWithInnerDims">
    <b class="style2">Carton</b>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1">Nom</td>
        <td class="style3" colspan="1" width="300">
          <xsl:value-of select="name"/>
        </td>
        <xsl:if test="imageThumbSize">
          <td rowspan="4" align="middle">
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <td class="style2" colspan="1">Nombre de pi&#232;ces par carton</td>
        <td class="style3" colspan="1" width="300">
          <xsl:value-of select="description"/>
        </td>
      </tr>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1">Dimensions (LxlxH)</td>
          <td class="style3" colspan="1" width="300">
            <xsl:apply-templates select="dimensions/unitVector3"/>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="weight">
          <td class="style2" colspan="1">Poids d'un carton</td>
          <td class="style3" colspan="1" width="300">
            <xsl:apply-templates select="weight/unitValue"/>
          </td>
        </xsl:if>
      </tr>
    </table>
  </xsl:template>
  <!--#### PALLET ####-->
  <xsl:template match="pallet">
    <b class="style2">Palette</b>
    <table class="style1" cellpadding="4">
      <tr>
        <td class="style2" colspan="1"><b>Nom</b></td>
        <td class="style3" colspan="1" width="300"><xsl:value-of select="name"/></td>
        <xsl:if test="imageThumbSize" >
          <td rowspan="4" colspan="1" align="middle" >
            <xsl:apply-templates select="imageThumbSize"/>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <td class="style2" colspan="1"><b>Description</b></td>
        <td class="style3" colspan="1" width="300"><xsl:value-of select="description"/></td>
      </tr>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1"><b>Dimensions  (LxlxH)</b></td>
          <td class="style3" colspan="1" width="300"><xsl:apply-templates select="dimensions/unitVector3"/></td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="weight">
          <td class="style2" colspan="1"><b>Poids</b></td>
          <td class="style3" colspan="1" width="300"><xsl:apply-templates select="weight/unitValue"/></td>
        </xsl:if>
      </tr>
    </table>
  </xsl:template>
  <!--#### BOX ####-->
  <xsl:template match="box">
    <b class="style2">Carton</b>
    <table class="style1">
      <tr>
        <td class="style2" colspan="1"><b>Nom</b></td>
        <td class="style3" colspan="1" width="300"><xsl:value-of select="name"/></td>
        <td rowspan="4" colspan="1" align="middle"><xsl:apply-templates select="imageThumbSize"/></td>
      </tr>
      <tr>
        <td class="style2" colspan="1"><b>Nombre de pi&#232;ces par carton</b></td>
        <td class="style3" colspan="1" width="300"><xsl:value-of select="description"/></td>
      </tr>
      <tr>
        <xsl:if test="dimensions">
          <td class="style2" colspan="1"><b>Dimensions (LxlxH)</b></td>
          <td class="style3" colspan="1" width="300"><xsl:apply-templates select="dimensions/unitVector3"/></td>
        </xsl:if>
      </tr>
      <xsl:if test="weight">
        <tr>
          <td class="style2" colspan="1"><b>Poids d'un carton</b></td>
          <td class="style3" colspan="1" width="300"><xsl:apply-templates select="weight/unitValue"/></td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### BAG ####-->
  <xsl:template match ="bag">
    <b class="style2">Sac</b>
    <table class="style1">
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
      <xsl:if test="netWeight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Net weight']"/>
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:apply-templates select="netWeight/unitValue"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### INTERLAYER ####-->
  <xsl:template match="interlayer">
    <b><xsl:value-of select="$loc/str[@name='Interlayer']"/></b>
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
      <tr>
        <xsl:if test="dimensions">
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="dimensions/unitVector2"/>
          </td>
        </xsl:if>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="thickness">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Thickness']"/>
            </b>
          </td>
          <td class="style3">
            <xsl:apply-templates select="thickness/unitValue"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unitM"></xsl:value-of>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="weight/valueM"></xsl:value-of>
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
      <tr>
        <td class="style2 " colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Length']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:apply-templates select="length/unitValue"/>
        </td>
        <td align="middle" colspan="2" rowspan="4">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Width']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:apply-templates select="width/unitValue"/>
        </td>
      </tr>
      <tr>
        <td class="style2"  colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Thickness']"/>
          </b>
        </td>
        <td class="style3"  colspan="1">
          <xsl:apply-templates select="thickness/unitValue"/>
        </td>
      </tr>
      <tr>
        <td class="style2"  colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/>
          </b>
        </td>
        <td class="style3"  colspan="1">
          <xsl:apply-templates select="weight/unitValue"/>)
        </td>
      </tr>
    </table>
  </xsl:template>
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
</xsl:stylesheet>
