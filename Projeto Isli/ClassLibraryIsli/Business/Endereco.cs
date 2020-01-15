using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibraryIsli.Business
{
    class Endereco
    {
        public int IdEndereco { get; set; }
        public int Cep { get; set; }
        public string NomeEndereco { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public Pais Pais { get; set; }
    }
}
