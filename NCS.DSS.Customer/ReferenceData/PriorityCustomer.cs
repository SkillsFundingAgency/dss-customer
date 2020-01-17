using System.ComponentModel;

namespace NCS.DSS.Customer.ReferenceData
{
    public enum PriorityCustomer
    {

        [Description("18 to 24 not in education, employment or training")]
        EighteenToTwentyfourNotInEducationEmploymentOrTraining = 1,

        [Description("Low skilled adults without a level 2 qualification")]
        LowSkilledAdultsWithoutALevel2Qualification = 2,

        [Description("Adults who have been unemployed for more than 12 months")]
        AdultsWhoHaveBeenUnemployedForMoreThan12Months = 3,

        [Description("Single parents with at least one dependant child living in the same household")]
        SingleParentsWithAtLeastOneDependantChildLivingInTheSameHousehold = 4,

        [Description("Adults with special educational needs and/or disabilities")]
        AdultsWithSpecialEducationalNeedsAndOrDisabilities = 5,

        [Description("Adults aged 50 years or over who are unemployed or at demonstrable risk of unemployment")]
        AdultsAged50YearsOrOverWhoAreUnemployedOrAtDemonstrableRiskOfUnemployment = 6,

        [Description("Not a priority customer")]
        NotAPriorityCustomer = 99
    }
}
