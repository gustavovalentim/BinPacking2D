using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace BinPacking2D
{
    class ObjetoCarregamento2D
    {
        public string Nome;
        public int Altura;
        public int Largura;
    }
    class InstanciaProblemaCarregamento2D
    {
        public int QuantidadeObjetosParaCarregar;
        public ObjetoCarregamento2D[] Objetos;
        public ObjetoCarregamento2D Caminhao;
        public void LerObjetos(string Arquivo)
        {
            string[] s = File.ReadAllLines(Arquivo);
            QuantidadeObjetosParaCarregar = s.GetLength(0);
            Objetos = new ObjetoCarregamento2D[QuantidadeObjetosParaCarregar];
            for (int i = 0; i < QuantidadeObjetosParaCarregar; i++)
            {
                string[] s1 = s[i].Split(';');
                Objetos[i] = new ObjetoCarregamento2D();
                Objetos[i].Nome = s1[0];
                Objetos[i].Altura = int.Parse(s1[1]);
                Objetos[i].Largura = int.Parse(s1[2]);
            }
        }
        public void LerCaminhao(string Arquivo)
        {
            string[] s = File.ReadAllLines(Arquivo);
            string[] s1 = s[0].Split(';');
            Caminhao = new ObjetoCarregamento2D();
            Caminhao.Nome = s1[0];
            Caminhao.Altura = int.Parse(s1[1]);
            Caminhao.Largura = int.Parse(s1[2]);
        }
    }
    class SolucaoObjetoCarregamento2D
    {
        public int InicioX;
        public int InicioY;
        public bool Carregado;
    }
    class SolucaoProblemaCarregamento2D
    {
        public SolucaoObjetoCarregamento2D[] SolucaoObjetosCarregamento;
        public int[] Alturas;
        public int AreaUtilizada;
    }
    class ProblemaCarregamento2D
    {
        public InstanciaProblemaCarregamento2D Instancia;
        public SolucaoProblemaCarregamento2D Solucao;
        public Bitmap Desenho;
        public void Desenhar()
        {
            int DesenhoAltura = 300;
            int DesenhoLargura = 480;
            int Escala = 30;
            Desenho = new Bitmap(DesenhoLargura, DesenhoAltura);
            Graphics g = Graphics.FromImage(Desenho);
            g.FillRectangle(Brushes.Red, 0, 0, DesenhoLargura, DesenhoAltura);
            g.DrawRectangle(Pens.Blue, 0, 0, DesenhoLargura, DesenhoAltura);
            for (int i = 0; i < Solucao.SolucaoObjetosCarregamento.GetLength(0); i++)
            {
                if (Solucao.SolucaoObjetosCarregamento[i].Carregado)
                {
                    int Xinicio = Solucao.SolucaoObjetosCarregamento[i].InicioX;
                    int Xfim = Xinicio + Instancia.Objetos[i].Largura;
                    int Yfim = Instancia.Caminhao.Altura - Solucao.SolucaoObjetosCarregamento[i].InicioY;
                    int Yinicio = Yfim - Instancia.Objetos[i].Altura;
                    g.FillRectangle(Brushes.AliceBlue, Escala * Xinicio, Escala * Yinicio, Escala * Instancia.Objetos[i].Largura, Escala * Instancia.Objetos[i].Altura);
                    g.DrawRectangle(Pens.Blue, Escala * Xinicio, Escala * Yinicio, Escala * Instancia.Objetos[i].Largura, Escala * Instancia.Objetos[i].Altura);
                }
            }
            Desenho.Save(@"C:\Teste\DesenhoCarregamento.jpg");
        }
        public SolucaoProblemaCarregamento2D MetodoSimulatedAnnealing(int[] SequenciaIncial, double TemperaturaInicial)
        {
            Random Aleatorio = new Random();
            SolucaoProblemaCarregamento2D SolucaoMelhor = MetodoBaixoDireita(SequenciaIncial);
            int[] SequenciaMelhor = new int[SequenciaIncial.GetLength(0)];
            int[] SequenciaAtual = new int[SequenciaIncial.GetLength(0)];
            for (int p = 0; p < SequenciaIncial.GetLength(0); p++)
            {
                SequenciaAtual[p] = SequenciaIncial[p];
                SequenciaMelhor[p] = SequenciaIncial[p];
            }
            
            SolucaoProblemaCarregamento2D SolucaoAtual = MetodoBaixoDireita(SequenciaIncial);
            SolucaoProblemaCarregamento2D SolucaoCandidata;
            double TemperaturaAtual = TemperaturaInicial;
            while(TemperaturaAtual>0.1)
            {
                int[] SequenciaCandidata = MelhorSequenciaVizinha(SequenciaAtual);
                SolucaoCandidata = MetodoBaixoDireita(SequenciaCandidata);
                if (SolucaoCandidata.AreaUtilizada > SolucaoMelhor.AreaUtilizada)
                {
                    for (int p = 0; p < SequenciaAtual.GetLength(0); p++)
                    {
                        SequenciaMelhor[p] = SequenciaCandidata[p];
                    }
                    SolucaoMelhor = MetodoBaixoDireita(SequenciaMelhor);
                }
                if (SolucaoCandidata.AreaUtilizada > SolucaoAtual.AreaUtilizada)
                {
                    for (int p = 0; p < SequenciaAtual.GetLength(0); p++)
                    {
                        SequenciaAtual[p] = SequenciaCandidata[p];
                    }
                    SolucaoAtual = MetodoBaixoDireita(SequenciaAtual);
                }
                else
                {
                    int Delta = SolucaoCandidata.AreaUtilizada - SolucaoAtual.AreaUtilizada;
                    if (Aleatorio.NextDouble() > Math.Exp(-Delta / TemperaturaAtual))
                    {
                        for (int p = 0; p < SequenciaAtual.GetLength(0); p++)
                        {
                            SequenciaAtual[p] = SequenciaCandidata[p];
                        }
                        SolucaoAtual = MetodoBaixoDireita(SequenciaAtual);
                    }
                }
                TemperaturaAtual *= 0.9;
            }
            return SolucaoMelhor;
        }
        public int[] MelhorSequenciaVizinha(int[] Sequencia)
        {
            SolucaoProblemaCarregamento2D MelhorSolucaoVizinha = new SolucaoProblemaCarregamento2D();
            int[] SequenciaVizinhaAtual = new int[Sequencia.GetLength(0)];
            int[] SequenciaMelhorVizinha = new int[Sequencia.GetLength(0)];
            for (int i = 0; i < Sequencia.GetLength(0); i++)
            {
                for (int j = i + 1; j < Sequencia.GetLength(0); j++)
                {
                    for (int p = 0; p < Sequencia.GetLength(0); p++)
                    {
                        SequenciaVizinhaAtual[p] = Sequencia[p];
                    }
                    SequenciaVizinhaAtual[i] = Sequencia[j];
                    SequenciaVizinhaAtual[j] = Sequencia[i];
                    SolucaoProblemaCarregamento2D SolucaoVizinhaAtual = MetodoBaixoDireita(SequenciaVizinhaAtual);
                    if(SolucaoVizinhaAtual.AreaUtilizada>MelhorSolucaoVizinha.AreaUtilizada)
                    {
                        MelhorSolucaoVizinha = MetodoBaixoDireita(SequenciaVizinhaAtual);
                        for (int p = 0; p < Sequencia.GetLength(0); p++)
                        {
                            SequenciaMelhorVizinha[p] = SequenciaVizinhaAtual[p];
                        }
                    }
                }
            }
            return SequenciaMelhorVizinha;
        }
        public SolucaoProblemaCarregamento2D MetodoBaixoDireita(int[] Sequencia)
        {
            SolucaoProblemaCarregamento2D SolucaoAnalisada = new SolucaoProblemaCarregamento2D();
            SolucaoAnalisada.AreaUtilizada = 0; 
            SolucaoAnalisada.Alturas = new int[Instancia.Caminhao.Largura];
            SolucaoAnalisada.SolucaoObjetosCarregamento = new SolucaoObjetoCarregamento2D[Instancia.QuantidadeObjetosParaCarregar];
            for (int i = 0; i < Sequencia.GetLength(0); i++)
            {
                int IndiceObjetoAtual = Sequencia[i];
                int AlturaMaxima = EncontraMaiorValorAltura(0, Instancia.Objetos[IndiceObjetoAtual].Largura - 1, SolucaoAnalisada.Alturas);
                int Deslocamento = EncontraMaiorDeslocamentoDireita(AlturaMaxima, Instancia.Objetos[IndiceObjetoAtual].Largura, SolucaoAnalisada.Alturas);
                int NovaAltura = Instancia.Objetos[IndiceObjetoAtual].Altura + AlturaMaxima;
                SolucaoAnalisada.SolucaoObjetosCarregamento[IndiceObjetoAtual] = new SolucaoObjetoCarregamento2D();
                if (NovaAltura <= Instancia.Caminhao.Altura)
                {
                    SolucaoAnalisada.SolucaoObjetosCarregamento[IndiceObjetoAtual].Carregado = true;
                    SolucaoAnalisada.SolucaoObjetosCarregamento[IndiceObjetoAtual].InicioX = Deslocamento;
                    SolucaoAnalisada.SolucaoObjetosCarregamento[IndiceObjetoAtual].InicioY = AlturaMaxima;
                    for (int j = Deslocamento; j < Deslocamento + Instancia.Objetos[IndiceObjetoAtual].Largura; j++)
                    {
                        SolucaoAnalisada.Alturas[j] = NovaAltura;
                    }
                    SolucaoAnalisada.AreaUtilizada += Instancia.Objetos[IndiceObjetoAtual].Altura * Instancia.Objetos[IndiceObjetoAtual].Largura;
                }
                else
                {
                    SolucaoAnalisada.SolucaoObjetosCarregamento[IndiceObjetoAtual].Carregado = false;
                }
            }
            return SolucaoAnalisada;
        }
        public int EncontraMaiorValorAltura(int inicio, int fim, int[] AlturasAnalisadas)
        {
            if(fim >= AlturasAnalisadas.GetLength(0))
            {
                fim = AlturasAnalisadas.GetLength(0) - 1;
            }
            int Maximo = int.MinValue;
            for (int i = inicio; i <= fim; i++)
            {
                if (AlturasAnalisadas[i] > Maximo) 
                {
                    Maximo = AlturasAnalisadas[i];
                }
            }
            return Maximo;
        }
        public int EncontraMaiorDeslocamentoDireita(int AlturaReferencia, int inicio, int[] AlturasAnalisadas)
        {
            int Deslocamento = 0;
            int posicao = inicio;
            while(posicao<Instancia.Caminhao.Largura && AlturasAnalisadas[posicao] <= AlturaReferencia)
            {
                Deslocamento++;
                posicao++;
            }
            return Deslocamento;
        }
    }
}
