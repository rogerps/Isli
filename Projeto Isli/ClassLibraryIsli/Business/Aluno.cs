using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibraryIsli.Business
{
    class Aluno
    {
        public int IdAluno { get; set; }
        public string Nome { get; set; }
        public string Matricula { get; set; }
        public int Cor { get; set; }
        public int NecessidadeEspeciais { get; set; }
        public int Pais { get; set; }
        public string CidadeNascimento { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public DateTime DataMatricula { get; set; }
        public string Rg { get; set; }
        public string Cpf { get; set; }
        public string OrgaoEmissor { get; set; }
        public DateTime DataEmissao { get; set; }
        public string CertidaoNascimento { get; set; }
        public DateTime DataEmissaoCertidao { get; set; }
        public string MunicipioCertidao { get; set; }
        public string Estado { get; set; }
        public int AnoLetivo { get; set; }
        public Endereco Endereco { get; set; }
        public Responsavel ResponsavelPedagogico { get; set; }
        public Responsavel ResponsavelFinanceiro { get; set; }


    }
}
