using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BinPacking2D
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCarregamento_Click(object sender, EventArgs e)
        {
            ProblemaCarregamento2D MeuProblema = new ProblemaCarregamento2D();
            MeuProblema.Instancia = new InstanciaProblemaCarregamento2D();
            MeuProblema.Instancia.LerCaminhao(@"C:\Teste\CaminhaoCarregamento.txt");
            MeuProblema.Instancia.LerObjetos(@"C:\Teste\ObjetosCarregamento.txt");
            int[] SequenciaTeste = new int[MeuProblema.Instancia.QuantidadeObjetosParaCarregar];
            for(int i=0;i<MeuProblema.Instancia.QuantidadeObjetosParaCarregar;i++)
            {
                SequenciaTeste[i] = MeuProblema.Instancia.QuantidadeObjetosParaCarregar - i -1;
            }
            MeuProblema.Solucao = MeuProblema.MetodoSimulatedAnnealing(SequenciaTeste, 100);
            MeuProblema.Desenhar();
            pictureBox1.Image = MeuProblema.Desenho;
        }
    }
}
