#region Using directives
using System;
using System.IO;
using System.Collections.Generic;
#endregion

namespace Sharp3D.Boxologic
{
    public class Boxlogic
    {
        #region Private data members
        private DateTime TimeStart { get; set; }
        private DateTime TimeStop { get; set; }
        private PalletInfo pallet;
        private List<BoxInfo> boxList = new List<BoxInfo>();
        private List<Layer> layers = new List<Layer>();
        private Scrappad scrapfirst, smallestz;
        private Scrappad trash;
        private Dictionary<int, KeyValuePair<int, decimal>> best_iterations = new Dictionary<int, KeyValuePair<int, decimal>>();

        private bool packing = true, layerdone = false, evened = false;
        private int best_variant;
        private decimal layerinlayer;
        private decimal prelayer;
        private decimal lilz;
        private int number_of_iterations;
        private decimal remainpy, preremainpy, remainpz;
        private decimal packedy, prepackedy;
        private decimal layerThickness;
        private int best_iteration;
        private int packednumbox;
        private int number_packed_boxes;
        private decimal packedvolume;
        private decimal best_solution_volume;
        private decimal total_box_volume;
        private double pallet_volume_used_percentage;
        private StreamWriter fso;
        public bool Debug { get; set; } = false;
        #endregion
        #region Public properties
        public string OutputFilePath { get; set; }
        #endregion
        #region Run / Initialize / Execute_iterations
        public void Run(BoxItem[] listBoxItem, decimal palletLength, decimal palletWidth, decimal palletHeight, ref SolutionArray solArray)
        {
            packing = true;

            // boxes
            foreach (BoxItem bi in listBoxItem)
            {
                for (int i = 0; i < bi.N; ++i)
                    boxList.Add(
                        new BoxInfo()
                        {
                            ID = bi.ID,
                            Dim1 = bi.Boxx,
                            Dim2 = bi.Boxy,
                            Dim3 = bi.Boxz,
                            N = bi.N,
                            AllowX = bi.AllowX,
                            AllowY = bi.AllowY,
                            AllowZ = bi.AllowZ
                        }
                        );
            }
            // pallet
            pallet = new PalletInfo(palletLength, palletWidth, palletHeight);
            // output file
            if (string.IsNullOrEmpty(OutputFilePath))
                fso = null;
            else
                fso = new StreamWriter(OutputFilePath, true);

            TimeStart = DateTime.Now;
            Initialize();
            Execute_iterations();
            TimeStop = DateTime.Now;
            Report_results2(ref solArray);
        }

        public void Run(int variant, BoxItem[] listBoxItem, decimal palletLength, decimal palletWidth, decimal palletHeight, ref SolutionArray solArray)
        {
            packing = true;

            // boxes
            foreach (BoxItem bi in listBoxItem)
            {
                for (int i = 0; i < bi.N; ++i)
                    boxList.Add(
                        new BoxInfo()
                        {
                            ID = bi.ID,
                            Dim1 = bi.Boxx,
                            Dim2 = bi.Boxy,
                            Dim3 = bi.Boxz,
                            N = bi.N,
                            AllowX = bi.AllowX,
                            AllowY = bi.AllowY,
                            AllowZ = bi.AllowZ
                        }
                        );
            }
            // pallet
            pallet = new PalletInfo(palletLength, palletWidth, palletHeight);
            // output file
            if (string.IsNullOrEmpty(OutputFilePath))
                fso = null;
            else
                fso = new StreamWriter(OutputFilePath, true);

            TimeStart = DateTime.Now;
            Initialize();
            Execute_iterations(variant);
            TimeStop = DateTime.Now;
            Report_results2(ref solArray);
        }

