#region Using directives
using System.ServiceModel;
#endregion

namespace treeDiM.StackBuilder.WCFAppServ
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IStackBuilder
    {
        #region Homogeneous stacking
        [OperationContract]
        DCSBSolution SB_GetCasePalletBestSolution(
            DCSBCase sbCase, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat, bool showCotations);
        [OperationContract]
        DCSBSolution SB_GetBundlePalletBestSolution(
            DCSBBundle sbBundle, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat, bool showCotations);
        [OperationContract]
        DCSBSolution SB_GetBundleCaseBestSolution(
            DCSBBundle sbBundle, DCSBCase sbCase
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat, bool showCotations);
        [OperationContract]
        DCSBSolution SB_GetBoxCaseBestSolution(
            DCSBCase sbBox, DCSBCase sbCase, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat, bool showCotations);
        #endregion

        #region JJA Specific
        #endregion

        #region Heterogeneous stacking
        [OperationContract]
        DCSBHSolution SB_GetHSolutionBestCasePallet(DCSBContentItem[] sbConstentItems
            , DCSBPallet sbPallet
            , DCSBHConstraintSet sbConstraintSet 
            , DCCompFormat expectedFormat, bool showCotations);
        [OperationContract]
        DCSBHSolutionItem SB_GetHSolutionPart(
            DCSBContentItem[] sbContentItems, DCSBPallet sbPallet, DCSBHConstraintSet sbConstraintSet
            , int solIndex, int binIndex
            , DCCompFormat expectedFormat, bool showCotations
            );
        #endregion
    }
}
