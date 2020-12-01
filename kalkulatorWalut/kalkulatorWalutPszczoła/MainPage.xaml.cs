using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Xml.Linq;
using Windows.Web.Http;
using Windows.Storage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace kalkulatorWalutPszczoła
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        const string daneNBP = "http://www.nbp.pl/kursy/xml/LastA.xml"; //tu adres pliku z danymi kursowymi
        List<PozycjaTabeliA> kursyAktualne = new List<PozycjaTabeliA>();

        
        public MainPage()
        {
            this.InitializeComponent();
            txtKwota.PlaceholderText = $"{0:f2}";
            tbPrzeliczona.Text = $"{0:f2}";

        }
    
        private async void NBPLoaded(object sender, RoutedEventArgs e)
        {
            bool a = ApplicationData.Current.LocalSettings.Values.ContainsKey("lbxNaWalute");
            bool b = ApplicationData.Current.LocalSettings.Values.ContainsKey("lbxZWaluty");

            var serwerNBP = new HttpClient();
            string dane = "";
            try
            {
                dane = await serwerNBP.GetStringAsync(new Uri(daneNBP));
                kursyAktualne.Clear();

                XDocument daneKursowe = XDocument.Parse(dane);

                kursyAktualne = (from item in daneKursowe.Descendants("pozycja")
                                 select new PozycjaTabeliA
                                 {
                                     przelicznik = item.Element("przelicznik").Value,
                                     kod_waluty = item.Element("kod_waluty").Value,
                                     kurs_sredni = item.Element("kurs_sredni").Value
                                 }
                    ).ToList();
                kursyAktualne.Insert(0, new PozycjaTabeliA() { kurs_sredni = "1,0000", kod_waluty = "PLN", przelicznik = "1" });
                lbxZWaluty.ItemsSource = kursyAktualne;
                lbxNaWalute.ItemsSource = kursyAktualne;

                if (a && b)
                {
                    object value1 = ApplicationData.Current.LocalSettings.Values["lbxZWaluty"];
                    object value2 = ApplicationData.Current.LocalSettings.Values["lbxNaWalute"];
                    lbxNaWalute.SelectedIndex = Int32.Parse(value2.ToString());
                    lbxZWaluty.SelectedIndex = Int32.Parse(value1.ToString());
                    
                }
                else
                {
                    lbxNaWalute.SelectedIndex = 0;
                    lbxZWaluty.SelectedIndex = 0;
                }
            }
            catch (Exception exception)
            {
                await new MessageDialog(exception.Message).ShowAsync();
            }
           

        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            Convert();
        }
        private void Convert()
        {

            double kwota;
            string waluta = "";
            if (double.TryParse(txtKwota.Text, out kwota))
            {
                kwota = ((PozycjaTabeliA)lbxZWaluty.SelectedItem).liczNaPLN(kwota, true);
                kwota = ((PozycjaTabeliA)lbxNaWalute.SelectedItem).liczNaPLN(kwota, false);
                waluta = ((PozycjaTabeliA)lbxNaWalute.SelectedItem).kod_waluty;
            }
            tbPrzeliczona.Text = $"{kwota:f2} {waluta}";

          

        }

        private void waluty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Convert();
            ApplicationData.Current.LocalSettings.Values["lbxZWaluty"] = lbxZWaluty.SelectedIndex;
            ApplicationData.Current.LocalSettings.Values["lbxNaWalute"] = lbxNaWalute.SelectedIndex;
        }

        private void btnOProgramie_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OProgramie));
        }

        private void btnPomoc_Click(object sender, RoutedEventArgs e)
        {
            
            this.Frame.Navigate(typeof(Pomoc), ((PozycjaTabeliA)lbxZWaluty.SelectedItem).kod_waluty);
        }
    }
}
