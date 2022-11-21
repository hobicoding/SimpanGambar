using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpanGambar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files(*.jpg; *.jpeg; *.bmp; *.png; *.ico) | *.jpg; *.jpeg; *.bmp; *.png; *.ico";
            if(open.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image = new Bitmap(open.FileName);
                textBox.Text = open.FileName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(textBox.Text=="")
            {
                MessageBox.Show("Pilih gambar terlebih dahulu");
            }
            else
            {
                string name = textBox.Text;
                string[] strpath = name.Split(Convert.ToChar(@"."));

                MemoryStream ms = new MemoryStream();
                pictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                int fileLenght = Convert.ToInt32(ms.Length) / 1024;

                try
                {
                    Data.Connect();
                    Data.Command("INSERT INTO SimpanGambar(Name,Type,Size,Data,DateUpload) VALUES(@0,@1,@2,@3,GETDATE())",
                        new object[] { name, strpath[strpath.Length - 1], fileLenght, ms.ToArray() });

                    MessageBox.Show("Gambar berhasil disimpan");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(),"GAGAL");
                }
                finally
                {
                    Data.Disconnect();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                Data.Connect();
                DataTable dt = Data.SelectDataTable("SELECT Data FROM SimpanGambar", null);
                if(dt!=null)
                {
                    if(dt.Rows.Count>0)
                    {
                        Byte[] data = new byte[0];
                        data = (Byte[])(dt.Rows[0][0]);
                        MemoryStream ms = new MemoryStream(data);
                        pictureBox.Image = Image.FromStream(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "GAGAL");
            }
            finally
            {
                Data.Disconnect();
            }
        }
    }
}