        private void Initialize()
        {
            total_box_volume = 0;
            foreach (BoxInfo bi in boxList)
            { total_box_volume += bi.Vol; }

            scrapfirst = new Scrappad();
            best_solution_volume = 0;
            number_of_iterations = 0;
        }
        public void Execute_iterations()
        {
            bool hundredpercent = false;
            for (int variant = 1; variant < 6; ++variant)
            {
                best_solution_volume = 0;
                number_of_iterations = 0;

                // initialize pallet
                pallet.Variant = variant;
                // LISTS ALL POSSIBLE LAYER HEIGHTS BY GIVING A WEIGHT VALUE TO EACH OF THEM.
                List_candidate_layers(variant);
                int iLayerIndex = 0;
                foreach (Layer l in layers)
                {
                    ++number_of_iterations;
                    double elapsed_time = (DateTime.Now - TimeStart).TotalSeconds;
                    if (Debug)
                        Console.WriteLine($"VARIANT: {variant,5:#####}; ITERATION (TOTAL): {number_of_iterations,5:#####}; BEST SO FAR: {pallet_volume_used_percentage:0.000}; TIME: {elapsed_time:0.00}");
                    packedvolume = 0;
                    packedy = 0;
                    packing = true;
                    layerThickness = l.LayerDim;
                    int itelayer = iLayerIndex++;
                    remainpy = pallet.Pallet_y;
                    remainpz = pallet.Pallet_z;
                    packednumbox = 0;
                    foreach (BoxInfo bi in boxList)
                        bi.Is_packed = false;

                    // ### BEGIN DO-WHILE
                    do
                    {
                        layerinlayer = 0;
                        layerdone = false;
                        Pack_layer(variant, false, ref hundredpercent);
                        packedy = packedy + layerThickness;
                        remainpy = pallet.Pallet_y - packedy;
                        if (0 != layerinlayer)
                        {
                            prepackedy = packedy;
                            preremainpy = remainpy;
                            remainpy = layerThickness - prelayer;
                            packedy = packedy - layerThickness + prelayer;
                            remainpz = lilz;
                            layerThickness = layerinlayer;
                            layerdone = false;
                            Pack_layer(variant, false, ref hundredpercent);
                            packedy = prepackedy;
                            remainpy = preremainpy;
                            remainpz = pallet.Pallet_z;
                        }
                        Find_layer(remainpy, pallet);
                    }
                    while (packing);
                    // END DO-WHILE

                    decimal best_solution_volume2 = 0;
                    if (best_iterations.ContainsKey(variant))
                        best_solution_volume2 = best_iterations[variant].Value;

                    if (packedvolume >= best_solution_volume2/*best_solution_volume*/)
                    {
                        best_solution_volume = packedvolume;
                        best_variant = variant;
                        best_iteration = itelayer;
                        number_packed_boxes = packednumbox;

                        best_iterations[variant] = new KeyValuePair<int, decimal>(itelayer, packedvolume);
                    }
                    if (hundredpercent) break;
                    pallet_volume_used_percentage = (double)best_solution_volume * 100 / (double)pallet.Vol;
                }
                //if (hundredpercent) break;
                if ((pallet.Dim1 == pallet.Dim2) && (pallet.Dim2 == pallet.Dim3))
                    variant = 5;
            }
        }
        public void Execute_iterations(int variant)
        {
            bool hundredpercent = false;
            best_solution_volume = 0;
            number_of_iterations = 0;

            // initialize pallet
            pallet.Variant = variant;
            // LISTS ALL POSSIBLE LAYER HEIGHTS BY GIVING A WEIGHT VALUE TO EACH OF THEM.
            List_candidate_layers(variant);
            int iLayerIndex = 0;
            foreach (Layer l in layers)
            {
                ++number_of_iterations;
                double elapsed_time = (DateTime.Now - TimeStart).TotalSeconds;
                if (Debug)
                {
                    Console.WriteLine(
                        $"VARIANT: {variant,5:#####}; ITERATION (TOTAL): {number_of_iterations,5:#####}; BEST SO FAR: {pallet_volume_used_percentage:0.000}; TIME: {elapsed_time:0.00}");
                }
                packedvolume = 0;
                packedy = 0;
                packing = true;
                layerThickness = l.LayerDim;
                int itelayer = iLayerIndex++;
                remainpy = pallet.Pallet_y;
                remainpz = pallet.Pallet_z;
                packednumbox = 0;
                foreach (BoxInfo bi in boxList)
                    bi.Is_packed = false;

                // ### BEGIN DO-WHILE
                do
                {
                    layerinlayer = 0;
                    layerdone = false;
                    Pack_layer(variant, false, ref hundredpercent);
                    packedy += layerThickness;
                    remainpy = pallet.Pallet_y - packedy;
                    if (0 != layerinlayer)
                    {
                        prepackedy = packedy;
                        preremainpy = remainpy;
                        remainpy = layerThickness - prelayer;
                        packedy = packedy - layerThickness + prelayer;
                        remainpz = lilz;
                        layerThickness = layerinlayer;
                        layerdone = false;
                        Pack_layer(variant, false, ref hundredpercent);
                        packedy = prepackedy;
                        remainpy = preremainpy;
                        remainpz = pallet.Pallet_z;
                    }
                    Find_layer(remainpy, pallet);
                }
                while (packing);
                // END DO-WHILE

                decimal best_solution_volume2 = 0;
                if (best_iterations.ContainsKey(variant))
                    best_solution_volume2 = best_iterations[variant].Value;

                if (packedvolume >= best_solution_volume2)
                {
                    best_solution_volume = packedvolume;
                    best_variant = variant;
                    best_iteration = itelayer;
                    number_packed_boxes = packednumbox;

                    best_iterations[variant] = new KeyValuePair<int, decimal>(itelayer, packedvolume);
                }
                if (hundredpercent) break;
                pallet_volume_used_percentage = (double)best_solution_volume * 100 / (double)pallet.Vol;
            }
        }
        #endregion
        #region List_candidate_layers (DONE)
        public void List_candidate_layers(int variant)
        {
            foreach (BoxInfo bi1 in boxList)
            {
                for (int y = 1; y <= 3; ++y)
                {
                    if (!bi1.AllowX && y == 1) continue;
                    if (!bi1.AllowY && y == 2) continue;
                    if (!bi1.AllowZ && y == 3) continue;

                    decimal exdim = 0, dimen2 = 0, dimen3 = 0;
                    switch (y)
                    {
                        case 1:
                            exdim = bi1.Dim1;
                            dimen2 = bi1.Dim2;
                            dimen3 = bi1.Dim3;
                            break;
                        case 2:
                            exdim = bi1.Dim2;
                            dimen2 = bi1.Dim1;
                            dimen3 = bi1.Dim3;
                            break;
                        case 3:
                            exdim = bi1.Dim3;
                            dimen2 = bi1.Dim1;
                            dimen3 = bi1.Dim2;
                            break;
                        default:
                            break;
                    }

                    if (
                        (exdim > pallet.Pallet_y)
                        ||
                        (
                            ((dimen2 > pallet.Pallet_x) || (dimen3 > pallet.Pallet_z))
                            && ((dimen3 > pallet.Pallet_x) || (dimen2 > pallet.Pallet_z))
                        )
                    )
                        continue;

                    if (null != layers.Find(lay => lay.LayerDim == exdim))
                        continue;

                    decimal layereval = 0;
                    foreach (BoxInfo bi2 in boxList)
                    {
                        if (bi1 != bi2)
                        {
                            decimal dimdif = Math.Abs(exdim - bi2.Dim1);
                            if (Math.Abs(exdim - bi2.Dim2) < dimdif)
                                dimdif = Math.Abs(exdim - bi2.Dim2);
                            if (Math.Abs(exdim - bi2.Dim3) < dimdif)
                                dimdif = Math.Abs(exdim - bi2.Dim3);
                            layereval += dimdif;
                        }
                    }
                    layers.Add(new Layer() { LayerEval = layereval, LayerDim = exdim });
                }
            }
            layers.Sort(new LayerComparer());
        }
        #endregion
        #region Find_smallest_z (DONE)
        //----------------------------------------------------------------------------
        // FINDS THE FIRST TO BE PACKED GAP IN THE LAYER EDGE
        //----------------------------------------------------------------------------
        private Scrappad Find_smallest_z()
        {
            Scrappad scrapmemb = scrapfirst;
            Scrappad sz = scrapfirst;
            while (null != scrapmemb.Next)
            {
                if (scrapmemb.Next.Cumz < sz.Cumz)
                    sz = scrapmemb.Next;
                scrapmemb = scrapmemb.Next;
            }
            return sz;
        }
        #endregion
        #region Pack_layer (DONE)
        ///----------------------------------------------------------------------------
        /// PACKS THE BOXES FOUND AND ARRANGES ALL VARIABLES AND RECORDS PROPERLY
        ///----------------------------------------------------------------------------
        private bool Pack_layer(int variant, bool packingbest, ref bool hundredpercent)
        {
            if (0 == layerThickness)
            {
                packing = false;
                return false;
            }

            scrapfirst = new Scrappad()
            {
                Cumx = pallet.Pallet_x,
                Cumz = 0
            };
            decimal cboxi = 0, cboxx = 0, cboxy = 0, cboxz = 0;
            while (true)
            {
                smallestz = Find_smallest_z();

                if (null == smallestz.Prev && null == smallestz.Next)
                {
                    //*** SITUATION-1: NO BOXES ON THE RIGHT AND LEFT SIDES ***
                    decimal lenx = smallestz.Cumx;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(variant, lenx, layerThickness, remainpy, lpz, lpz,
                        ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    boxList[(int)cboxi].Cox = 0;
                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx)
                    {
                        smallestz.Cumz += cboxz;
                    }
                    else
                    {
                        smallestz.Next = new Scrappad
                        {
                            Prev = smallestz,
                            Cumx = smallestz.Cumx,
                            Cumz = smallestz.Cumz
                        };
                        smallestz.Cumx = cboxx;
                        smallestz.Cumz += cboxz;
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (null == smallestz.Prev)
                {
                    //*** SITUATION-2: NO BOXES ON THE LEFT SIDE ***
                    decimal lenx = smallestz.Cumx;
                    decimal lenz = smallestz.Next.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(variant, lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    BoxInfo bi = boxList[(int)cboxi];
                    bi.Coy = packedy;
                    bi.Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx)
                    {
                        bi.Cox = 0;
                        if (smallestz.Cumz + cboxz == smallestz.Next.Cumz)
                        {
                            smallestz.Cumz = smallestz.Next.Cumz;
                            smallestz.Cumx = smallestz.Next.Cumx;
                            trash = smallestz.Next;
                            smallestz.Next = smallestz.Next.Next;
                            if (null != smallestz.Next)
                            {
                                smallestz.Next.Prev = smallestz;
                            }
                        }
                        else
                        {
                            smallestz.Cumz += cboxz;
                        }
                    }
                    else
                    {
                        bi = boxList[(int)cboxi];
                        bi.Cox = smallestz.Cumx - cboxx;
                        if (smallestz.Cumz + cboxz == smallestz.Next.Cumz)
                        {
                            smallestz.Cumx -= cboxx;
                        }
                        else
                        {
                            smallestz.Next.Prev = new Scrappad
                            {
                                Next = smallestz.Next,
                                Prev = smallestz
                            };
                            smallestz.Next = smallestz.Next.Prev;
                            smallestz.Next.Cumx = smallestz.Cumx;
                            smallestz.Cumx -= cboxx;
                            smallestz.Next.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (null == smallestz.Next)
                {
                    //*** SITUATION-3: NO BOXES ON THE RIGHT SIDE ***
                    decimal lenx = smallestz.Cumx - smallestz.Prev.Cumx;
                    decimal lenz = smallestz.Prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(variant, lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    BoxInfo bi = boxList[(int)cboxi];
                    bi.Coy = packedy;
                    bi.Coz = smallestz.Cumz;
                    bi.Cox = smallestz.Prev.Cumx;

                    if (cboxx == smallestz.Cumx - smallestz.Prev.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Cumx;
                            smallestz.Prev.Next = null;
                        }
                        else
                        {
                            smallestz.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Prev.Cumx + cboxx;
                        }
                        else
                        {
                            smallestz.Prev.Next = new Scrappad
                            {
                                Prev = smallestz.Prev,
                                Next = smallestz
                            };
                            smallestz.Prev = smallestz.Prev.Next;
                            smallestz.Prev.Cumx = smallestz.Prev.Prev.Cumx + cboxx;
                            smallestz.Prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else if (smallestz.Prev.Cumz == smallestz.Next.Cumz)
                {
                    //*** SITUATION-4: THERE ARE BOXES ON BOTH OF THE SIDES ***
                    //*** SUBSITUATION-4A: SIDES ARE EQUAL TO EACH OTHER ***
                    decimal lenx = smallestz.Cumx - smallestz.Prev.Cumx;
                    decimal lenz = smallestz.Prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(variant, lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    if (cboxx == smallestz.Cumx - smallestz.Prev.Cumx)
                    {
                        boxList[(int)cboxi].Cox = smallestz.Prev.Cumx;
                        if (smallestz.Cumz + cboxz == smallestz.Next.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Next.Cumx;
                            if (null != smallestz.Next.Next)
                            {
                                smallestz.Prev.Next = smallestz.Next.Next;
                                smallestz.Next.Next.Prev = smallestz.Prev;
                            }
                            else
                            {
                                smallestz.Prev.Next = null;
                            }
                        }
                        else
                        {
                            smallestz.Cumz += cboxz;
                        }
                    }
                    else if (smallestz.Prev.Cumx < pallet.Pallet_x - smallestz.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Cumx -= cboxx;
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                        }
                        else
                        {
                            boxList[(int)cboxi].Cox = smallestz.Prev.Cumx;
                            smallestz.Prev.Next = new Scrappad
                            {
                                Prev = smallestz.Prev,
                                Next = smallestz
                            };
                            smallestz.Prev = smallestz.Prev.Next;
                            smallestz.Prev.Cumx = smallestz.Prev.Prev.Cumx + cboxx;
                            smallestz.Prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Prev.Cumx + cboxx;
                            boxList[(int)cboxi].Cox = smallestz.Prev.Cumx;
                        }
                        else
                        {
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                            smallestz.Next.Prev = new Scrappad
                            {
                                Next = smallestz.Next,
                                Prev = smallestz
                            };
                            smallestz.Next = smallestz.Next.Prev;
                            smallestz.Next.Cumx = smallestz.Cumx;
                            smallestz.Next.Cumz = smallestz.Cumz + cboxz;
                            smallestz.Cumx -= cboxx;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
                else
                {
                    //*** SUBSITUATION-4B: SIDES ARE NOT EQUAL TO EACH OTHER ***
                    decimal lenx = smallestz.Cumx - smallestz.Prev.Cumx;
                    decimal lenz = smallestz.Prev.Cumz - smallestz.Cumz;
                    decimal lpz = remainpz - smallestz.Cumz;
                    Find_box(variant, lenx, layerThickness, remainpy, lenz, lpz
                        , ref cboxi, ref cboxx, ref cboxy, ref cboxz);

                    if (layerdone) break;
                    if (evened) continue;

                    boxList[(int)cboxi].Coy = packedy;
                    boxList[(int)cboxi].Coz = smallestz.Cumz;
                    boxList[(int)cboxi].Cox = smallestz.Prev.Cumx;
                    if (cboxx == smallestz.Cumx - smallestz.Prev.Cumx)
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Cumx;
                            smallestz.Prev.Next = smallestz.Next;
                            smallestz.Next.Prev = smallestz.Prev;
                        }
                        else
                            smallestz.Cumz += cboxz;
                    }
                    else
                    {
                        if (smallestz.Cumz + cboxz == smallestz.Prev.Cumz)
                        {
                            smallestz.Prev.Cumx = smallestz.Prev.Cumx + cboxx;
                        }
                        else if (smallestz.Cumz + cboxz == smallestz.Next.Cumz)
                        {
                            boxList[(int)cboxi].Cox = smallestz.Cumx - cboxx;
                            smallestz.Cumx -= cboxx;
                        }
                        else
                        {
                            smallestz.Prev.Next = new Scrappad
                            {
                                Prev = smallestz.Prev,
                                Next = smallestz
                            };
                            smallestz.Prev = smallestz.Prev.Next;
                            smallestz.Prev.Cumx = smallestz.Prev.Prev.Cumx + cboxx;
                            smallestz.Prev.Cumz = smallestz.Cumz + cboxz;
                        }
                    }
                    VolumeCheck((int)cboxi, cboxx, cboxy, cboxz, packingbest, ref hundredpercent);
                }
            }
            return true;
        }
        #endregion
        #region Find_layer (DONE)
        ///----------------------------------------------------------------------------
        /// FINDS THE MOST PROPER LAYER HEIGHT BY LOOKING AT THE UNPACKED BOXES AND THE
        /// REMAINING EMPTY SPACE AVAILABLE
        ///----------------------------------------------------------------------------
        private int Find_layer(decimal thickness, PalletInfo p)
        {
            decimal exdim = 0, dimdif = 0, dimen2 = 0, dimen3 = 0;
            decimal layereval = 0, eval = decimal.MaxValue;
            layerThickness = 0;
            for (int x = 0; x < boxList.Count; x++)
            {
                BoxInfo bi = boxList[x];
                if (bi.Is_packed)
                    continue;
                for (int y = 1; y <= 3; y++)
                {
                    if (!bi.AllowX && y == 1) continue;
                    if (!bi.AllowY && y == 2) continue;
                    if (!bi.AllowZ && y == 3) continue;

                    switch (y)
                    {
                        case 1:
                            exdim = bi.Dim1;
                            dimen2 = bi.Dim2;
                            dimen3 = bi.Dim3;
                            break;
                        case 2:
                            exdim = bi.Dim2;
                            dimen2 = bi.Dim1;
                            dimen3 = bi.Dim3;
                            break;
                        case 3:
                            exdim = bi.Dim3;
                            dimen2 = bi.Dim1;
                            dimen3 = bi.Dim2;
                            break;
                        default:
                            break;
                    }
                    layereval = 0;
                    if ((exdim <= thickness)
                        && (((dimen2 <= p.Pallet_x) && (dimen3 <= p.Pallet_z))
                        || ((dimen3 <= p.Pallet_x) && (dimen2 <= p.Pallet_z))))
                    {
                        for (int z = 0; z < boxList.Count; z++)
                        {
                            if (!(x == z) && !(boxList[z].Is_packed))
                            {
                                dimdif = Math.Abs(exdim - boxList[z].Dim1);
                                if (Math.Abs(exdim - boxList[z].Dim2) < dimdif)
                                    dimdif = Math.Abs(exdim - boxList[z].Dim2);
                                if (Math.Abs(exdim - boxList[z].Dim3) < dimdif)
                                    dimdif = Math.Abs(exdim - boxList[z].Dim3);
                                layereval += dimdif;
                            }
                        }
                        if (layereval < eval)
                        {
                            eval = layereval;
                            layerThickness = exdim;
                        }
                    }
                }
            }
            if (layerThickness == 0 || layerThickness > remainpy)
                packing = false;
            return 0;
        }
        #endregion
        #region CheckFound (DONE)
        /// <summary>
        /// AFTER FINDING EACH BOX, THE CANDIDATE BOXES AND THE CONDITION OF THE LAYER
        /// ARE EXAMINED
        /// </summary>
        private void CheckFound(
            ref decimal cboxi, ref decimal cboxx, ref decimal cboxy, ref decimal cboxz
            , decimal boxi, decimal boxx, decimal boxy, decimal boxz
            , decimal bboxi, decimal bboxx, decimal bboxy, decimal bboxz)
        {
            evened = false;
            if (boxi >= 0 && 0 != boxx * boxy * boxz)
            {
                cboxi = boxi;
                cboxx = boxx;
                cboxy = boxy;
                cboxz = boxz;
            }
            else
            {
                if ((bboxi >= 0 && 0 != bboxx * bboxy * bboxz) && (layerinlayer > 0 || (null == smallestz.Prev && null == smallestz.Next)))
                {
                    if (layerinlayer == 0.0M)
                    {
                        prelayer = layerThickness;
                        lilz = smallestz.Cumz;
                    }
                    cboxi = bboxi;
                    cboxx = bboxx;
                    cboxy = bboxy;
                    cboxz = bboxz;
                    layerinlayer = layerinlayer + bboxy - layerThickness;
                    layerThickness = bboxy;
                }
                else
                {
                    if (null == smallestz.Prev && null == smallestz.Next)
                    {
                        layerdone = true;
                    }
                    else
                    {
                        evened = true;
                        if (null == smallestz.Prev)
                        {
                            trash = smallestz.Next;
                            smallestz.Cumx = smallestz.Next.Cumx;
                            smallestz.Cumz = smallestz.Next.Cumz;
                            smallestz.Next = smallestz.Next.Next;
                            if (null != smallestz.Next)
                            {
                                smallestz.Next.Prev = smallestz;
                            }
                            trash = null;
                        }
                        else if (null == smallestz.Next)
                        {
                            smallestz.Prev.Next = null;
                            smallestz.Prev.Cumx = smallestz.Cumx;
                            smallestz = null;
                        }
                        else
                        {
                            if (smallestz.Prev.Cumz == smallestz.Next.Cumz)
                            {
                                smallestz.Prev.Next = smallestz.Next.Next;
                                if (null != smallestz.Next.Next)
                                {
                                    smallestz.Next.Next.Prev = smallestz.Prev;
                                }
                                smallestz.Prev.Cumx = smallestz.Next.Cumx;
                                smallestz.Next = null;
                                smallestz = null;
                            }
                            else
                            {
                                smallestz.Prev.Next = smallestz.Next;
                                smallestz.Next.Prev = smallestz.Prev;
                                if (smallestz.Prev.Cumz < smallestz.Next.Cumz)
                                {
                                    smallestz.Prev.Cumx = smallestz.Cumx;
                                }
                                smallestz = null;
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #region VolumeCheck (DONE)
        /// <summary>
        /// AFTER PACKING OF EACH BOX, 100% PACKING CONDITION IS CHECKED
        /// </summary>
        private void VolumeCheck(int cboxi, decimal cboxx, decimal cboxy, decimal cboxz, bool packingbest
            , ref bool hundredpercent)
        {
            BoxInfo bi = boxList[cboxi];
            bi.SetPacked(cboxx, cboxy, cboxz);
            packedvolume += bi.Vol;
            packednumbox++;
            if (packingbest)
            {
                bi.WriteToFile(fso, best_variant, 0);
            }
            else if (packedvolume == pallet.Vol || packedvolume == total_box_volume)
            {
                packing = false;
                hundredpercent = true;
            }
        }
        #endregion
        #region Find_box (DONE)
        /// <summary>
        /// FINDS THE MOST PROPER BOXES BY LOOKING AT ALL SIX POSSIBLE ORIENTATIONS,
        /// EMPTY SPACE GIVEN, ADJACENT BOXES, AND PALLET LIMITS 
        /// </summary>
        private void Find_box(int variant, decimal hmx, decimal hy, decimal hmy, decimal hz, decimal hmz,
            ref decimal cboxi, ref decimal cboxx, ref decimal cboxy, ref decimal cboxz)
        {
            decimal boxi = 0;
            decimal boxx = 0, boxy = 0, boxz = 0;
            decimal bboxi = 0;
            decimal bboxx = 0, bboxy = 0, bboxz = 0;
            decimal bfx = decimal.MaxValue, bfy = decimal.MaxValue, bfz = decimal.MaxValue;
            decimal bbfx = decimal.MaxValue, bbfy = decimal.MaxValue, bbfz = decimal.MaxValue;
            boxi = 0; bboxi = 0;

            for (int y = 0; y < boxList.Count; y += boxList[y].N)
            {
                int x = y;
                for (x = y; x < y + boxList[y].N - 1; x++)
                {
                    if (!boxList[x].Is_packed) break;
                }
                if (boxList[x].Is_packed) continue;
                if (x >= boxList.Count)
                    return;
                BoxInfo bi = boxList[x];
                // 1 2 3
                if ((variant == 1 && bi.AllowZ)
                    || (variant == 2 && bi.AllowX)
                    || (variant == 3 && bi.AllowX)
                    || (variant == 4 && bi.AllowZ)
                    || (variant == 5 && bi.AllowY)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz,
                        bi.Dim1, bi.Dim2, bi.Dim3
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);

                    if ((bi.Dim1 == bi.Dim3) && (bi.Dim3 == bi.Dim2))
                        continue;
                }
                // 1 3 2
                if ((variant == 1 && bi.AllowY)
                    || (variant == 2 && bi.AllowX)
                    || (variant == 3 && bi.AllowX)
                    || (variant == 4 && bi.AllowY)
                    || (variant == 5 && bi.AllowZ)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz
                        , bi.Dim1, bi.Dim3, bi.Dim2
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                }

                // 2 1 3
                if ((variant == 1 && bi.AllowZ)
                    || (variant == 2 && bi.AllowY)
                    || (variant == 3 && bi.AllowY)
                    || (variant == 4 && bi.AllowZ)
                    || (variant == 5 && bi.AllowX)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz
                        , bi.Dim2, bi.Dim1, bi.Dim3
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                }
                // 2 3 1
                if ((variant == 1 && bi.AllowX)
                    || (variant == 2 && bi.AllowY)
                    || (variant == 3 && bi.AllowY)
                    || (variant == 4 && bi.AllowX)
                    || (variant == 5 && bi.AllowZ)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz
                        , bi.Dim2, bi.Dim3, bi.Dim1
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                }
                // 3 1 2
                if ((variant == 1 && bi.AllowY)
                    || (variant == 2 && bi.AllowZ)
                    || (variant == 3 && bi.AllowZ)
                    || (variant == 4 && bi.AllowY)
                    || (variant == 5 && bi.AllowX)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz
                        , bi.Dim3, bi.Dim1, bi.Dim2
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                }
                // 3 2 1
                if ((variant == 1 && bi.AllowX)
                    || (variant == 2 && bi.AllowZ)
                    || (variant == 3 && bi.AllowZ)
                    || (variant == 4 && bi.AllowX)
                    || (variant == 5 && bi.AllowY)
                    )
                {
                    Analyse_box(x, hmx, hy, hmy, hz, hmz
                        , bi.Dim3, bi.Dim2, bi.Dim1
                        , ref bfx, ref bfy, ref bfz
                        , ref bbfx, ref bbfy, ref bbfz
                        , ref boxi, ref boxx, ref boxy, ref boxz
                        , ref bboxi, ref bboxx, ref bboxy, ref bboxz);
                }
            }
            CheckFound(ref cboxi, ref cboxx, ref cboxy, ref cboxz
                , boxi, boxx, boxy, boxz
                , bboxi, bboxx, bboxy, bboxz);
        }
        #endregion
        #region Analyse_box
        /// <summary>
        /// ANALYZES EACH UNPACKED BOX TO FIND THE BEST FITTING ONE TO THE EMPTY SPACE
        /// GIVEN
        /// </summary>
        private void Analyse_box(decimal x, decimal hmx, decimal hy, decimal hmy, decimal hz, decimal hmz,
            decimal dim1, decimal dim2, decimal dim3
            , ref decimal bfx, ref decimal bfy, ref decimal bfz
            , ref decimal bbfx, ref decimal bbfy, ref decimal bbfz
            , ref decimal boxi, ref decimal boxx, ref decimal boxy, ref decimal boxz
            , ref decimal bboxi, ref decimal bboxx, ref decimal bboxy, ref decimal bboxz)
        {
            if (dim1 <= hmx && dim2 <= hmy && dim3 <= hmz)
            {
                if (dim2 <= hy)
                {
                    if (hy - dim2 < bfy)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                    else if (hy - dim2 == bfy && hmx - dim1 < bfx)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                    else if (hy - dim2 == bfy && hmx - dim1 == bfx && Math.Abs(hz - dim3) < bfz)
                    {
                        boxx = dim1;
                        boxy = dim2;
                        boxz = dim3;
                        bfx = hmx - dim1;
                        bfy = hy - dim2;
                        bfz = Math.Abs(hz - dim3);
                        boxi = x;
                    }
                }
                else
                {
                    if (dim2 - hy < bbfy)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                    else if (dim2 - hy == bbfy && hmx - dim1 < bbfx)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                    else if (dim2 - hy == bbfy && hmx - dim1 == bbfx && Math.Abs(hz - dim3) < bbfz)
                    {
                        bboxx = dim1;
                        bboxy = dim2;
                        bboxz = dim3;
                        bbfx = hmx - dim1;
                        bbfy = dim2 - hy;
                        bbfz = Math.Abs(hz - dim3);
                        bboxi = x;
                    }
                }
            }
        }
        #endregion
        #region Report_results
        public void Report_results(ref SolutionArray solArray)
        {
            pallet.Variant = best_variant;
            double packed_box_percentage = (double)best_solution_volume * 100 / (double)total_box_volume;
            pallet_volume_used_percentage = (double)best_solution_volume * 100 / (double)pallet.Vol;
            double elapsed_time = (TimeStop - TimeStart).TotalMilliseconds * 0.001;

            List_candidate_layers(best_variant);
            packedvolume = 0;
            packedy = 0;
            packing = true;
            layerThickness = layers[best_iteration].LayerDim;
            remainpy = pallet.Pallet_y;
            remainpz = pallet.Pallet_z;

            foreach (BoxInfo bi in boxList)
                bi.Is_packed = false;

            bool hundredpercent = false;
            do
            {
                layerinlayer = 0;
                layerdone = false;
                Pack_layer(best_variant, true, ref hundredpercent);
                packedy += layerThickness;
                remainpy = pallet.Pallet_y - packedy;
                if (0 != layerinlayer)
                {
                    prepackedy = packedy;
                    preremainpy = remainpy;
                    remainpy = layerThickness - prelayer;
                    packedy = packedy - layerThickness + prelayer;
                    remainpz = lilz;
                    layerThickness = layerinlayer;
                    layerdone = false;
                    Pack_layer(best_variant, true, ref hundredpercent);
                    packedy = prepackedy;
                    remainpy = preremainpy;
                    remainpz = pallet.Pallet_z;
                }
                System.Diagnostics.Debug.Assert(remainpy >= 0);
                Find_layer(remainpy, pallet);
            }
            while (packing);


            Solution sol = new Solution() { Variant = best_variant, Iteration = best_iteration };
            foreach (BoxInfo bi in boxList)
            {
                if (bi.Is_packed)
                    sol.ItemsPacked.Add(bi.ToSolItem(best_variant));
                else
                    sol.ItemsUnpacked.Add(bi.ToSolItem(best_variant));
            }
            solArray.Solutions.Add(sol);
            /*
            foreach (BoxInfo bi in boxList)
            {
                if (bi.Is_packed)
                    bi.WriteToFile(fso, best_variant, best_iteration);
            }
            */
            if (Debug)
            {
                Console.WriteLine($"ELAPSED TIME                       : Almost {elapsed_time:0.000} s");
                Console.WriteLine($"TOTAL NUMBER OF ITERATIONS DONE    : {number_of_iterations}");
                Console.WriteLine($"BEST SOLUTION FOUND AT             : ITERATION: {best_iteration} OF VARIANT: {best_variant}");
                Console.WriteLine($"TOTAL NUMBER OF BOXES              : {boxList.Count}");
                Console.WriteLine($"PACKED NUMBER OF BOXES             : {number_packed_boxes}");
                Console.WriteLine($"TOTAL VOLUME OF ALL BOXES          : {total_box_volume}");
                Console.WriteLine($"PALLET VOLUME                      : {pallet.Vol}");
                Console.WriteLine($"BEST SOLUTION'S VOLUME UTILIZATION : {best_solution_volume} OUT OF {pallet.Vol}");
                Console.WriteLine($"PERCENTAGE OF PALLET VOLUME USED   : {pallet_volume_used_percentage:0.000}");
                Console.WriteLine($"PERCENTAGE OF PACKEDBOXES (VOLUME) : {packed_box_percentage:0.000}");
                Console.WriteLine($"WHILE PALLET ORIENTATION           : X={pallet.Pallet_x}; Y={pallet.Pallet_y}; Z= {pallet.Pallet_z}");
                Console.WriteLine();
                foreach (var variant in best_iterations.Keys)
                    Console.WriteLine($"{variant} {best_iterations[variant].Key} -> {best_iterations[variant].Value}");
            }
        }

        public void Report_results2(ref SolutionArray solArray)
        {
            foreach (var variant in best_iterations.Keys)
            {
                pallet.Variant = variant;
                double packed_box_percentage = (double)best_iterations[variant].Value * 100 / (double)total_box_volume;
                pallet_volume_used_percentage = (double)best_iterations[variant].Value * 100 / (double)pallet.Vol;
                double elapsed_time = (TimeStop - TimeStart).TotalMilliseconds * 0.001;

                List_candidate_layers(variant);
                packedvolume = 0;
                packedy = 0;
                packing = true;
                layerThickness = layers[best_iterations[variant].Key].LayerDim;
                remainpy = pallet.Pallet_y;
                remainpz = pallet.Pallet_z;

                foreach (BoxInfo bi in boxList)
                    bi.Is_packed = false;

                bool hundredpercent = false;
                do
                {
                    layerinlayer = 0;
                    layerdone = false;
                    Pack_layer(variant, true, ref hundredpercent);
                    packedy += layerThickness;
                    remainpy = pallet.Pallet_y - packedy;
                    if (0 != layerinlayer)
                    {
                        prepackedy = packedy;
                        preremainpy = remainpy;
                        remainpy = layerThickness - prelayer;
                        packedy = packedy - layerThickness + prelayer;
                        remainpz = lilz;
                        layerThickness = layerinlayer;
                        layerdone = false;
                        Pack_layer(variant, true, ref hundredpercent);
                        packedy = prepackedy;
                        remainpy = preremainpy;
                        remainpz = pallet.Pallet_z;
                    }
                    System.Diagnostics.Debug.Assert(remainpy >= 0);
                    Find_layer(remainpy, pallet);
                }
                while (packing);


                Solution sol = new Solution() { Variant = variant, Iteration = best_iterations[variant].Key };
                foreach (BoxInfo bi in boxList)
                {
                    if (bi.Is_packed)
                        sol.ItemsPacked.Add(bi.ToSolItem(variant));
                    else
                        sol.ItemsUnpacked.Add(bi.ToSolItem(variant));
                }
                solArray.Solutions.Add(sol);
                if (Debug)
                {
                    Console.WriteLine($"ELAPSED TIME                       : Almost {elapsed_time:0.000} s");
                    Console.WriteLine($"TOTAL NUMBER OF ITERATIONS DONE    : {number_of_iterations}");
                    Console.WriteLine($"BEST SOLUTION FOUND AT             : ITERATION: {best_iteration} OF VARIANT: {best_variant}");
                    Console.WriteLine($"TOTAL NUMBER OF BOXES              : {boxList.Count}");
                    Console.WriteLine($"PACKED NUMBER OF BOXES             : {number_packed_boxes}");
                    Console.WriteLine($"TOTAL VOLUME OF ALL BOXES          : {total_box_volume}");
                    Console.WriteLine($"PALLET VOLUME                      : {pallet.Vol}");
                    Console.WriteLine($"BEST SOLUTION'S VOLUME UTILIZATION : {best_solution_volume} OUT OF {pallet.Vol}");
                    Console.WriteLine($"PERCENTAGE OF PALLET VOLUME USED   : {pallet_volume_used_percentage:0.000}");
                    Console.WriteLine($"PERCENTAGE OF PACKEDBOXES (VOLUME) : {packed_box_percentage:0.000}");
                    Console.WriteLine($"WHILE PALLET ORIENTATION           : X={pallet.Pallet_x}; Y={pallet.Pallet_y}; Z= {pallet.Pallet_z}");
                    Console.WriteLine();
                }
            }
        }
        #endregion
    }
}
