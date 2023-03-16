using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;
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

namespace WpfOsztalyzas
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string fajlNev = "naplo.txt";
        //Így minden metódus fogja tudni használni.
        List<Osztalyzat> jegyek = new List<Osztalyzat>();

        public MainWindow()
        {
            InitializeComponent();
            // todo Fájlok kitallózásával tegye lehetővé a naplófájl kiválasztását!
            // Ha nem választ ki semmit, akkor "naplo.csv" legyen az állomány neve. A későbbiekben ebbe fog rögzíteni a program.
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files(*.csv)| *.csv";
            openFileDialog.ShowDialog();
            fajlNev = openFileDialog.FileName;

            StreamReader sr = new StreamReader(fajlNev);
            while (!sr.EndOfStream)
            {
                string[] sor = sr.ReadLine().Split(";");
                jegyek.Add(new Osztalyzat(sor[0], sor[1], sor[2], Convert.ToInt32(sor[3])));
            }

            dgJegyek.ItemsSource = jegyek;
            Info();

            // todo A kiválasztott naplót egyből töltse be és a tartalmát jelenítse meg a datagrid-ben!
        }

        private void btnRogzit_Click(object sender, RoutedEventArgs e)
        {
            //todo Ne lehessen rögzíteni, ha a következők valamelyike nem teljesül!
            // a) - A név legalább két szóból álljon és szavanként minimum 3 karakterből!
            //      Szó = A szöközökkel határolt karaktersorozat.
            string[] vagottNev = txtNev.Text.Split(" ");
            if (vagottNev.Length == 1)
            {
                MessageBox.Show("Hibásan megadott név! A névnek két szóból kell állnia!");
                return;
            }
            foreach (string jelenlegi in vagottNev)
            {
                if (jelenlegi.Trim().Length < 3)
                {
                    MessageBox.Show("A név karaktereinek számának legalább 3-nak kell lennie!");
                    return;
                }
            }
            // b) - A beírt dátum újabb, mint a mai dátum

            //todo A rögzítés mindig az aktuálisan megnyitott naplófájlba történjen!


            //A CSV szerkezetű fájlba kerülő sor előállítása
            string csvSor = $"{txtNev.Text};{datDatum.Text};{cboTantargy.Text};{sliJegy.Value}";
            //Megnyitás hozzáfűzéses írása (APPEND)
            StreamWriter sw = new StreamWriter(fajlNev, append: true);
            sw.WriteLine(csvSor);
            sw.Close();
            //todo Az újonnan felvitt jegy is jelenjen meg a datagrid-ben!
            jegyek.Add(new Osztalyzat(txtNev.Text, datDatum.Text, cboTantargy.Text, Convert.ToInt32(sliJegy.Value)));
            dgJegyek.Items.Refresh();
            Info();
        }

        private void btnBetolt_Click(object sender, RoutedEventArgs e)
        {
            jegyek.Clear();  //A lista előző tartalmát töröljük
            StreamReader sr = new StreamReader(fajlNev); //olvasásra nyitja az állományt
            while (!sr.EndOfStream) //amíg nem ér a fájl végére
            {
                string[] mezok = sr.ReadLine().Split(";"); //A beolvasott sort feltördeli mezőkre
                //A mezők értékeit felhasználva létrehoz egy objektumot
                Osztalyzat ujJegy = new Osztalyzat(mezok[0], mezok[1], mezok[2], int.Parse(mezok[3])); 
                jegyek.Add(ujJegy); //Az objektumot a lista végére helyezi
            }
            sr.Close(); //állomány lezárása

            //A Datagrid adatforrása a jegyek nevű lista lesz.
            //A lista objektumokat tartalmaz. Az objektumok lesznek a rács sorai.
            //Az objektum nyilvános tulajdonságai kerülnek be az oszlopokba.
            dgJegyek.ItemsSource = jegyek;
            Info();
        }

        private void sliJegy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            lblJegy.Content = sliJegy.Value; //Több alternatíva van e helyett! Legjobb a Data Binding!
        }

        private void Info()
        {
            int jegyekOsszege = 0;
            int jegyekSzama = 0;
            foreach (var item in jegyek)
            {
                jegyekOsszege += item.Jegy;
                jegyekSzama++;
            }
            lblInfo.Content = $"Fájl neve: {fajlNev}\nJegyek száma: {jegyekSzama} | Jegyek átlaga: {jegyekOsszege/jegyekSzama}";
        }

        private void Megjelenites(object sender, RoutedEventArgs e)
        {
            foreach (Osztalyzat jelenlegi in jegyek)
            {
                jelenlegi.ForditottNev();
            }
        }

        //todo Felület bővítése: Az XAML átszerkesztésével biztosítsa, hogy láthatóak legyenek a következők!
        // - A naplófájl neve
        // - A naplóban lévő jegyek száma
        // - Az átlag

        //todo Új elemek frissítése: Figyeljen rá, ha új jegyet rögzít, akkor frissítse a jegyek számát és az átlagot is!

        //todo Helyezzen el alkalmas helyre 2 rádiónyomógombot!
        //Feliratok: [■] Vezetéknév->Keresztnév [O] Keresztnév->Vezetéknév
        //A táblázatban a név azserint szerepeljen, amit a rádiónyomógomb mutat!
        //A feladat megoldásához használja fel a ForditottNev metódust!
        //Módosíthatja az osztályban a Nev property hozzáférhetőségét!
        //Megjegyzés: Felételezzük, hogy csak 2 tagú nevek vannak
    }
}

