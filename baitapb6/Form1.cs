using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace baitapb6
{
    public partial class Form1 : Form
    {
        String strcnn = "Server=TRI-LIEU\\SQLEXPRESS;Database=QLSV;Trusted_Connection=True;";
        BindingSource bs;
        DataSet ds;

        public Form1()
        {
            InitializeComponent();

            using (SqlConnection cnn = new SqlConnection(strcnn))
            {
                cnn.Open();
                SqlDataAdapter adt = new SqlDataAdapter("SELECT * FROM SinhVien", cnn);
                SqlCommandBuilder builder = new SqlCommandBuilder(adt);
                ds = new DataSet();
                adt.Fill(ds, "SinhVien");
                bs = new BindingSource(ds, "SinhVien");
                txtMaSV.DataBindings.Add("Text", bs, "MaSV");
                txtTenSV.DataBindings.Add("Text", bs, "TenSV");
                txtDiemSV.DataBindings.Add("Text", bs, "DiemSV");
                txtMaKhoa.DataBindings.Add("Text", bs, "MaKhoa");
            }
        }

        private void btnLuiHet_Click(object sender, EventArgs e)
        {
            if (bs != null && bs.Position > 0)
            {
                bs.MoveFirst();
            }
        }

        private void btnLui1Trang_Click(object sender, EventArgs e)
        {
            if (bs != null && bs.Position < bs.Count - 1)
            {
                bs.Position += 1;
            }
        }

        private void btnToi1Trang_Click(object sender, EventArgs e)
        {
            if (bs != null && bs.Position > 0)
            {
                bs.Position -= 1;
            }
        }

        private void btnToiHet_Click(object sender, EventArgs e)
        {
            if (bs != null && bs.Position < bs.Count - 1)
            {
                bs.MoveLast();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (bs == null)
            {
                bs = new BindingSource();
                ds = new DataSet();
                ds.Tables.Add("SinhVien");
                ds.Tables["SinhVien"].Columns.Add("MaSV");
                ds.Tables["SinhVien"].Columns.Add("TenSV");
                ds.Tables["SinhVien"].Columns.Add("DiemSV");
                ds.Tables["SinhVien"].Columns.Add("MaKhoa");
                ds.Tables["SinhVien"].PrimaryKey = new DataColumn[] { ds.Tables["SinhVien"].Columns["MaSV"] };
                bs.DataSource = ds.Tables["SinhVien"];
                txtMaSV.DataBindings.Add("Text", bs, "MaSV");
                txtTenSV.DataBindings.Add("Text", bs, "TenSV");
                txtDiemSV.DataBindings.Add("Text", bs, "DiemSV");
                txtMaKhoa.DataBindings.Add("Text", bs, "MaKhoa");
            }
            bs.AddNew();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (bs != null && bs.Position >= 0)
            {
                using (SqlConnection cnn = new SqlConnection(strcnn))
                {
                    cnn.Open();
                    SqlTransaction transaction = cnn.BeginTransaction();
                    try
                    {
                        // Delete related records from KetQua table
                        SqlCommand cmd = new SqlCommand("DELETE FROM KetQua WHERE MaSV = @MaSV", cnn, transaction);
                        cmd.Parameters.AddWithValue("@MaSV", txtMaSV.Text);
                        cmd.ExecuteNonQuery();

                        // Delete record from SinhVien table
                        bs.RemoveCurrent();
                        SqlDataAdapter adt = new SqlDataAdapter("SELECT * FROM SinhVien", cnn);
                        SqlCommandBuilder builder = new SqlCommandBuilder(adt);
                        adt.Update(ds.Tables["SinhVien"]);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (bs != null)
            {
                bs.EndEdit();
                using (SqlConnection cnn = new SqlConnection(strcnn))
                {
                    cnn.Open();

                    // Delete related records from KetQua table
                    SqlCommand cmd = new SqlCommand("DELETE FROM KetQua WHERE MaSV = @MaSV", cnn);
                    cmd.Parameters.AddWithValue("@MaSV", txtMaSV.Text);
                    cmd.ExecuteNonQuery();

                    SqlDataAdapter adt = new SqlDataAdapter("SELECT * FROM SinhVien", cnn);
                    SqlCommandBuilder builder = new SqlCommandBuilder(adt);
                    adt.Update(ds.Tables["SinhVien"]);
                }
            }
        }
    }
}