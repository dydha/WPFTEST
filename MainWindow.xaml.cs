using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFTEST.DTOs;
using WPFTEST.Services;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace WPFTEST
{
    public partial class MainWindow : Window
    {
        private FilterInfoCollection? videoDevices;
        private VideoCaptureDevice? videoSource;
        private ZXing.Windows.Compatibility.BarcodeReader barcodeReader;

        //  private Dictionary<VariantDTO, int> scannedProducts = new();
        private List<VariantDTO> scannedProducts = new();

        public  MainWindow()
        {
            InitializeComponent();
           
                      videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            barcodeReader = new ZXing.Windows.Compatibility.BarcodeReader();
        }

        private  void StartCameraButton_Click(object sender, RoutedEventArgs e)
        {
           
           if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);
                videoSource.Start();
            }
            
        }
        
        private System.Drawing.Bitmap? capturedBitmap; // Déclarer une variable pour stocker l'image capturée

        private async void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Capturez une image à partir de la webcam.
                capturedBitmap = (System.Drawing.Bitmap)eventArgs.Frame.Clone();
                DecodingOptions readOptions = new()
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.EAN_13},
                    TryHarder = true
                };
                barcodeReader = new()
                {
                    Options = readOptions,
                };
              
                // Utilisez le lecteur de code-barres pour tenter de lire le code-barre.
                var  result = barcodeReader.Decode(capturedBitmap);

               
                if (result != null)
                {
                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        
                    }
                    try
                    {
                        var product = await VariantsService.Instance.GetVariantAsync(result.Text);
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = result.Text);

                        if (product != null)
                        {
                            Dispatcher.Invoke(() => BarcodeTextBox.Text = product.Name);                          
                            scannedProducts.Add(product);
                            var groups = scannedProducts.ToLookup(v => v);                          
                            var totalPrice = groups.Sum(group => group.Key.Price * group.ToArray().Length);
                            var articles = groups.Select(group => $"{group.ToArray().Length}x {group.Key.Name} {group.Key.Price:C}");
                            Dispatcher.Invoke(() => {
                                BarcodeListView.ItemsSource = articles;
                                TotalPriceTextBlock.Text = $"Total: {totalPrice:C}";
                            });

                        }
                        else
                        {
                            Dispatcher.Invoke(() => BarcodeTextBox.Text = "produit non trouvé.");
                            if (videoSource != null && videoSource.IsRunning)
                            {
                                videoSource.SignalToStop();
                                videoSource.WaitForStop();
                            }
                        }
                    }
                    catch
                    {
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = "une erreur est survenue.");
                        videoSource.SignalToStop();
                    }
                }
                else
                {
                    // Aucun code-barre n'a été trouvé dans la photo.
                    Dispatcher.Invoke(() => BarcodeTextBox.Text = "Not found!");
                }
            }
            catch (Exception ex)
            {
                // Gérez les erreurs de capture ici.
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(DeleteVariantFromCard);
                videoSource.Start();
            }
        }
        private async void DeleteVariantFromCard(object sender, NewFrameEventArgs eventArgs)
        {
            
        }
        private void CapturePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            // Capturez une photo à partir de la webcam lorsque l'utilisateur le souhaite.
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            // Utilisez le lecteur de code-barres pour tenter de lire le code-barre à partir de la photo capturée.
            try
            {
                if (capturedBitmap != null)
                {
                    Result result = barcodeReader.Decode(capturedBitmap);

                    if (result != null)
                    {
                        // Un code-barre a été lu avec succès.
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = result.Text);
                    }
                    else
                    {
                        // Aucun code-barre n'a été trouvé dans la photo.
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = "Not found!");
                    }
                }
                else
                {
                    // Gérer le cas où l'image capturée est null (non capturée).
                }
            }
            catch (Exception ex)
            {
                // Gérez les erreurs de capture ici.
            }
        }


        private void StopCameraButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }
        }

        

        
    }
   
   

}

