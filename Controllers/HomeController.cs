using AuvoTech.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Text;

namespace AuvoTech.Controllers
{
    public class HomeController : Controller
    {
        //Define uma instância de IHostingEnvironment
        IHostingEnvironment _appEnvironment;
        //Injeta a instância no construtor para poder usar os recursos
        public HomeController(IHostingEnvironment env)
        {
            _appEnvironment = env;
        }
        //Retorna a View Index.cshtml que será o formulário para
        //selecionar os arquivos a serem enviados 
        public IActionResult Index()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //método para enviar os arquivos usando a interface IFormFile
        public async Task<IActionResult> EnviarArquivo(List<IFormFile> arquivos)
        {
            //INICIO DO TRATAMENTO DE ARQUIVO

            //Verifica se foi enviado algum arquivo
            if (arquivos.Count == 0)
            {
                ViewData["Error"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }         
            // VERIFICA SE OS ARQUIVOS ENVIADOS SEGUEM OS PADROES EXIGIDOS PELO APLICATIVO
            foreach (var arquivo in arquivos)
            {
                //verifica se existem arquivos vazios e alerta o usuario. Necessario (?)
                if (arquivo == null || arquivo.Length == 0)
                {
                    ViewData["Error"] = $"Aviso: Voce importou um arquivo vazio {arquivo.FileName}.";
                    return View(ViewData);
                }
                //Verifica e restringe o tipo de arquivo enviado
                if (!arquivo.FileName.Contains(".ofx"))
                {
                    ViewData["Error"] = "Aviso: Arquivo Não Suportado. Somente arquivos do tipo '.ofx' são aceitos na aplicação. Tente novamente.";
                    return View(ViewData);
                }
                //Criacao de variaveis e Armazenamento dos Arquivos
                //var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "EmailRegister.htm");
                string wwwRoot  = _appEnvironment.WebRootPath;
                string fileDb   = "\\Arquivos\\";
                string fileDest = "\\Recebidos\\";
                string fileName = arquivo.FileName.ToString();
                string fullPath = wwwRoot + fileDb + fileDest + fileName;

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                string readFile = System.IO.File.ReadAllText(fullPath);

                ViewData["Read"] = $"{readFile}";
            }

            //FINAL
            //Mostra a leitura dos arquivos .OFX removendo as transacoes repetidas 
            ViewData["Resultado"] = $"Falta fazer isso pra terminar o desafio";
            return View(ViewData);
        }

    }
}