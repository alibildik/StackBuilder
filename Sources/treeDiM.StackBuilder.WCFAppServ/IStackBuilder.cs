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
            , DCCompFormat expectedFormat);
        [OperationContract]
        DCSBSolution SB_GetBundlePalletBestSolution(
            DCSBBundle sbBundle, DCSBPallet sbPallet, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat);
        [OperationContract]
        DCSBSolution SB_GetBundleCaseBestSolution(
            DCSBBundle sbBundle, DCSBCase sbCase
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat);
        [OperationContract]
        DCSBSolution SB_GetBoxCaseBestSolution(
            DCSBCase sbBox, DCSBCase sbCase, DCSBInterlayer sbInterlayer
            , DCSBConstraintSet sbConstraintSet
            , DCCompFormat expectedFormat);
        #endregion

        #region JJA Specific methods
        [OperationContract]
        DCSBCaseConfig[] JJA_GetCaseConfigs(DCSBDim3D dimensions, double weight, int pcb, DCCompFormat imageFormat);
        [OperationContract]
        DCSBLoadResultContainer[] JJA_GetMultiContainerResults(DCSBDim3D dimensions, double weight, int noItemPerCase
            , DCSBContainer[] containers);
        [OperationContract]
        DCSBLoadResultPallet[] JJA_GetMultiPalletResults(DCSBDim3D dimensions, double weight, int noItemPerCase
            , DCSBPalletWHeight[] pallets);
        [OperationContract]
        DCSBLoadResultSingleContainer JJA_GetLoadResultSingleContainer(DCSBDim3D dimensions, double weight, int noItemPerCase
            , DCSBContainer container, DCSBOrientation orientation, DCCompFormat imageFormat);
        [OperationContract]
        DCSBLoadResultSinglePallet JJA_GetLoadResultSinglePallet(DCSBDim3D dimensions, double weight, int noItemPerCase
            , DCSBPalletWHeight pallet, DCSBOrientation orientation, DCCompFormat imageFormat);
        #endregion

        #region Heterogeneous stacking
        [OperationContract]
        DCSBHSolution SB_GetHSolutionBestCasePallet(DCSBContentItem[] sbConstentItems
            , DCSBPallet sbPallet
            , DCSBHConstraintSet sbConstraintSet 
            , DCCompFormat expectedFormat);
        [OperationContract]
        DCSBHSolutionItem SB_GetHSolutionPart(
            DCSBContentItem[] sbContentItems
            , DCSBPallet sbPallet, DCSBHConstraintSet sbConstraintSet
            , int solIndex, int binIndex
            , DCCompFormat expectedFormat
            );
        #endregion
    }
}
