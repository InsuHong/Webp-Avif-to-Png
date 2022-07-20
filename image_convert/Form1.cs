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

       // static System.Object lockThis = new System.Object();
        //String now_dir , image_path;
        //public static MagickImage mimg;
        
        
        public Form1()
        {
            InitializeComponent();



        }

        public void Change_Images(String dir_path)
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
                            BackgroundWorker Convert_Image = new BackgroundWorker();
                            Convert_Image.DoWork += new DoWorkEventHandler(Convert_Image_DoWork);
                            Convert_Image.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Convert_Image_RunWorkerCompleted);
                            Convert_Image.RunWorkerAsync(f_path);
                            //convert_image(f_path);
                            //backgroundWorker2.RunWorkerAsync(f_path); // 백그라운드에 등록
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
                        Change_Images(sub_dirname);
                    }

                }


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





        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.AllowDrop = true;
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
            label1.Text = "Converting image";


            foreach (String obj in files)
            {
                FileAttributes chkAtt = File.GetAttributes(obj);
                if ((chkAtt & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    // 디렉토리일 경우
                    if (System.IO.Directory.Exists(obj))
                    {
                        BackgroundWorker Convert_Dir;
                        Convert_Dir = new BackgroundWorker();
                        Convert_Dir.DoWork += new DoWorkEventHandler(Convert_Dir_DoWork);
                        Convert_Dir.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Convert_Dir_RunWorkerCompleted);
                        Convert_Dir.RunWorkerAsync(obj);
                    }
                }
                else
                {
                    // 파일 일 경우
                    if (System.IO.File.Exists(obj))
                    {
                        String file_name = Path.GetFileNameWithoutExtension(obj);
                        String f_ext = System.IO.Path.GetExtension(files[0]).Replace(".", "").ToLower();
                        if (f_ext == "avif" || f_ext == "webp")
                        {
                            BackgroundWorker Convert_Image;
                            Convert_Image = new BackgroundWorker();
                            Convert_Image.DoWork += new DoWorkEventHandler(Convert_Image_DoWork);
                            Convert_Image.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Convert_Image_RunWorkerCompleted);
                            Convert_Image.RunWorkerAsync(obj);
                        }
                    }
                }



                
            }

            /*
            if (f_ext == "avif" || f_ext == "webp")
            {
                backgroundWorker1.RunWorkerAsync(now_dir);
//                Thread myThread1 = new Thread(new ParameterizedThreadStart(Thread_Change));
//                myThread1.Start(now_dir);
//                myThread1.Join();  //쓰레드 종료시까지 기다림

            }
            */


        }



        private void Convert_Dir_DoWork(object sender, DoWorkEventArgs e)
        {
            String now_dir = e.Argument as String;
            if (System.IO.Directory.Exists(now_dir))
            {
                Change_Images(now_dir);
            }
            e.Result = now_dir;
        }

        private void Convert_Dir_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
            String now_dir = e.Result.ToString();
            if (System.IO.Directory.Exists(now_dir))
            {
              //  Del_Old_Images(now_dir);
            }
            label1.Text = "Converting complete";
        }



        private void Convert_Image_DoWork(object sender, DoWorkEventArgs e)
        {

            String image_path = e.Argument as String;
        //Debug.WriteLine("이미지경로 : " + image_path);
        String file_dir = Path.GetDirectoryName(image_path);
            String file_name = Path.GetFileNameWithoutExtension(image_path);
            FileInfo file = new FileInfo(image_path);
            if (file.Exists)
            {

                
                MagickImageCollection animatedWebP = new MagickImageCollection(image_path);
                if(animatedWebP.Count > 1)
                {
                    animatedWebP.Write(file_dir + @"\" + file_name + ".gif");
                }
                else
                {
                    MagickImage mimg = new MagickImage(image_path);
                    mimg.Write(file_dir + @"\" + file_name + ".png");
                }
                
            }
            e.Result = image_path;

        }

        private void Convert_Image_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            String image_path = e.Result.ToString();
            FileInfo file = new FileInfo(image_path);
            if (file.Exists)
            {
                if(checkBox1.Checked == true)
                {
                    del_image(image_path);
                }
                
            }
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
