using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace Projekat16011
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        private System.Drawing.Bitmap m_Bitmap;
        private System.Drawing.Bitmap m_Undo;
        public Form1()
        {
            InitializeComponent();

            m_Bitmap = new Bitmap(2, 2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog(); //Omogucava da izabere datoteku za otvaranje
            file.Filter = "Image File (*.bmp, *.jpg, *.png, *.gif, *.jpeg)| *.bmp; *.jpg; *.png; *.gif; *.jpeg"; //Filtirira tako da prikazuje samo slike sa ekstenzijama .jpg i .png
            if (DialogResult.OK == file.ShowDialog()) //Ispituje da li je korisnik izabrao sliku
            {
                // Ucitava sliku kao bitmapu
                Bitmap originalBmp = new Bitmap(file.FileName);

                // Kreira Bitmapu u 24b formatu
                this.OriginalImage = new Bitmap(originalBmp.Width, originalBmp.Height, PixelFormat.Format24bppRgb);

                using (Graphics gr = Graphics.FromImage(this.OriginalImage))
                {
                    gr.DrawImage(originalBmp, new Rectangle(0, 0, this.OriginalImage.Width, this.OriginalImage.Height));
                }
                this.picOriginal.Image = this.OriginalImage;
            }
        }

        private void picOriginal_Click(object sender, EventArgs e)
        {

        }

        //RGB->YUV
        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage); 

            Processing.ConvertRgbYuv(copy);
            this.ImageAfterEffect = copy;
            this.pic2.Image = this.ImageAfterEffect;

        }

        //Grey
        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap copy = new Bitmap((Bitmap) this.OriginalImage);

            Processing.ConvertToGrey(copy);
            this.ImageAfterEffect = copy;
            this.pic2.Image = this.ImageAfterEffect;
        }

        //Downsampling
        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap copy = new Bitmap(this.OriginalImage.Width, this.OriginalImage.Height, PixelFormat.Format24bppRgb);

            using (Graphics gr = Graphics.FromImage(copy))
            {
                gr.DrawImage(this.OriginalImage, new Rectangle(0, 0, copy.Width, copy.Height));
            }

            Processing.ConvertRgbYuvDOWNSAMPLING(copy);
           // Processing.TEST(copy);
            this.DownsampledImage = copy;
            this.ImageAfterEffect = DownsampledImage;
            this.pic2.Image = this.ImageAfterEffect;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //GAMMA
        private void GAMMA_Click(object sender, EventArgs e)
        {
            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);

            if (string.IsNullOrEmpty(textBox1.Text))
                MessageBox.Show("Niste uneli RED komponentu! ");
            if (string.IsNullOrEmpty(textBox2.Text))
                MessageBox.Show("Niste uneli BLUE komponentu! ");
            if (string.IsNullOrEmpty(textBox3.Text)) 
                MessageBox.Show("Niste uneli GREEN komponentu! ");

            double red = Convert.ToDouble(textBox1.Text);
            double blue = Convert.ToDouble(textBox2.Text);
            double green = Convert.ToDouble(textBox3.Text);

            Processing.FilterGamma(copy, red, blue, green);

            this.ImageAfterEffect = copy;
            this.pic2.Image = this.ImageAfterEffect;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        //SMOOTH
        private void button1_Click(object sender, EventArgs e)
        {
            int val = Convert.ToInt32(textBox4.Text);

            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);
            if (BitmapFilterConvolution.Smooth(copy, val))
            {
                this.ImageAfterEffect = copy;
                this.pic2.Image = this.ImageAfterEffect;
            }
          
        }

        //EDGE DETECT VERTICAL
        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);

            if (BitmapFilterConvolution.EdgeDetectVertical(copy))
            {
                this.ImageAfterEffect = copy;
                this.pic2.Image = this.ImageAfterEffect;
            }
                
        }

        //TIME WRAP NORMAL
        private void button3_Click(object sender, EventArgs e)
        {
            byte val = Convert.ToByte(textBox5.Text);

            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);
            if (Processing.TimeWarp(copy, val, false))
            {
                this.ImageAfterEffect = copy;
                this.pic2.Image = this.ImageAfterEffect;
            }
               
        }

        //TIME WRAP anti aliasing
        private void button10_Click(object sender, EventArgs e)
        {
            byte val = Convert.ToByte(textBox6.Text);

            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);
            if (Processing.TimeWarp(copy, val, true))
            {
                this.ImageAfterEffect = copy;
                this.pic2.Image = this.ImageAfterEffect;
            }
                

        }

        private void button7_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JPEG files(*.jpeg) | *.jpeg";
            //sfd.RestoreDirectory = true;

            if (DialogResult.OK == sfd.ShowDialog())
            {
                this.ImageAfterEffect.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {

            if (!this.ukljucen) {
                button12.BackColor = Color.Green;
                button12.Text = "MOD-UKLJUCEN";
                this.ukljucen = true;
                this.temp = this.OriginalImage;
                this.OriginalImage = this.DownsampledImage;
            }
            else
            {
                button12.BackColor = Color.Red;
                button12.Text = "MOD-ISKLJUCEN";
                this.ukljucen = false;
                this.OriginalImage = this.temp;
            }      
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string compressedOutputPath = Path.Combine(desktopPath, "compressed_output.jpg");

            Bitmap copy = new Bitmap((Bitmap)this.OriginalImage);

            HuffmanEncoder.CompressBitmap(copy, compressedOutputPath);

            MessageBox.Show("Slika je uspeno kompresovana i zapamcena na Desktop-u!! ");
            Console.WriteLine("Bitmap image compressed successfully.");
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
