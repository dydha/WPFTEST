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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using WPFTEST.DTOs;
using WPFTEST.Exceptions;
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
        private System.Drawing.Bitmap? capturedBitmap; // Déclarer une variable pour stocker l'image capturée
        private List<VariantDTO> scannedProducts = new();

        public  MainWindow()
        {
            InitializeComponent();
           
                      videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            barcodeReader = new ZXing.Windows.Compatibility.BarcodeReader();
        }
        //----------------------------ADD TO CARD-------------------------------------
        private  void OnAddToCardClicked(object sender, RoutedEventArgs e)
        {
           
           if (videoDevices != null && videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(AddToCard);
                videoSource.Start();
         
            }
            
        }
        
      
        private async  void AddToCard(object sender, NewFrameEventArgs eventArgs)
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
              
               
                var  result = barcodeReader.Decode(capturedBitmap);
                if(result != null) 
                {
                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();

                    }
                    try
                    {
                        var product = await VariantsService.Instance.GetVariantAsync(result.Text);
                       
                            Dispatcher.Invoke(() => BarcodeTextBox.Text = product.Name);
                            scannedProducts.Add(product);
                            UpdateProductList();
                       
                       
                    }
                    catch(NotFoundException)
                    {
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = "Le produit n'a pas été trouvé.");

                    }
                    catch (Exception)
                    {
                        Dispatcher.Invoke(() => BarcodeTextBox.Text = "Une erreur est survenue.");

                    }
                }
              


            }
            catch (Exception)
            {
                Dispatcher.Invoke(() => BarcodeTextBox.Text = " Une erreur est survenue.");
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();

                }
            }
           
           
        }
        //---------------------------- END ADD TO CARD-------------------------------------
        //---------------------------- DELETE FROM CARD-------------------------------------
        private void OnDeleteFromCardClicked(object sender, RoutedEventArgs e)
        {

            if (videoDevices != null && videoDevices.Count > 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += new NewFrameEventHandler(DeleteFromCard);
                videoSource.Start();

            }

        }


        private  void DeleteFromCard(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                // Capturez une image à partir de la webcam.
                capturedBitmap = (System.Drawing.Bitmap)eventArgs.Frame.Clone();
                DecodingOptions readOptions = new()
                {
                    PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.EAN_13 },
                    TryHarder = true
                };
                barcodeReader = new()
                {
                    Options = readOptions,
                };

                // Utilisez le lecteur de code-barres pour tenter de lire le code-barre.
                var result = barcodeReader.Decode(capturedBitmap);
                if (result != null)
                {
                    if (videoSource != null && videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();

                    }
                   
                        var product = scannedProducts.Where(p => p.EanNumber == result.Text).FirstOrDefault();
                        if (product != null)
                        {
                            Dispatcher.Invoke(() => BarcodeTextBox.Text = "Produit supprimé.");
                            scannedProducts.Remove(product);
                            UpdateProductList();
                        }
                        else
                        {
                            Dispatcher.Invoke(() => BarcodeTextBox.Text = "Le produit n'a pas encore été ajouté au panier.");

                        }
                   
                }


            }
            catch (Exception)
            {
                Dispatcher.Invoke(() => BarcodeTextBox.Text = "Une erreur est survenue.");
                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.SignalToStop();

                }
            }


        }
        //---------------------------- END DELETE FROM CARD-------------------------------------

        private void UpdateProductList()
        {
            var groups = scannedProducts.ToLookup(v => v);
            var totalPrice = groups.Sum(group => group.Key.Price * group.ToArray().Length);
            var articles = groups.Select(group => $"{group.ToArray().Length}x {group.Key.Name} {group.Key.Price:C}");
            Dispatcher.Invoke(() => {
                BarcodeListView.ItemsSource = articles;
                TotalPriceTextBlock.Text = $"Total: {totalPrice:C}";
            });
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

