namespace Netahsilat.DIAService
{
    public static class DiaEndPoints
    {
        public static class Suffixes
        {
            public const string POST = "_ekle";
            public const string LIST = "_listele";
            public const string GET = "_getir"; 
            public const string DETAILED_LIST = "_listele_ayrintili";
        }
        public static class Keys
        {
            public const string BANKACCOUNT = "bcs_bankahesabi";
            public const string CURRENTACCOUNTFICHE = "scf_carihesap_fisi";
            public const string CURRENTACCOUNT = "scf_carikart";
            public const string PAYPLAN = "scf_odeme_plani";
            public const string SALESMAN = "scf_satiselemani";
            public const string CURRENCY = "sis_doviz";
            public const string SPECODE = "sis_ozelkod";
            public const string VOUCHERNUMBER = "sis_numara";

            public const string PROJECTCODE = "prj_proje";
            public const string AUTHKEY = "sis_seviyekodu";
            public const string EXPENCECENTER = "muh_masrafmerkezi";
            public const string DIAPARAMETER = "sis_parametreler";
            public const string TOPTRANSACTIONTYPE = "sis_ust_islem_turu";
            public const string COMPANY = "sis_firma";
            public const string EXCHANGERATES = "sis_doviz_kuru";
            public const string BANKPAYBACKPLAN = "scf_banka_odeme_plani";
        }
    }
}
