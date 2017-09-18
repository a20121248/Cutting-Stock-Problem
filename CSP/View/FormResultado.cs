﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSP.Controller;
using CSP.Model;
using System.Reflection;

namespace CSP.View
{
    internal partial class FormResultado : Form
    {
        public Nodo arbol_solucion;
        public List<Rectangulo> listaPiezas;
        public List<Stock> listaStocks;
        public List<Pen> listaPencils;
        public List<Brush> listaBrushes;
        public List<Tuple<float, float>> listaPosicionesStocks;
        public Pen penRojo;
        public float factorImg = 2.5f;

        public FormResultado(FormGenetico formGenetico)
        {
            InitializeComponent();
            txtMejorSolucion.Text = Utilities.ConvertirListaACadena(formGenetico.solucion);
            txtFitness.Text = string.Format("{0:0.00}", formGenetico.fitness);
            arbol_solucion = formGenetico.arbol_solucion;
            listaPiezas = formGenetico.listaPiezas;
            listaStocks = formGenetico.listaStocks;
            InicializarColores();
            InicializarPosicionesStocks();
        }

        private void InicializarPosicionesStocks()
        {
            listaPosicionesStocks = new List<Tuple<float, float>>();

            float x = 20 * factorImg;
            float y = 20 * factorImg;
            for (int i = 1; i <= listaStocks.Count; ++i)
            {
                Tuple<float, float> posicion = new Tuple<float, float>(x, y);
                listaPosicionesStocks.Add(posicion);

                x += 100 * factorImg;
                if (i % 3 == 0)
                {
                    x = 20 * factorImg;
                    y += 100 * factorImg;
                }
            }
        }

        private void InicializarColores()
        {
            penRojo = new Pen(new SolidBrush(Color.Red), 1);

            Random rnd = new Random();
            int random;

            listaBrushes = new List<Brush>();
            listaPencils = new List<Pen>();

            Type brushesType = typeof(Brushes);
            Type pencilsType = typeof(Pens);
            PropertyInfo[] brushesProperties = brushesType.GetProperties();
            PropertyInfo[] pencilsProperties = pencilsType.GetProperties();
            for (int i = 0; i < listaPiezas.Count; ++i)
            {
                random = rnd.Next(brushesProperties.Length);
                Brush brush = Brushes.Transparent;
                brush = (Brush)brushesProperties[random].GetValue(null, null);
                listaBrushes.Add(brush);

                random = rnd.Next(pencilsProperties.Length);
                Pen pencil;
                pencil = (Pen)pencilsProperties[random].GetValue(null);
                listaPencils.Add(pencil);
            }            
        }

        private void DibujarPiezas(Graphics gr, Nodo arbol, float x, float y)
        {
            Rectangulo pieza = arbol.Rect;
            int pieza_id = pieza.Id;
            // Si es una hoja, entonces dibujar la pieza y detener la recursion
            if (arbol.Izquierdo == null && arbol.Derecho == null)
            {
                float x_abs_pieza = arbol.Rect.X + x;
                float y_abs_pieza = arbol.Rect.Y + y;
                float ancho_pieza = listaPiezas[pieza_id].W;
                float alto_pieza = listaPiezas[pieza_id].H;

                Rectangle rect = new Rectangle((int)x_abs_pieza, (int)y_abs_pieza, (int)ancho_pieza, (int)alto_pieza);
                gr.FillRectangle(listaBrushes[pieza_id], rect);
                //gr.DrawRectangle(listaPencils[pieza_id], x_abs_pieza, y_abs_pieza, ancho_pieza, alto_pieza);
                return;
            }
            // Si es un arbol, llamar de forma recursiva a sus hijos
            else
            {
                float x_abs_bloque = arbol.Rect.X + x;
                float y_abs_bloque = arbol.Rect.Y + y;
                DibujarPiezas(gr, arbol.Izquierdo, x_abs_bloque, y_abs_bloque);
                DibujarPiezas(gr, arbol.Derecho, x_abs_bloque, y_abs_bloque);
            }
        }

        private void DibujarStocks(Graphics gr)
        {
            foreach (Stock stock in listaStocks)
            {
                int id = stock.Id;
                float h = stock.H;
                float w = stock.W;
                //gr.DrawRectangle(listaPencils[id], 0, 0, w, h);
                gr.DrawRectangle(penRojo, listaPosicionesStocks[id-1].Item1, listaPosicionesStocks[id-1].Item2, w, h);
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(splitContainer1.Panel1.BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int ancho = 40;
            int alto = 15;
            int offset = 30;
            int x = 20;
            int y = 150;
            for (int i = 0; i < listaPiezas.Count; ++i)
            {
                Rectangle rect = new Rectangle(x, y, ancho, alto);
                e.Graphics.FillRectangle(listaBrushes[i], rect);
                e.Graphics.DrawString(string.Format("Pieza #{0}", i + 1),
                                      this.Font, Brushes.Black, new Point(x + ancho + 10, y));
                y = y + offset;
            }
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(splitContainer1.Panel1.BackColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            DibujarPiezas(e.Graphics, arbol_solucion,
                          listaPosicionesStocks[0].Item1 + arbol_solucion.Rect.X,
                          listaPosicionesStocks[0].Item2 + arbol_solucion.Rect.Y);
            DibujarStocks(e.Graphics);
        }
    }
}
