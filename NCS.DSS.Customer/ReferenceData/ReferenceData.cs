using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCS.DSS.Customer.Data
{
    public class ReferenceData
    {
        #region Constructor
        public ReferenceData()
        {
            #region ListEthnicity Data
            this.ListEthnicity = new List<Ethnicity>();
            ListEthnicity.Add(new Ethnicity { Key = 31, Value = "English / Welsh / Scottish / Northern Irish / British" });
            ListEthnicity.Add(new Ethnicity { Key = 32, Value = "Irish" });
            ListEthnicity.Add(new Ethnicity { Key = 33, Value = "Gypsy or Irish Traveller" });
            ListEthnicity.Add(new Ethnicity { Key = 34, Value = "Any Other White background" });
            ListEthnicity.Add(new Ethnicity { Key = 35, Value = "White and Black Caribbean" });
            ListEthnicity.Add(new Ethnicity { Key = 36, Value = "White and Black African" });
            ListEthnicity.Add(new Ethnicity { Key = 37, Value = "White and Asian" });
            ListEthnicity.Add(new Ethnicity { Key = 38, Value = "Any Other Mixed / multiple ethnic background" });
            ListEthnicity.Add(new Ethnicity { Key = 39, Value = "Indian" });
            ListEthnicity.Add(new Ethnicity { Key = 40, Value = "Pakistani" });
            ListEthnicity.Add(new Ethnicity { Key = 41, Value = "Bangladeshi" });
            ListEthnicity.Add(new Ethnicity { Key = 42, Value = "Chinese" });
            ListEthnicity.Add(new Ethnicity { Key = 43, Value = "Any other Asian background" });
            ListEthnicity.Add(new Ethnicity { Key = 44, Value = "African" });
            ListEthnicity.Add(new Ethnicity { Key = 45, Value = "Caribbean" });
            ListEthnicity.Add(new Ethnicity { Key = 46, Value = "Any other Black / African / Caribbean background" });
            ListEthnicity.Add(new Ethnicity { Key = 47, Value = "Arab" });
            ListEthnicity.Add(new Ethnicity { Key = 98, Value = "Any other ethnic group" });
            ListEthnicity.Add(new Ethnicity { Key = 99, Value = "Not provided" });
            #endregion

            #region Gender Data
            this.ListGender = new List<Gender>();
            ListGender.Add(new Gender { Key = 1, Value = "Female" });
            ListGender.Add(new Gender { Key = 2, Value = "Male" });
            ListGender.Add(new Gender { Key = 3, Value = "Not applicable" });
            ListGender.Add(new Gender { Key = 99, Value = "Not provided" });

            #endregion

            #region IntroducedBy Data
            this.ListIntroducedBy = new List<IntroducedBy>();
            ListIntroducedBy.Add(new IntroducedBy { Key = 1, Value = "TBD" });
            ListIntroducedBy.Add(new IntroducedBy { Key = 2, Value = "Not Provided" });
            #endregion
            
        }
        #endregion

        #region Lists
        public List<Ethnicity> ListEthnicity { get; set; }
        public List<Gender> ListGender { get; set; }
        public List<IntroducedBy> ListIntroducedBy { get; set; }
        #endregion

        #region Structs
        public struct Ethnicity { public int Key { get; set; }[StringLength(100)] public string Value { get; set; } }
        public struct Gender { public int Key { get; set; }[StringLength(100)] public string Value { get; set; } }
        public struct IntroducedBy { public int Key { get; set; }[StringLength(100)] public string Value { get; set; } }
        #endregion

    }

}
