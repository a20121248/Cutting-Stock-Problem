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

namespace CSP.View
{
    internal partial class FormGenetico : Form
    {
        private CuttingStockProblem csp;
        private Cromosoma cromosoma;
        private List<Stock> listaStocksConPiezas;
        private List<Rectangulo> listaPiezas;

        public FormGenetico(FormPedidos formPedidos)
        {
            InitializeComponent();
            this.listaPiezas = formPedidos.listaPiezas;
            csp = new CuttingStockProblem(formPedidos.listaPiezas, formPedidos.listaStocks);
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            double probabilidadMutacion = double.Parse(txtProbabilidadMutacion.Text);
            int tamanhoPoblacion = int.Parse(txtTamanhoPoblacion.Text);
            double pesoMinimizarRectangulo = double.Parse(txtPesoMinimizarRectangulo.Text);
            double pesoFactorCuadratura = double.Parse(txtPesoFactorCuadratura.Text);

            Data data = new Data(probabilidadMutacion, tamanhoPoblacion, pesoMinimizarRectangulo, pesoFactorCuadratura);
            FormCargando loading = new FormCargando(FormCargando.ALGORITMO_GENETICO, csp, data);
            loading.ShowDialog(this);

            //csp.IniciarAlgoritmoGenetico(probabilidadMutacion, tamanhoPoblacion, pesoMinimizarRectangulo, pesoFactorCuadratura);
            csp.IniciarAlgoritmoStocks();
            csp.IniciarAlgoritmoDefectos();

            this.cromosoma = csp.cromosoma;
            this.listaStocksConPiezas = csp.listaStocks;


            /*
            AlgoritmoGenetico algGenetico = new AlgoritmoGenetico(this);
            solucion = algGenetico.mejorCromosoma.ListaGenes;
            fitness = algGenetico.mejorCromosoma.Fitness;
            arbol_solucion = algGenetico.mejorCromosoma.Tree;
            */
        }

        private void btnVerResultado_Click(object sender, EventArgs e)
        {
            FormResultado formResultado = new FormResultado(this.listaStocksConPiezas);
            formResultado.Show();
        }

        private void btnVerStockInfinito_Click(object sender, EventArgs e)
        {
            FormResultadoStockInfinito formResultadoStockInfinito = new FormResultadoStockInfinito(this.csp.algoritmoGenetico.CrearCromosoma(this.cromosoma.ListaGenes, this.listaPiezas));
            formResultadoStockInfinito.Show();
        }
    }
}
