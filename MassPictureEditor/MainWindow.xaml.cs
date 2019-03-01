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
        string sourceDirectory = "C:\\Users\\М\\Desktop\\log";
        string targetDirectory = "C:\\Users\\М\\Desktop\\копии фото";
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
                await Task.Delay(1000);

                string[] allFoundFiles = Directory.GetFiles(sourceDirectory, "*.jpg", SearchOption.TopDirectoryOnly);

                foreach (string path in allFoundFiles)
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

                    Bitmap resultImg = new Bitmap(img, new System.Drawing.Size(randomWidth, resultHeight));
                    string fileName = Guid.NewGuid() + ".jpg";
                    string resultPath = System.IO.Path.Combine(targetDirectory, fileName);
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
