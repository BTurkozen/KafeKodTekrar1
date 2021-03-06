﻿using KafeKod.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KafeKodTekrar1
{
    
    public partial class SiparisForm : Form
    {
        
        public event EventHandler<MasaTasimaEventArgs> MasaTasiniyor;
        KafeVeri db;
        Siparis siparis;
        BindingList<SiparisDetay> blSiparisdetaylari;
        public SiparisForm(KafeVeri kafeveri, Siparis _siparis)
        {
            db = kafeveri;
            this.siparis = _siparis;
            blSiparisdetaylari = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            MasaNolariYukle();
            MasaNoGuncelle();
            TutarGuncelle();
            cboUrun.DataSource = db.Urunler;
            //cboUrun.SelectedItem = null;
            dgvSiparisDetaylari.DataSource = blSiparisdetaylari;
        }

        #region TutarGuncelle, MasaNolariYukle, MasaNoGuncelle Metotları
        private void TutarGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void MasaNolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i < db.MasaAdet; i++)
            {
                if (!db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void MasaNoGuncelle()
        {
            Text = "Masa " + siparis.MasaNo;
            lblMasaNo.Text = siparis.MasaNo.ToString("00");
        }
        #endregion

        private void btnSiparisEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedItem == null)
            {
                MessageBox.Show("Lütfen Bir Ürün Şeçiniz!!!");
                return;
            }
            Urun seciliUrun = (Urun)cboUrun.SelectedItem;

            var sd = new SiparisDetay
            {
                UrunAd = seciliUrun.UrunAd,
                BirimFiyat = seciliUrun.BirimFiyat,
                Adet = (int)nudAdet.Value
        };
            blSiparisdetaylari.Add(sd);
            cboUrun.SelectedIndex = 0;
            nudAdet.Value = 1;
            TutarGuncelle();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Sipariş iptal edilecektir. Onaylıyor musunuz?",
                "Sipariş İptal Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Iptal;
                siparis.KapanisZamani = DateTime.Now;
                Close();
            }
        }

        private void BtnOdemeAl_Click(object sender, EventArgs e)
        {
            var dr = MessageBox.Show(
                "Ödeme alındıysa masanın hesabı kapatılacaktır. Onaylıyor musunuz?",
                "Masa Kapatma Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (dr == DialogResult.Yes)
            {
                siparis.Durum = SiparisDurum.Odendi;
                siparis.KapanisZamani = DateTime.Now;
                siparis.OdenenTutar = siparis.ToplamTutar();
                Close();
            }
        }

        private void dgvSiparisDetaylari_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowIndex = dgvSiparisDetaylari.HitTest(e.X, e.Y).RowIndex;
                if (rowIndex > -1)
                {
                    dgvSiparisDetaylari.ClearSelection();
                    dgvSiparisDetaylari.Rows[rowIndex].Selected = true;
                    cmsSiparisDetaylari.Show(MousePosition);
                }
            }
        }

        private void tsmiSiparisDetaySil_Click(object sender, EventArgs e)
        {
            if (dgvSiparisDetaylari.SelectedRows.Count > 0)
            {
                var seciliSatir = dgvSiparisDetaylari.SelectedRows[0];
                var sipdetay = (SiparisDetay)seciliSatir.DataBoundItem;
                blSiparisdetaylari.Remove(sipdetay);
            }
            TutarGuncelle();
        }

        private void BtnSiparisMasa_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedItem == null)
            {
                MessageBox.Show("Lütfen Hedef Masa No'yu Seçiniz.");
                return;
            }
            int eskiMasaNo = siparis.MasaNo;
            int hedefMasaNo = (int)cboMasaNo.SelectedItem;
            if (MasaTasiniyor != null)
            {
                var args = new MasaTasimaEventArgs
                {
                    TasinanSiparis = siparis,
                    EskiMasaNo = eskiMasaNo,
                    YeniMasaNo = hedefMasaNo
                };
                MasaTasiniyor(this, args);
            }
            siparis.MasaNo = hedefMasaNo;
            MasaNoGuncelle();
            MasaNolariYukle();
        }
    }

    public class MasaTasimaEventArgs: EventArgs
    {
        public Siparis TasinanSiparis { get; set; }
        public int EskiMasaNo { get; set; }
        public int YeniMasaNo { get; set; }
    }
}
