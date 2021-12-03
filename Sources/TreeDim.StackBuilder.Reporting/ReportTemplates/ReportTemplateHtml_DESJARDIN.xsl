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
            <style>
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
                <p class="MsoNormal" style='margin-top: 0cm; margin-right: 800pt; margin-bottom: 0cm; margin-left: -72.0pt'>
                   
                </p>
                <table class="TableGrid" border="0" cellspacing="0" cellpadding="0" width="1026" style='width: 600pt; margin-left: 0pt; border-collapse: collapse'>
                     <tr style='height: 391.55pt'>
                        <td width="600" colspan="4" valign="top" style='width: 600pt; border: solid black 1.0pt; border-bottom: none; padding: 0cm 0cm 0cm 3.15pt; height: 800pt'>
                            <p class="MsoNormal" style='margin-top: 0cm; margin-right: -.1pt; margin-bottom: 0cm; margin-left: 8.55pt'>
								<xsl:apply-templates select="analysis" />
                            </p>
                        </td>
                    </tr>
                    <tr style='height: 33.35pt'>
                        <td width="506" rowspan="5" valign="bottom" style='width: 379.2pt; border: solid black 1.0pt; border-top: none; padding: 0cm 0cm 0cm 3.15pt; height: 33.35pt'>
                            <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 2.1pt'>
                                <span style='font-size: 8.0pt; line-height: 107%; font-family: "Times New Roman",serif'>This drawing belongs to DESJARDIN and cannot be transferred to anyone without DESJARDIN authorization.
                                </span>
                            </p>
                            <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 2.1pt'>
                                <span style='font-size: 8.0pt; line-height: 107%; font-family: "Times New Roman",serif'>Les poids sont donnes a titre indicatif avec +/-5%. 
                                </span>
                            </p>
                        </td>
                        <td width="521" colspan="3" style='width: 390.6pt; border: solid black 1.0pt; border-left: none; padding: 0cm 0cm 0cm 3.15pt; height: 33.35pt'>
                            <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 5.7pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>PLAN DE PALETTISATION
                                </span>
                            </p>
                        </td>
                    </tr>
                    <tr style='height: 23.8pt'>
                        <td width="347" colspan="2" rowspan="3" valign="top" style='width: 260.4pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                            <p class="MsoNormal" style='margin-top: 0cm; margin-right: 0cm; margin-bottom: 0cm; margin-left: 7.05pt'>
                                <img width="327" height="77" src="images\logo_DESJARDIN.gif"/>
                            </p>
                        </td>
                        <td width="174" style='width: 130.15pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                            <p class="MsoNormal" style='margin-bottom: 0cm'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>Drawned by:
                                </span>
                            </p>
                        </td>
                    </tr>
                    <tr style='height: 23.8pt'>
                        <td width="174" style='width: 130.15pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 23.8pt'>
                            <p class="MsoNormal" style='margin-bottom: 0cm'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>Checked by:
                                </span>
                            </p>
                        </td>
                    </tr>
                    <tr style='height: 22.15pt'>
                        <td width="174" style='width: 130.15pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 22.15pt'>
                            <p class="MsoNormal" style='margin-bottom: 0cm'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>Date:
                                </span>
                            </p>
                        </td>
                    </tr>
                    <tr style='height: 28.4pt'>
                        <td width="173" style='width: 130.1pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                            <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 3.2pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>PAPER FORMAT: A4
                                </span>
                            </p>
                        </td>
                        <td width="174" style='width: 130.35pt; border: solid black 1.0pt; border-left: none; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                            <p class="MsoNormal" style='margin-bottom: 0cm'>
                                <span style='font-size: 10.0pt; line-height: 107%; font-family: "Times New Roman",serif'>Ref.:
                                </span>
                            </p>
                        </td>
                        <td width="174" valign="top" style='width: 130.15pt; border-top: none; border-left: none; border-bottom: solid black 1.0pt; border-right: solid black 1.0pt; padding: 0cm 0cm 0cm 3.15pt; height: 28.4pt'>
                            <p class="MsoNormal" align="center" style='margin-top: 0cm; margin-right: 3.15pt; margin-bottom: 0cm; margin-left: 0cm; text-align: center'>
                                <span style='font-size: 18.0pt; line-height: 107%; font-family: "Times New Roman",serif'>1/1
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
    <xsl:apply-templates select="pallet"/>
    <xsl:apply-templates select="constraintSet"/>
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

	<!--#### CONSTRAINT SET ####-->
	<xsl:template match="constraintSet">
		<h3>
			<xsl:value-of select="$loc/str[@name='Constraint set']"/>
		</h3>
		<table class="style1">
			<xsl:if test="overhang">
				<tr>
					<td class="style2">
						<b>
							<xsl:value-of select="$loc/str[@name='Overhang Length x Width']"/>
						</b>
					</td>
					<td class="style3">
						<xsl:apply-templates select="overhang/unitVector2"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="maximumHeight">
				<tr>
					<td class="style2">
						<b>
							<xsl:value-of select="$loc/str[@name='Maximum pallet height']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="maximumHeight/unitValue"/>)
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="maximumWeight">
				<tr>
					<td class="style2">
						<b>
							<xsl:value-of select="$loc/str[@name='Maximum weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="maximumWeight/unitValue"/>
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
							<xsl:value-of select="$loc/str[@name='Net weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="netWeight/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="weightLoad">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Load weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="weightLoad/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="bboxLoad">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Load dimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="bboxLoad/unitVector3"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="weightTotal">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="weightTotal/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="bboxTotal">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Overall dimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="bboxTotal/unitVector3"/>
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
		<xsl:apply-templates select="layers"/>
	</xsl:template>
	<!---->
	<xsl:template match="solutionPalletsOnPallet">
		<h3>
			<xsl:value-of select="$loc/str[@name='Solution']"/>
		</h3>
		<table class="style1">
			<tr>
				<td>
					<xsl:apply-templates select="item"/>
				</td>
			</tr>
			<xsl:if test="weightLoad">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Load weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="weightLoad/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="bboxLoad">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Load dimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="bboxLoad/unitVector3"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="weightTotal">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Weight']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="weightTotal/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="bboxTotal">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Overall dimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="3">
						<xsl:apply-templates select="bboxTotal/unitVector3"/>
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
			<xsl:if test="layerDimensions">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Dimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="layerDimensions/unitVector2"/>/>)
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="layerWeight">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Weight']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="layerWeight/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="netWeight">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Net weight']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="layerNetWeight/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="layerSpaces">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Spaces']"/>
						</b>
					</td>
					<td class="style3" colspace="1">
						<xsl:apply-templates select="layerSpaces/unitValue"/>
					</td>
				</tr>
			</xsl:if>
		</table>
	</xsl:template>
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
						<xsl:value-of select="$loc/str[@name='Dimensions']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="dimensions/unitVector3"/>
				</td>
				<td rowspan="5" align="middle">
					<xsl:apply-templates select="imageThumbSize"/>
				</td>
			</tr>
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
			<xsl:if test="admissibleLoad">
				<tr>
					<td class="style2">
						<b>
							<xsl:value-of select="$loc/str[@name='Admissible load on top']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="admissibleLoad/unitValue"/>
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
						<xsl:value-of select="$loc/str[@name='Dimensions']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="dimensions/unitVector3"/>
				</td>
				<td rowspan="5" colspan="2" align="middle">
					<xsl:apply-templates select="imageThumbSize"/>
				</td>
			</tr>
			<tr>
				<td class="style2" colspan="1">
					<b>
						<xsl:value-of select="$loc/str[@name='Net weight']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="netWeight/unitValue"/>
				</td>
			</tr>
			<tr>
				<td class="style2" colspan="1">
					<b>
						<xsl:value-of select="$loc/str[@name='Wrapper weight']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="wrapperWeight/unitValue"/>
				</td>
			</tr>
			<tr>
				<td class="style2" colspan="1">
					<b>
						<xsl:value-of select="$loc/str[@name='Tray weight']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="trayWeight/unitValue"/>
				</td>
			</tr>
			<tr>
				<td class="style2" colspan="1">
					<b>
						<xsl:value-of select="$loc/str[@name='Weight']"/>
					</b>
				</td>
				<td class="style3" colspan="1">
					<xsl:apply-templates select="weight/unitValue"/>
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
							<xsl:value-of select="$loc/str[@name='Diameter']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="diameter/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="height">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Height']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="height/unitValue"/>
					</td>
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
				<xsl:if test="innerDimensions">
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='InnerDimensions']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="innerDimensions/unitVector3"/>
					</td>
				</xsl:if>
			</tr>
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
				<xsl:if test="imageThumbSize">
					<td rowspan="3" colspan="2" align="middle">
						<xsl:apply-templates select="imageThumbSize"/>
					</td>
				</xsl:if>
			</tr>
			<xsl:if test="weight">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Weight']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="weight/unitValue"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="admissibleLoad">
				<tr>
					<td class="style2" colspan="1">
						<b>
							<xsl:value-of select="$loc/str[@name='Admissible load weight']"/>
						</b>
					</td>
					<td class="style3" colspan="1">
						<xsl:apply-templates select="admissibleLoad/unitValue"/>
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
	<!--#### BAG ####-->
	<xsl:template match ="bag">
		<h3>
			<xsl:value-of select="$loc/str[@name='Bag']"/>
		</h3>
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
</xsl:stylesheet>
