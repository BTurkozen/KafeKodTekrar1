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
    public partial class GecmiSiparislerForm : Form
    {
        KafeContex db;
        public GecmiSiparislerForm(KafeContex _kafeVeri)
        {
            db = _kafeVeri;
            InitializeComponent();
            dgvSiparisler.AutoGenerateColumns = false;
            dgvSiparisDetaylari.AutoGenerateColumns = false;
            dgvSiparisler.DataSource = db.GecmisSiparisler;
        }

        private void dgvSiparisler_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvSiparisler.SelectedRows.Count > 0)
            {
                DataGridViewRow satir = dgvSiparisler.SelectedRows[0];
                Siparis siparis = (Siparis)satir.DataBoundItem;
                dgvSiparisDetaylari.DataSource = siparis.SiparisDetaylar;
            }
        }
    }
}
