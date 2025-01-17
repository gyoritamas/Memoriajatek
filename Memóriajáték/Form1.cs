﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;

namespace Memóriajáték
{
    public partial class Form1 : Form
    {
        // A Random objektum fogja kiválasztani az ikonokat a cellákba
        Random random = new Random();

        // Itt állítsuk be az ikonok színét
        Color defaultIconColor = Color.MidnightBlue;


        // Mindegyik betű egy érdekes ikon
        // a Webdings font készletében
        // és mindegyik ikon kétszer szerepel a listában
        List<string> icons = new List<string>()
        {
            "!", "!", "N", "N", ",", ",", "k", "k", "p", "p", "T", "T", "j", "j", "g", "g",
            "b", "b", "v", "v", "w", "w", "z", "z", "U", "U", "F", "F", "?", "?", "h", "h",
            "i", "i", "D", "D", "y", "y", "B", "B", "4", "4", "a", "a", "L", "L", "W", "W",
            "Z", "Z", ".", ".", "-", "-", "Q", "Q", "V", "V", "K", "K", "t", "t", "2", "2"
        };

        // firstClicked az elsőnek kiválasztott címkére fog mutatni 
        // amire a játékos klikkelt, de ennek értéke addig null 
        // amíg a játékos rá nem kattintott egy címkére
        Label firstClicked = null;

        // secondClicked a második címkére fog hivatkozni 
        // amire a játékos kattint
        Label secondClicked = null;

        /// <summary>
        /// Minden ikont hozzárendel egy véletlenszerű négyzethez
        /// </summary>

        private void AssignIconsToSquares()
        {
            // A TableLayoutPanel-nek 16 címkéje van,
            // és a listának 16 iknoja,
            // tehát véletlenszerűen mindegyik ikont hozzá kell adni
            // mindegyik címkéhez a nézetben
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;
                if (iconLabel != null)
                {
                    int randomNumber = random.Next(icons.Count);
                    iconLabel.Text = icons[randomNumber];
                    iconLabel.ForeColor = iconLabel.BackColor;
                    icons.RemoveAt(randomNumber);
                }
            }
        }


        private string SetSound(string eventWaitingForSound)
        {
            switch (eventWaitingForSound)
            {
                case "correct": return "correct.wav";
                case "incorrect": return "incorrect.wav";
                case "hide": return "hide.wav";
            }
            return null;
        }
        private void PlaySound(String file)
        {
            if (file != null)
            {
                SoundPlayer _soundPlayer = new SoundPlayer(@"Sounds\" + file);
                _soundPlayer.Play();
            }
        }

        public Form1()
        {
            InitializeComponent();

            AssignIconsToSquares();

        }

        public Stopwatch stopwatch = new Stopwatch();

        ///<summary>
        ///Minden címke Klikk eseményét kezeli
        ///</summary>
        ///<param name="sender">A címke, amire kattintottak</param>
        ///<param name="e"></param>
        private void label_Click(object sender, EventArgs e)
        {
            // Első kattintáskor elindítja a játékidőt mérő stoppert
            if (!timerGametime.Enabled)
            {
                timerGametime.Enabled = true;
                stopwatch = Stopwatch.StartNew();
            }

            // A stopper csak akkor működik, ha két nem egyező 
            // ikont jelenített meg a játékosnak,
            // addig minden klikkelést figyelmen kívül hagy
            if (timer1.Enabled == true)
                return;

            Label clickedLabel = sender as Label;
            if (clickedLabel != null)
            {
                // Ha a címke színe fekete, akkor a játékos már rákattinott
                // és az ikon is megjelent
                // így hagyja figyelmen kívül az utasítást
                if (clickedLabel.ForeColor == defaultIconColor)
                    return;

                // Ha a firstClicked értéke null, akkor ez az első ikon 
                // a párok közül, amit a játékos választott ki,
                // így a firstClicked a kiválasztott címkével lesz egyenlő 
                // a szöveg színe fekete lesz, majd visszatér
                if (firstClicked == null)
                {
                    timer2.Start();
                    firstClicked = clickedLabel;
                    firstClicked.ForeColor = defaultIconColor;

                    return;
                }

                // Amikor a játékos ide elér, a stopper nem számol
                // és a firstClicked értéke nem null,
                // tehát a második ikonra klikkelt a játékos
                // Beállítja a színét feketére
                secondClicked = clickedLabel;
                secondClicked.ForeColor = defaultIconColor;
                timer2.Stop();

                // Ellenőrzi, hogy a játékos nyert-e
                CheckForWinner();

                // Ha a játékos két egyező ikonra klikkel, akkor feketén maradnak
                // és visszaállítja a firstClicked és a secondClicked értékeit, 
                // hogy egy másik ikonra kattinthasson
                if (firstClicked.Text == secondClicked.Text)
                {
                    PlaySound(SetSound("correct"));
                    firstClicked = null;
                    secondClicked = null;
                    return;
                }

                // Ha a játékos ide elér,
                // akkor két különböző ikonra kattintott
                // a stopper időzítő elindul
                // majd elrejti az ikonokat
                PlaySound(SetSound("incorrect"));
                timer1.Start();

            }

        }

        /// <summary>
        /// Az stopper elindul, ha a játékos
        /// két ikonra klikkel, amik nem egyeznek meg,
        /// addig elszámol háromnegyed másodpercig,
        /// majd leáll és elrejti mindkét ikont
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Megállítja a stoppert
            timer1.Stop();

            // Elrejti mindkét ikont
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Visszaállítja a firstClicked és secondClicked 
            // értékét, így legközelebb ha címkére kattint,
            // akkor tudja majd a program, hogy ez lesz az első kattintás
            firstClicked = null;
            secondClicked = null;
        }

        private void timerGametime_Tick(object sender, EventArgs e)
        {
            string elapsedTime = stopwatch.Elapsed.ToString();
            labelGametime.Text = elapsedTime.Substring(0, elapsedTime.IndexOf('.'));
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            firstClicked.ForeColor = firstClicked.BackColor;
            firstClicked = null;
            PlaySound(SetSound("hide"));
        }

        /// <summary>
        /// Ellenőrzi, hogy meg lett e az összes ikon párja úgy,
        /// hogy összehasonlítja az előtér színét a háttérszínnel.
        /// Ha mindegyik ikonnak megegyezik, akkor a játékos nyert
        /// </summary>
        private void CheckForWinner()
        {
            // Végighalad mindegyik címkén a TableLayoutPanel-en, 
            // ellenőrzi, hogy mindegyik ikon megegyezik-e
            foreach (Control control in tableLayoutPanel1.Controls)
            {
                Label iconLabel = control as Label;

                if (iconLabel != null)
                {
                    if (iconLabel.ForeColor == iconLabel.BackColor)
                        return;
                }
            }

            // Ha a ciklus nem tér vissza, akkor nem talált
            // több párosítatlan ikont
            // Ami azt jelenti, hogy a játékos nyert. Megmutatja az üzenetet és bezárja az űrlapot
            // A játékidőt mérő stoppert pedig leállítjuk
            stopwatch.Stop();
            MessageBox.Show($"Sikerült megtalálni az összes párt!\n{labelGametime.Text}", "Gratulálok");
            Close();
        }

    }
}
