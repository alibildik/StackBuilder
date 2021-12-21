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
        <table class="style1" cellpadding="4">
          <tr>
            <td class="style2" colspan="1">
              <b>
                <xsl:value-of select="$loc/str[@name='Document']"/>
              </b>
            </td>
            <td class="style3" colspan="2">
              <xsl:value-of select="name"/>
            </td>
            <xsl:if test="companyLogo">
              <td colspan="1" align="middle">
                <xsl:apply-templates select="companyLogo"/>
              </td>
            </xsl:if>
          </tr>
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
              <td class="style2" colspan="1"/>
            </tr>
          </xsl:if>
          <tr>
            <xsl:if test="dateOfCreation">
              <td class="style2" colspan="1">
                <b>
                  <xsl:value-of select="$loc/str[@name='Date']"/>
                </b>
              </td>
              <td class="style3" colspan="1">
                <xsl:value-of select="dateOfCreation"/>
              </td>
            </xsl:if>
            <xsl:if test="author">
              <td class="style2" colspan="1">
                <b>
                  <xsl:value-of select="$loc/str[@name='Author']"/>
                </b>
              </td>
              <td class="style3" colspan="1">
                <xsl:value-of select="author"></xsl:value-of>
              </td>
            </xsl:if>
          </tr>
        </table>
        <xsl:apply-templates select="analysis"/>
        <xsl:apply-templates select="hAnalysis"/>
        <xsl:apply-templates select="packStress"/>
      </body>
    </html>
  </xsl:template>
  <!--#### ANALYSIS ####-->
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
    <xsl:apply-templates select="pallet"/>
    <xsl:apply-templates select="truck"/>
    <xsl:apply-templates select="interlayer"/>
    <xsl:apply-templates select="palletCorner"/>
    <xsl:apply-templates select="palletCap"/>
    <xsl:apply-templates select="palletFilm"/>
    <xsl:apply-templates select="constraintSet"/>
    <xsl:apply-templates select="solution"/>

  </xsl:template>
  <!--#### HANALYSIS ####-->
  <xsl:template match="hAnalysis">
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
    <xsl:apply-templates select="pallet"/>
    <xsl:apply-templates select="hConstraintSet"/>
    <xsl:apply-templates select="hSolution"/>
  </xsl:template>
  <!--#### PACKSTRESS ####-->
  <xsl:template match ="packStress">
    <h2>
      <xsl:value-of select ="$loc/str[@name='PackStress']"/>
    </h2>
    <xsl:apply-templates select="bctCase"/>
    <xsl:apply-templates select="material"/>
    <xsl:apply-templates select="staticBCT"/>
    <xsl:apply-templates select="dynamicBCT"/>
    <xsl:apply-templates select="palletisation"/>
  </xsl:template>
  <!--#### BCTCASE ####-->
  <xsl:template match="bctCase">
    <h3>
      <xsl:value-of select="$loc/str[@name='Case']"/>
    </h3>
    <table class="style1" cellpadding="3">
      <tr>
        <xsl:if test="length">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/> (<xsl:value-of select="length/unit"/>*<xsl:value-of select="width/unit"/>*<xsl:value-of select="height/unit"/>)
            </b>
          </td>
        </xsl:if>
        <xsl:if test="weight">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"/>)
            </b>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="length">
          <td class="style3" colspan="1">
            <xsl:value-of select="length/value"/>*<xsl:value-of select="width/value"/>*<xsl:value-of select="height/value"/>
          </td>
        </xsl:if>
        <xsl:if test="weight">
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
  <!---->
  <!--#### MATERIAL ####-->
  <xsl:template match="material">
    <h3>
      <xsl:value-of select="$loc/str[@name='Material']"/>
    </h3>
    <table class="style1" cellpadding="2">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="name"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Profile']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="profile"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Thickness']"/> (<xsl:value-of select="thickness/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="thickness/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='ECT']"/> (<xsl:value-of select="ect/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="ect/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Stiffness X']"/> (<xsl:value-of select="stiffnessX/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="stiffnessX/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Stiffness Y']"/> (<xsl:value-of select="stiffnessY/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="stiffnessY/value"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### static BCT ####-->
  <xsl:template match="staticBCT">
    <h3>
      <xsl:value-of select="$loc/str[@name='Static BCT']"/>
    </h3>
    <table class="style1" cellpadding="2">
      <tr>
        <td class="style2" cellspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Static BCT']"/> (<xsl:value-of select="bct/unit"/>)
          </b>
        </td>
        <td class="style3" cellspan="1">
          <xsl:value-of select="bct/value"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### dynamic BCT ####-->
  <xsl:template match="dynamicBCT">
    <h3>
      <xsl:value-of select="$loc/str[@name='Dynamic BCT']"/>
    </h3>
    <table class="style1" cellpadding="7">
      <tr>
        <td class="style2" cellspan="1">
          <b>Humidity/Storage</b>
        </td>
        <td class="style2" cellspan="1">
          <b>0-45%</b>
        </td>
        <td class="style2" cellspan="1">
          <b>46-55%</b>
        </td>
        <td class="style2" cellspan="1">
          <b>56-65%</b>
        </td>
        <td class="style2" cellspan="1">
          <b>66-75%</b>
        </td>
        <td class="style2" cellspan="1">
          <b>76-85%</b>
        </td>
        <td class="style2" cellspan="1">
          <b>86-100%</b>
        </td>
      </tr>
      <xsl:for-each select="bctRow">
        <tr>
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="name"/>
            </b>
          </td>
          <xsl:for-each select ="bctValue">
            <td class="style3" cellspan="1">
              <xsl:value-of select="."/>
            </td>            
          </xsl:for-each>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>
  <!--#### palletisation ####-->
  <xsl:template match="palletisation">
    <h3>
      <xsl:value-of select="$loc/str[@name='Palletisation']"/>
    </h3>
    <xsl:apply-templates select="bctPallet"/>
    <xsl:apply-templates select="bctSolution"/>
  </xsl:template>
  <xsl:template match="bctPallet">
    <table class="style1" cellpadding="4">
      <tr>
        <xsl:if test="length">
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/> (<xsl:value-of select="length/unit"/>x<xsl:value-of select="width/unit"/>x<xsl:value-of select="height/unit"/> )
            </b>
          </td>
        </xsl:if>
        <xsl:if test="weight">
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"/>)
            </b>
          </td>
        </xsl:if>
        <xsl:if test="overhangX">
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Overhang']"/>(<xsl:value-of select="overhangX/unit"/>*<xsl:value-of select="overhangY/unit"/>)
            </b>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="length">
          <td class="style3" cellspan="1">
            <xsl:value-of select="length/value"/>x<xsl:value-of select="width/value"/>x<xsl:value-of select="height/value"/>
          </td>
        </xsl:if>
        <xsl:if test="weight">
          <td class="style3" cellspan="1">
            <xsl:value-of select="weight/value"/>
          </td>
        </xsl:if>
        <xsl:if test="overhangX">
          <td class="style3" cellspan="1">
            <xsl:value-of select="overhangX/value"/>*<xsl:value-of select="overhangY/value"/>
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
  <xsl:template match="bctSolution">
    <table class="style1" cellpadding="3">
      <xsl:if test="noLayersAndNoCases">
        <tr>
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Layers x Cases']"/>
            </b>
          </td>
          <td class="style3" cellspan="1">
            <xsl:value-of select="noLayersAndNoCases"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <xsl:if test="weightLoad">
        <tr>
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Load weight']"/> (<xsl:value-of select="weightLoad/unit"/>)
            </b>
          </td>
          <td class="style3" cellspan="1">
            <xsl:value-of select="weightLoad/value"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <xsl:if test="weightTotal">
        <tr>
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Total weight']"/> (<xsl:value-of select="weightTotal/unit"/>)
            </b>
          </td>
          <td class="style3" cellspan="1">
            <xsl:value-of select="weightTotal/value"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <xsl:if test="loadOnLowestCase">
        <tr>
          <td class="style2" cellspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Load on lowest case']"/> (<xsl:value-of select="loadOnLowestCase/unit"/>)
            </b>
          </td>
          <td class="style3" cellspan="1">
            <xsl:value-of select="loadOnLowestCase/value"/>
          </td>
          <td/>
        </tr>
      </xsl:if>
      <xsl:if test="view_solution_iso">
        <tr>
          <td/>
          <td/>
          <td colspan="2" align="middle">
            <xsl:apply-templates select="view_solution_iso"/>
          </td>
        </tr>
      </xsl:if>
    </table>
  </xsl:template>
  <!--#### ECTANALYSIS ####-->
  <xsl:template match="ectAnalysis">
    <h2>
      <xsl:value-of select="$loc/str[@name='Box Compression Test analysis']"/>
    </h2>
    <xsl:apply-templates select="cardboard"></xsl:apply-templates>
    <table class="style1" cellpadding="2">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Case type']"/>
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Printed surface']"/>
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Mc Kee Formula']"/>
          </b>
        </td>
      </tr>
      <tr>
        <td class="style3">
          <xsl:value-of select="caseType"></xsl:value-of>
        </td>
        <td class="style3">
          <xsl:value-of select="printedSurface"></xsl:value-of>
        </td>
        <td class="style3">
          <xsl:value-of select="mcKeeFormulaMode"></xsl:value-of>
        </td>
      </tr>
    </table>
    <xsl:apply-templates select="bct_static"></xsl:apply-templates>
    <xsl:apply-templates select="bct_dynamic"></xsl:apply-templates>
  </xsl:template>
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
      <xsl:value-of select="$loc/str[@name='Solution']"/>
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
		  <xsl:if test="noInterlayers">
			  <tr>
				  <td class="style2" colspan="1">
					  <b>
						  <xsl:value-of select="$loc/str[@name='Number of interlayers']"/>
					  </b>
				  </td>
				  <td class="style3" colspan="3">
					  <xsl:value-of select="noInterlayers"/>
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
		  <xsl:if test="bboxLoad">
			  <tr>
				  <td class="style2" colspan="1">
					  <b>
						  <xsl:value-of select="$loc/str[@name='Load dimensions']"/> (<xsl:value-of select="bboxLoad/unit"/>)
					  </b>
				  </td>
				  <td class="style3" colspan="3">
					  <xsl:value-of select="bboxLoad/v0"/> x <xsl:value-of select="bboxLoad/v1"/> x <xsl:value-of select="bboxLoad/v2"/>
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
		  <xsl:if test="bboxTotal">
			  <tr>
				  <td class="style2" colspan="1">
					  <b>
						  <xsl:value-of select="$loc/str[@name='Overall dimensions']"/> (<xsl:value-of select="bboxTotal/unit"/>)
					  </b>
				  </td>
				  <td class="style3" colspan="3">
					  <xsl:value-of select="bboxTotal/v0"/> x <xsl:value-of select="bboxTotal/v1"/> x <xsl:value-of select="bboxTotal/v2"/>
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
	  </table>
	  <table class="style1">
		  <tr>
			  <td rowspan="2" align="middle">
				  <xsl:apply-templates select="view_solution_iso"/>
			  </td>
			  <td align="middle" colspan="1">
				  <xsl:apply-templates select="view_solution_front"/>
			  </td>
			  <td align="middle" colspan="1">
				  <xsl:apply-templates select="view_solution_left"/>
			  </td>
		  </tr>
		  <tr>
			  <td/>
			  <td align="middle" colspan="1">
				  <xsl:apply-templates select="view_solution_right"/>
			  </td>
			  <td align="middle" colspan="1">
				  <xsl:apply-templates select="view_solution_back"/>
			  </td>
		  </tr>
	  </table>
    <xsl:apply-templates select="layers"/>
  </xsl:template>
  <!--### HCONSTRAINTSET ###-->
  <xsl:template match="hConstraintSet">
    <h3>
      <xsl:value-of select="$loc/str[@name='Constraint set']"/>
    </h3>
    <table class="style1">
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
    </table>
  </xsl:template>
  <!--#### HSOLUTION ####-->
  <xsl:template match="hSolution">
    <h3>
      <xsl:value-of select="$loc/str[@name='Solution']"/>
    </h3>
    <xsl:apply-templates select="solItem"/>
  </xsl:template>
  <!--#### SOLITEM ####-->
  <xsl:template match="solItem">
    <h4>
      <xsl:value-of select="$loc/str[@name='Part']"/> : <xsl:value-of select="index"/>
    </h4>
    <xsl:apply-templates select="itemQuantities"/>
    <table class="style1">
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
      <tr>
        <td align="middle" colspan="1">
          <xsl:apply-templates select="view_solution_front"/>
        </td>
        <td align="middle" colspan="1">
          <xsl:apply-templates select="view_solution_left"/>
        </td>
        <td align="middle" colspan="1">
          <xsl:apply-templates select="view_solution_right"/>
        </td>
        <td align="middle" colspan="1">
          <xsl:apply-templates select="view_solution_back"/>
        </td>
      </tr>
      <tr>
        <td colspan="4" align="middle">
          <xsl:apply-templates select="view_solution_iso"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### ITEMQUANTITIES-->
  <xsl:template match="itemQuantities">
    <h4>
      <xsl:value-of select="$loc/str[@name='Items']"/>
    </h4>
    <table class="style1">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Count']"/>
          </b>
        </td>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="unitWeight"/>)
          </b>
        </td>
      </tr>
      <xsl:apply-templates select="itemQuantity"/>
      <tr/>
    </table>
  </xsl:template>
  <!--#### ITEMQUANTITY-->
  <xsl:template match="itemQuantity">
    <tr>
      <td class="style2" colspan="1">
        <b>
          <xsl:value-of select="name"/>
        </b>
      </td>
      <td class="style2" colspan="1">
        <xsl:value-of select="count"/>
      </td>
      <td class="style2" colspan="1">
        <xsl:value-of select="weight"/>
      </td>
      <xsl:if test="imageThumbSize">
        <td class="style2" colspan="1">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </xsl:if>
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
    <h3>
      <xsl:value-of select="$loc/str[@name='Layer(s)']"/>
    </h3>
    <xsl:apply-templates select="layer"/>
  </xsl:template>
  <!--#### LAYER ####-->
  <xsl:template match="layer">
    <table class="style1" cellpadding="3">
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Layer Indexes']"/>
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="layerIndexes"/>
        </td>
        <td rowspan="5" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:apply-templates select="item"/>
      <xsl:if test="layerLength">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Dimensions']"/> (<xsl:value-of select="layerLength/unit"/> x <xsl:value-of select="layerWidth/unit"/> x <xsl:value-of select="layerHeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="layerLength/value"/> x <xsl:value-of select="layerWidth/value"/> x <xsl:value-of select="layerHeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="layerWeight">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="layerWeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="layerWeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="netWeight">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Net weight']"/> (<xsl:value-of select="layerNetWeight/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="layerNetWeight/value"/>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="layerSpaces">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Spaces']"/> (<xsl:value-of select="layerSpaces/unit"/>)
            </b>
          </td>
          <td class="style3" colspace="1">
            <xsl:value-of select="layerSpaces/value"/>
          </td>
        </tr>
      </xsl:if>
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
  <!--CARDBOARD-->
  <xsl:template match="cardboard">
    <h3>
      <xsl:value-of select="$loc/str[@name='Carton']"/>
    </h3>
    <b>
      <xsl:value-of select="$loc/str[@name='Cardboard']"/>
    </b>
    <table border="1" cellpadding="5">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Name']"/>
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Thickness']"/> (<xsl:value-of select="thickness/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='ECT']"/> (<xsl:value-of select="ect/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='StiffnessX']"/> (<xsl:value-of select="stiffnessX/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='StiffnessY']"/> (<xsl:value-of select="stiffnessY/unit"></xsl:value-of>)
          </b>
        </td>
      </tr>
      <tr>
        <td class="style3">
          <xsl:value-of select="name"></xsl:value-of>
        </td>
        <td>
          <xsl:value-of select="thickness/value"></xsl:value-of>
        </td>
        <td>
          <xsl:value-of select="ect/value"></xsl:value-of>
        </td>
        <td>
          <xsl:value-of select="stiffnessX/value"></xsl:value-of>
        </td>
        <td>
          <xsl:value-of select="stiffnessY/value"></xsl:value-of>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--BCT_STATIC-->
  <xsl:template match="bct_static">
    <table border="1" cellpadding="2">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Static BCP']"/>
          </b>
        </td>
        <td class="style3">
          <xsl:text></xsl:text>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--BCT_DYNAMIC-->
  <xsl:template match="bct_dynamic">
    <b>
      <xsl:value-of select="$loc/str[@name='Dynamic BCP']"/>
    </b>
    <table border="1" cellpadding="7">
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Storage']"/>
          </b>
        </td>
        <td class="style2">
          <b>0-45 %</b>
        </td>
        <td class="style2">
          <b>46-55 %</b>
        </td>
        <td class="style2">
          <b>56-65 %</b>
        </td>
        <td class="style2">
          <b>66-75 %</b>
        </td>
        <td class="style2">
          <b>76-85 %</b>
        </td>
        <td class="style2">
          <b>86-100 %</b>
        </td>
      </tr>
      <xsl:apply-templates select="bct_dynamic_row"></xsl:apply-templates>
    </table>
  </xsl:template>
  <!--BCT_DYNAMIC_STORAGE-->
  <xsl:template match="bctDynRow">
    <tr>
      <td class="style2">
        <b>
          <xsl:value-of select="bctDynRowName"/>
        </b>
      </td>
      <xsl:apply-templates select="bctDynRowValue"/>
    </tr>
  </xsl:template>
  <xsl:template match="bctDynamicRowValue">
    <td class="style2">
      <xsl:value-of select="bct_value"/>
    </td>
  </xsl:template>
  <!--#### #### #### #### #### #### #### #### ####-->
  <!--#### CASE ####-->
  <xsl:template match="case">
    <h3>Case</h3>
    <table class="style1" cellpadding="3">
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
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="length/value"></xsl:value-of>
        </td>
        <td rowspan="5" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="width/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Height']"/> (<xsl:value-of select="height/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="height/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="weight/value"></xsl:value-of>
        </td>
      </tr>
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
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Dimensions']"/> (<xsl:value-of select="length/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="length/value"/> * <xsl:value-of select="width/value"/> * <xsl:value-of select="height/value"/>
        </td>
        <td rowspan="5" colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Net weight']"/> (<xsl:value-of select="netWeight/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="netWeight/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Wrapper weight']"/> (<xsl:value-of select="wrapperWeight/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="wrapperWeight/value"/>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Tray weight']"/> (<xsl:value-of select="trayWeight/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="trayWeight/value"/>
        </td>
      </tr>
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
      <xsl:if test="diameter">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Diameter']"/> (<xsl:value-of select="diameter/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="diameter/value"></xsl:value-of>
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
            <xsl:value-of select="description"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <tr>
        <xsl:if test="length">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Ext. length']"/> (<xsl:value-of select="length/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="length/value"/>
          </td>
        </xsl:if>
        <xsl:if test="innerLength">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Interior length']"/> (<xsl:value-of select="innerLength/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="innerLength/value"></xsl:value-of>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="width">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Ext. width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="width/value"></xsl:value-of>
          </td>
        </xsl:if>
        <xsl:if test="innerWidth">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Interior width']"/> (<xsl:value-of select="innerWidth/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="innerWidth/value"></xsl:value-of>
          </td>
        </xsl:if>
      </tr>
      <tr>
        <xsl:if test="height">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Ext. height']"/> (<xsl:value-of select="height/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="height/value"></xsl:value-of>
          </td>
        </xsl:if>
        <xsl:if test="innerHeight">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Interior height']"/> (<xsl:value-of select="innerHeight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="innerHeight/value"></xsl:value-of>
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
        <xsl:if test="length">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="length/value"></xsl:value-of>
          </td>
        </xsl:if>
        <td rowspan="5" colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="width">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="width/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="height">
        <tr>
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Height']"/> (<xsl:value-of select="height/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="height/value"/>
          </td>
        </tr>
      </xsl:if>
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
        <xsl:if test="length">
          <td class="style2" colspan="1">
            <b>
              <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="length/value"></xsl:value-of>
          </td>
        </xsl:if>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="width">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="width/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="height">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Height']"/> (<xsl:value-of select="height/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="height/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3" colspan="1">
            <xsl:value-of select="weight/value"></xsl:value-of>
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
        <xsl:if test="length">
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="length/value"></xsl:value-of>
          </td>
        </xsl:if>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <xsl:if test="width">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="width/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="thickness">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Thickness']"/> (<xsl:value-of select="thickness/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="thickness/value"></xsl:value-of>
          </td>
        </tr>
      </xsl:if>
      <xsl:if test="weight">
        <tr>
          <td class="style2">
            <b>
              <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
            </b>
          </td>
          <td class="style3">
            <xsl:value-of select="weight/value"></xsl:value-of>
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
        <td class="style2 " colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="length/value"></xsl:value-of>
        </td>
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
            <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3"  colspan="1">
          <xsl:value-of select="weight/value"></xsl:value-of>
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
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Ext. length']"/> (<xsl:value-of select="length/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="length/value"></xsl:value-of>
        </td>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Interior length']"/> (<xsl:value-of select="innerLength/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="innerLength/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Ext. width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="width/value"></xsl:value-of>
        </td>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Interior width']"/> (<xsl:value-of select="innerWidth/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="innerWidth/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Ext. height']"/> (<xsl:value-of select="height/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="height/value"></xsl:value-of>
        </td>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Interior height']"/> (<xsl:value-of select="innerHeight/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="innerHeight/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2" colspan="1">
          <b>
            <xsl:value-of select="$loc/str[@name='Weight']"/> (<xsl:value-of select="weight/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3" colspan="1">
          <xsl:value-of select="weight/value"></xsl:value-of>
        </td>
        <td colspan="2" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### PALLET FILM ####-->
  <xsl:template match ="palletFilm">
    <h3>
      <xsl:value-of select="$loc/str[@name='Pallet film']"/>
    </h3>
    <table class="style1"  cellpadding="4">
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
    </table>
  </xsl:template>
  <!--#### BUNDLE ####-->
  <xsl:template match="bundle">
    <h3>
      <xsl:value-of select="$loc/str[@name='Bundle']"/>
    </h3>
    <table class="style1">
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
      <tr>
        <td class="style2">
          <strong>
            <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
          </strong>
        </td>
        <td class="style3">
          <xsl:value-of select="length/value"></xsl:value-of>
        </td>
        <td rowspan="6" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="width/value"></xsl:value-of>
        </td>
      </tr>
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
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Unit thickness']"/> (<xsl:value-of select="unitThickness/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="unitThickness/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Unit weight']"/> (<xsl:value-of select="unitWeight/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="unitWeight/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style3">
          <b>
            <xsl:value-of select="$loc/str[@name='Total thickness']"/> (<xsl:value-of select="totalThickness/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="totalThickness/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style3">
          <b>
            <xsl:value-of select="$loc/str[@name='Total weight']"/> (<xsl:value-of select="weightTotal/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="weightTotal/value"></xsl:value-of>
        </td>
      </tr>
    </table>
  </xsl:template>
  <!--#### TRUCK ####-->
  <xsl:template match="truck">
    <h3>
      <xsl:value-of select="$loc/str[@name='Truck']"/>
    </h3>
    <table class="style1" cellpadding="3">
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
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Description']"/>
          </b>
        </td>
        <td class="style3" colspan="2">
          <xsl:value-of select="description"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Length']"/> (<xsl:value-of select="length/unit"/>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="length/value"></xsl:value-of>
        </td>
        <td rowspan="4" align="middle">
          <xsl:apply-templates select="imageThumbSize"/>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Width']"/> (<xsl:value-of select="width/unit"/>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="width/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Height']"/> (<xsl:value-of select="height/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="height/value"></xsl:value-of>
        </td>
      </tr>
      <tr>
        <td class="style2">
          <b>
            <xsl:value-of select="$loc/str[@name='Admissible load']"/> (<xsl:value-of select="admissibleLoad/unit"></xsl:value-of>)
          </b>
        </td>
        <td class="style3">
          <xsl:value-of select="admissibleLoad/value"></xsl:value-of>
        </td>
      </tr>
    </table>
  </xsl:template>
</xsl:stylesheet>