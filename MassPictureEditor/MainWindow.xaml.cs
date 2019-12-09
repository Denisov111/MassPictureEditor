using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace MassPictureEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string sourceDirectory = @"C:\Users\XXXXX\Desktop\log";
        string targetDirectory = @"C:\Users\XXXXX\Desktop\копии фото";
        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            sourceLabel.Content = sourceDirectory;
            targetLabel.Content = targetDirectory;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                sourceLabel.Content = sourceDirectory = dialog.SelectedPath;
            }
        }


        async private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(sourceDirectory))
            {
                //чистим папку
                string[] allFoundFiles_ = Directory.GetFiles(targetDirectory, "*.jpg", SearchOption.TopDirectoryOnly);
                foreach (string path in allFoundFiles_)
                {
                    File.Delete(path);
                }

                string[] allFoundDirs_ = Directory.GetDirectories(targetDirectory);
                foreach (string path in allFoundDirs_)
                {
                    allFoundFiles_ = Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly);
                    foreach (string path_ in allFoundFiles_)
                    {
                        File.Delete(path_);
                    }
                    Directory.Delete(path, true);
                }

                await Task.Delay(1000);

                string[] allFoundFiles = Directory.GetFiles(sourceDirectory, "*.jpg", SearchOption.TopDirectoryOnly);

                string[] allFoundFilesRecursive = Directory.GetFiles(sourceDirectory, "*.jpg", SearchOption.AllDirectories);

                foreach (string path in allFoundFilesRecursive)
                {
                    Bitmap img = new Bitmap(path);
                    int width = img.Width;
                    int height = img.Height;

                    firstImgImage.Source = BitmapToImageSource(img);
                    int maxWidth = 0;
                    int minWidth = 0;
                    //если изображение меньше 1400, то его можно только увеличить, иначе - только уменьшить
                    if (width < 1400)
                    {
                        //увеличиваем ширину в пределах 20%, но не менее чем на 3 пикселя
                        maxWidth = (int)(width * 1.2);
                        minWidth = width + 3;
                        minWidth = (minWidth > maxWidth) ? maxWidth : minWidth;
                    }
                    else
                    {
                        //уменьшаем ширину в пределах 20%, но не менее чем на 3 пикселя
                        maxWidth = 1350;
                        minWidth = 800;
                    }

                    int randomWidth = rnd.Next(minWidth, maxWidth);
                    int resultHeight = (randomWidth * height) / width;

                    statusLabel.Content = "Соотношение: " + (decimal)randomWidth / (decimal)width;

                    string fileName_ = System.IO.Path.GetFileName(path).Replace(".jpg", "");

                    Bitmap resultImg = new Bitmap(img, new System.Drawing.Size(randomWidth, resultHeight));
                    string oldName = path.Replace(sourceDirectory, "").Replace("\\", "").Replace(".jpg", "");
                    string oldNameOfDir = path.Replace(sourceDirectory, "").Replace(".jpg", "").Replace(fileName_, ""); ;

                    string fileName = "";
                    string resultPath = "";

                    if (fileName_== oldName)
                    {
                        fileName = oldName + "_" + Guid.NewGuid() + ".jpg";
                        resultPath = System.IO.Path.Combine(targetDirectory, fileName);
                    }
                    else
                    {
                        fileName = fileName_ + "_" + Guid.NewGuid() + ".jpg";
                        string subDirPath = targetDirectory + oldNameOfDir;
                        if(!Directory.Exists(subDirPath))
                        {
                            Directory.CreateDirectory(subDirPath);
                        }
                        resultPath = System.IO.Path.Combine(subDirPath, fileName);
                    }

                    resultImg.Save(resultPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    secondImgImage.Source = BitmapToImageSource(resultImg);
                    await Task.Delay(1);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Нет папки исходных файлов");
            }

            statusLabel.Content = "Готово!!!";
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                targetLabel.Content = targetDirectory = dialog.SelectedPath;
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
