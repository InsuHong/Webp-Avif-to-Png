using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;

namespace image_convert
{
    public partial class Form1 : Form
    {

        static System.Object lockThis = new System.Object();
        String now_dir;
        public static MagickImage mimg;
        public Form1()
        {
            InitializeComponent();
        }

        public void read_dir(String dir_path)
        {
            String FolderName = dir_path;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FolderName);

            //파일처리
            String f_path = "", sub_dirname;
            if (di.Exists)
            {
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    f_path = FolderName + "\\" + File.Name;
                    FileInfo file = new FileInfo(f_path);
                    if (file.Exists)
                    {
                        String f_ext = System.IO.Path.GetExtension(File.Name).Replace(".", "").ToLower();
                        if (f_ext == "avif" || f_ext == "webp")
                        {
                            convert_image(f_path);
                        }

                    }
                    


                }
                //디렉토리 처리
                di = new System.IO.DirectoryInfo(FolderName);
                System.IO.DirectoryInfo sub_di;
                foreach (System.IO.DirectoryInfo Dirs in di.GetDirectories())
                {
                    sub_dirname = FolderName + "\\" + Dirs.Name;
                    sub_di = new System.IO.DirectoryInfo(sub_dirname);
                    if (sub_di.Exists)
                    {
                        read_dir(sub_dirname);
                    }

                }


            }
            else
            {
            }

        }

        public void del_all(String dir_path)
        {
            String FolderName = dir_path;
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(FolderName);

            //파일처리
            String f_path = "", sub_dirname;
            if (di.Exists)
            {
                foreach (System.IO.FileInfo File in di.GetFiles())
                {
                    f_path = FolderName + "\\" + File.Name;
                    FileInfo file = new FileInfo(f_path);
                    if (file.Exists)
                    {
                        String f_ext = System.IO.Path.GetExtension(File.Name).Replace(".", "").ToLower();
                        if (f_ext == "avif" || f_ext == "webp")
                        {
                            del_image(f_path);
                        }
                    }



                }
                //디렉토리 처리
                di = new System.IO.DirectoryInfo(FolderName);
                System.IO.DirectoryInfo sub_di;
                foreach (System.IO.DirectoryInfo Dirs in di.GetDirectories())
                {
                    sub_dirname = FolderName + "\\" + Dirs.Name;
                    sub_di = new System.IO.DirectoryInfo(sub_dirname);
                    if (sub_di.Exists)
                    {
                        del_all(sub_dirname);
                    }

                }


            }
            else
            {
            }






        }




        static void convert_image(String image_path)
        {


            String f_ext = System.IO.Path.GetExtension(image_path).Replace(".", "").ToLower();
            if (f_ext == "avif" || f_ext == "webp")
            {
                Thread myThread1 = new Thread(new ParameterizedThreadStart(Thread_convert));
                myThread1.Start(image_path);
                myThread1.Join();  //쓰레드 종료시까지 기다림
                mimg.Dispose();

            }
        }

        static void del_image(String image_path)
        {


            String f_ext = System.IO.Path.GetExtension(image_path).Replace(".", "").ToLower();
            if (f_ext == "avif" || f_ext == "webp")
            {

                try
                {
                    FileInfo file = new FileInfo(image_path);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                catch (IOException exception)
                {
                }
            }
        }




        static void Thread_convert(object i_path)
        {
                String image_path = Convert.ToString(i_path);
                String file_dir = Path.GetDirectoryName(image_path);
                String file_name = Path.GetFileNameWithoutExtension(image_path);


            mimg = new MagickImage(image_path);
            mimg.Write(file_dir + @"\" + file_name + ".png");
            


        }




        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.AllowDrop = true;
            pictureBox2.AllowDrop = true;
        }


        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                        String temp_txt = label1.Text;
            label1.Text = "Converting image";
            String file_dir = Path.GetDirectoryName(files[0]);
            String file_name = Path.GetFileNameWithoutExtension(files[0]);
            String f_ext = System.IO.Path.GetExtension(files[0]).Replace(".", "").ToLower();
            if (f_ext == "avif" || f_ext == "webp")
            {
                MagickImage mimg = new MagickImage(files[0]);
                pictureBox1.Image = mimg.ToBitmap();
                mimg.Dispose();
            }

            read_dir(file_dir);
            now_dir = file_dir;
            label1.Text = temp_txt;

        }
        private void pictureBox2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }



        private void pictureBox2_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            String file_dir = Path.GetDirectoryName(files[0]);
            del_all(file_dir);

            
        }






        //이미지 파일 메모리에 적재(사용중인 파일 문제 해결)
        public static Bitmap LoadBitmap(string path)
        {
            if (File.Exists(path))
            {
                // open file in read only mode
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                // get a binary reader for the file stream
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // copy the content of the file into a memory stream
                    var memoryStream = new MemoryStream(reader.ReadBytes((int)stream.Length));
                    // make a new Bitmap object the owner of the MemoryStream
                    return new Bitmap(memoryStream);
                }
            }
            else
            {
                //        MessageBox.Show("Error Loading File.", "Error!", MessageBoxButtons.OK);
                return null;
            }
        }



        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);

            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            return DateTime.Now;
        }



        //EOF



    }
}
