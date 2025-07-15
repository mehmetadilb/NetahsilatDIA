using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Netahsilat.DIAService.Model
{
    public class Kart
    {
        public int _key_scf_malzeme_baglantisi { get; set; }
        public _Key_Scf_Odeme_Plani _key_scf_odeme_plani { get; set; }
        public int _key_sis_ozelkod { get; set; }
        public int _key_sis_seviyekodu { get; set; }
        public _Key_Sis_Sube _key_sis_sube { get; set; }
        public string aciklama1 { get; set; }
        public string aciklama2 { get; set; }
        public string aciklama3 { get; set; }
        public string belgeno { get; set; }
        public string fisno { get; set; }
        public M_Kalemler[] m_kalemler { get; set; }
        public string saat { get; set; }
        public string tarih { get; set; }
        public string turu { get; set; }
    }

    public class _Key_Scf_Odeme_Plani
    {
        public string kodu { get; set; }
    }

    public class _Key_Sis_Sube
    {
        public string subekodu { get; set; }
    }

    public class M_Kalemler
    {
        public int _key_bcs_bankahesabi { get; set; }
        public int _key_muh_masrafmerkezi { get; set; }
        public int _key_ote_rezervasyonkarti { get; set; }
        public int _key_prj_proje { get; set; }
        public int _key_scf_banka_odeme_plani { get; set; }
        public _Key_Scf_Carikart _key_scf_carikart { get; set; }
        public _Key_Scf_Odeme_Plani1 _key_scf_odeme_plani { get; set; }
        public int _key_scf_satiselemani { get; set; }
        public int _key_shy_servisformu { get; set; }
        public _Key_Sis_Doviz _key_sis_doviz { get; set; }
        public _Key_Sis_Doviz_Raporlama _key_sis_doviz_raporlama { get; set; }
        public int _key_sis_ozelkod { get; set; }
        public string aciklama { get; set; }
        public string alacak { get; set; }
        public string borc { get; set; }
        public string dovizkuru { get; set; }
        public string kurfarkialacak { get; set; }
        public string kurfarkiborc { get; set; }
        public string makbuzno { get; set; }
        public string carikart { get; set; }
        public string cariunvan { get; set; }
        public string raporlamadovizkuru { get; set; }
        public string vade { get; set; }
    }

    public class _Key_Scf_Carikart
    {
        public string carikartkodu { get; set; }
    }

    public class _Key_Scf_Odeme_Plani1
    {
        public string kodu { get; set; }
    }

    public class _Key_Sis_Doviz
    {
        public string adi { get; set; }
    }

    public class _Key_Sis_Doviz_Raporlama
    {
        public string adi { get; set; }
    }

}
